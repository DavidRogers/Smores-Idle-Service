using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
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
		[HttpGet]
		public int UniqueUser(string id)
		{
			string uriString = ConfigurationManager.AppSettings["SQLSERVER_CONNECTION_STRING"];
			using (UserStatusDataContext context = new UserStatusDataContext(uriString))
			{
				UserStatus user = context.UserStatus.FirstOrDefault(x => x.UserHash == id);
				return user == null ? (int) UserStatusEnum.Unknown : user.Status;
			}
		}

		[HttpPost]
		public void UniqueUser(string id, [FromBody]int status)
		{
			string uriString = ConfigurationManager.AppSettings["SQLSERVER_CONNECTION_STRING"];
			using (UserStatusDataContext context = new UserStatusDataContext(uriString))
			{
				UserStatus user = context.UserStatus.FirstOrDefault(x => x.UserHash == id);
				if (user != null)
				{
					user.Status = status;
					user.LastUpdated = DateTime.UtcNow;
				}
				else
				{
					context.UserStatus.InsertOnSubmit(new UserStatus { UserHash = id, Status = status, LastUpdated = DateTime.UtcNow });
				}

				context.SubmitChanges();
			}
		}
	}
}