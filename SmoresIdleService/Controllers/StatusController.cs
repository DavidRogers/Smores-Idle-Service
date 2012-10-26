using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace SmoresIdleService.Controllers
{
	public class StatusController : ApiController
	{
		[HttpGet]
		public int UniqueUser(string id)
		{
			using (UserStatusDataContext context = new UserStatusDataContext())
			{
				UserStatus user = context.UserStatus.FirstOrDefault(x => x.UserHash == id);
				return user == null ? -1 : user.Status;
			}
		}

		[HttpPost]
		public void UniqueUser(string id, int status)
		{
			using (UserStatusDataContext context = new UserStatusDataContext())
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

				try
				{
					context.SubmitChanges();
				}
				catch (Exception)
				{
					// log something...
				}
			}

		}
	}
}