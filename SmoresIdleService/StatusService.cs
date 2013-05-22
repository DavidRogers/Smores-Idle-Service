using System;
using System.Configuration;
using System.Data.Linq;
using System.Linq;
using System.Web;
using System.Web.Caching;
using Microsoft.AspNet.SignalR.Hubs;
using SmoresIdleService.Models;

namespace SmoresIdleService
{
	public class StatusService
	{
		public static UserStatusModel GetStatus(string token)
		{
			if (string.IsNullOrEmpty(token) || token.Length != 32)
				return null;

			UserStatusModel model = HttpRuntime.Cache.Get(token) as UserStatusModel;
			if (model == null)
			{
				string uriString = ConfigurationManager.AppSettings["SQLSERVER_CONNECTION_STRING"];
				using (UserStatusDataContext context = new UserStatusDataContext(uriString))
				{
					UserStatus user = context.UserStatus.FirstOrDefault(x => x.UserHash == token);
					model = new UserStatusModel
					{
						Status = user == null ? (int) UserStatusEnum.Unknown : user.Status,
						LastUpdated = user == null ? new DateTime().ToUniversalTime() : user.LastUpdated,
						Token = token
					};
					// these are some basic sanity checks in the case where someone stopped using the client for long periods of time
					if (model.Status == (int) UserStatusEnum.Active && (DateTime.UtcNow - model.LastUpdated) > TimeSpan.FromDays(2))
						model.Status = (int) UserStatusEnum.Unknown;
					if (model.Status == (int) UserStatusEnum.Away && (DateTime.UtcNow - model.LastUpdated) > TimeSpan.FromDays(7))
						model.Status = (int) UserStatusEnum.Unknown;
					if (model.Status == (int) UserStatusEnum.Busy && (DateTime.UtcNow - model.LastUpdated) > TimeSpan.FromDays(1))
						model.Status = (int) UserStatusEnum.Unknown;
					if (model.Status == (int) UserStatusEnum.Idle && (DateTime.UtcNow - model.LastUpdated) > TimeSpan.FromHours(4))
						model.Status = (int) UserStatusEnum.Unknown;
				}

				HttpRuntime.Cache.Add(token, model, null, Cache.NoAbsoluteExpiration, TimeSpan.FromHours(24), CacheItemPriority.Normal, null);
			}
			return model;
		}

		public static bool UpdateStatus(UserStatusModel userStatus)
		{
			if (userStatus == null || string.IsNullOrEmpty(userStatus.Token) || userStatus.Token.Length != 32 || !Enum.IsDefined(typeof(UserStatusEnum), userStatus.Status))
				return false;

			userStatus.LastUpdated = DateTime.UtcNow;
			string uriString = ConfigurationManager.AppSettings["SQLSERVER_CONNECTION_STRING"];
			using (UserStatusDataContext context = new UserStatusDataContext(uriString))
			{
				UserStatus user = context.UserStatus.FirstOrDefault(x => x.UserHash == userStatus.Token);
				if (user != null)
				{
					if (user.Status == userStatus.Status)
						return true;

					user.Status = userStatus.Status;
					user.LastUpdated = DateTime.UtcNow;
					HttpRuntime.Cache.Remove(userStatus.Token);
				}
				else
				{
					context.UserStatus.InsertOnSubmit(new UserStatus { UserHash = userStatus.Token, Status = userStatus.Status, LastUpdated = DateTime.UtcNow });
				}

				context.SubmitChanges(ConflictMode.ContinueOnConflict);
			}

			return true;
		}

		public static void NotifyStatusSubscribers(IHubConnectionContext hubContext, UserStatusModel userStatus)
		{
			hubContext.Group(userStatus.Token).StatusChanged(userStatus);
		}
	}
}