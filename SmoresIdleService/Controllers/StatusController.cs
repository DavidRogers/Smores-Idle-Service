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
			return StatusHelper.GetStatus(token);
		}

		public HttpResponseMessage Post(UserStatusModel userStatus)
		{
			if (ModelState.IsValid && StatusHelper.UpdateStatus(userStatus))
				return new HttpResponseMessage(HttpStatusCode.Accepted);

			return new HttpResponseMessage(HttpStatusCode.BadRequest);
		}
	}
}