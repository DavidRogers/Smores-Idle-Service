using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using SmoresIdleService.Models;

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
		public HttpResponseMessage Post(UserStatusModel userStatus)
		{
			string uriString = ConfigurationManager.AppSettings["SQLSERVER_CONNECTION_STRING"];
			using (UserStatusDataContext context = new UserStatusDataContext(uriString))
			{
				UserStatus user = context.UserStatus.FirstOrDefault(x => x.UserHash == userStatus.Token);
				return new HttpResponseMessage
				{
					Content = new ObjectContent(typeof(UserStatus), new UserStatusModel
						{
							Status = user == null ? (int) UserStatusEnum.Unknown : user.Status,
							LastUpdated = user == null ? DateTime.UtcNow : user.LastUpdated,
							Token = userStatus.Token
						}, new JsonMediaTypeFormatter())
				};
			}
		}

		public void Put(UserStatusModel userStatus)
		{
			string uriString = ConfigurationManager.AppSettings["SQLSERVER_CONNECTION_STRING"];
			UserStatus user;
			using (UserStatusDataContext context = new UserStatusDataContext(uriString))
			{
				user = context.UserStatus.FirstOrDefault(x => x.UserHash == userStatus.Token);
				if (user != null)
				{
					user.Status = userStatus.Status;
					user.LastUpdated = DateTime.UtcNow;
				}
				else
				{
					context.UserStatus.InsertOnSubmit(user = new UserStatus { UserHash = userStatus.Token, Status = userStatus.Status, LastUpdated = DateTime.UtcNow });
				}

				context.SubmitChanges();
			}
		}
	}
}