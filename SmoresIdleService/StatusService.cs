using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Caching;
using Microsoft.AspNet.SignalR.Hubs;
using SmoresIdleService.Hubs;
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
					user.Status = userStatus.Status;
					user.LastUpdated = userStatus.LastUpdated;
					HttpRuntime.Cache.Remove(userStatus.Token);
				}
				else
				{
					context.UserStatus.InsertOnSubmit(new UserStatus { UserHash = userStatus.Token, Status = userStatus.Status, LastUpdated = userStatus.LastUpdated });
				}

				context.SubmitChanges();
			}

			return true;
		}

		internal static SubscriptionService GetSubscriptionsFromCache()
		{
			return (SubscriptionService) HttpRuntime.Cache.Get(StatusHub.UserSubscriptionsCacheKey);
		}

		public static void NotifyStatusSubscribers(IHubConnectionContext hubContext, UserStatusModel userStatus)
		{
			SubscriptionService subscriptionService = GetSubscriptionsFromCache();

			List<string> subsciptions;
			if (subscriptionService.ReverseUserSubscriptions.TryGetValue(userStatus.Token, out subsciptions))
			{
				foreach (string callerId in subsciptions)
					hubContext.Client(callerId).StatusChanged(userStatus);
			}
		}
	}
}