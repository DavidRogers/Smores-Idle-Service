using System;
using System.Configuration;
using System.Linq;
using System.Web.Http;

namespace SmoresIdleService.Controllers
{
	public enum UserStatusEnum
	{
		Unknown = 0,
		Idle = 1,
		Busy = 3,
		Active = 5
	}

	public class StatusController : ApiController
	{
		public UserStatus Post(UserStatus userStatus)
		{
			string uriString = ConfigurationManager.AppSettings["SQLSERVER_CONNECTION_STRING"];
			using (UserStatusDataContext context = new UserStatusDataContext(uriString))
			{
				UserStatus user = context.UserStatus.FirstOrDefault(x => x.UserHash == userStatus.UserHash);
				return new UserStatus
				{
					Status = user == null ? (int) UserStatusEnum.Unknown : user.Status,
					LastUpdated = user == null ? DateTime.UtcNow : user.LastUpdated,
					UserHash = userStatus.UserHash
				};
			}
		}

		public UserStatus Put(UserStatus userStatus)
		{
			string uriString = ConfigurationManager.AppSettings["SQLSERVER_CONNECTION_STRING"];
			UserStatus user;
			using (UserStatusDataContext context = new UserStatusDataContext(uriString))
			{
				user = context.UserStatus.FirstOrDefault(x => x.UserHash == userStatus.UserHash);
				if (user != null)
				{
					user.Status = userStatus.Status;
					user.LastUpdated = DateTime.UtcNow;
				}
				else
				{
					context.UserStatus.InsertOnSubmit(user = new UserStatus { UserHash = userStatus.UserHash, Status = userStatus.Status, LastUpdated = DateTime.UtcNow });
				}

				context.SubmitChanges();
			}

			return new UserStatus
			{
				LastUpdated = user.LastUpdated,
				Status = user.Status,
				UserHash = user.UserHash
			};
		}
	}
}