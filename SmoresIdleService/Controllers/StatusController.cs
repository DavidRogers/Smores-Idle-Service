using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Caching;
using System.Web.Http;
using SmoresIdleService.Models;

namespace SmoresIdleService.Controllers
{
	public enum UserStatusEnum
	{
		Unknown = 0,
		Idle = 1,
		Busy = 3,
		Away = 4,
		Active = 5
	}

	public class StatusController : ApiController
	{
		public UserStatusModel Get(string token)
		{
			if (string.IsNullOrEmpty(token))
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

		public HttpResponseMessage Post(UserStatusModel userStatus)
		{
			if (ModelState.IsValid)
			{
				string uriString = ConfigurationManager.AppSettings["SQLSERVER_CONNECTION_STRING"];
				using (UserStatusDataContext context = new UserStatusDataContext(uriString))
				{
					UserStatus user = context.UserStatus.FirstOrDefault(x => x.UserHash == userStatus.Token);
					if (user != null)
					{
						user.Status = userStatus.Status;
						user.LastUpdated = DateTime.UtcNow;
						HttpRuntime.Cache.Remove(userStatus.Token);
					}
					else
					{
						context.UserStatus.InsertOnSubmit(new UserStatus { UserHash = userStatus.Token, Status = userStatus.Status, LastUpdated = DateTime.UtcNow });
					}

					context.SubmitChanges();
				}
				return new HttpResponseMessage(HttpStatusCode.Accepted);
			}

			throw new HttpResponseException(HttpStatusCode.BadRequest);
		}
	}
}