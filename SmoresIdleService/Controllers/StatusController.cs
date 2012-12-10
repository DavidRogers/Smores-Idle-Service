using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using SmoresIdleService.Hubs;
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
			{
				// tell the hub connections about this event during the transition!
				IHubContext context = GlobalHost.ConnectionManager.GetHubContext<StatusHub>();
				StatusHelper.NotifyStatusSubscribers(context.Clients, userStatus);

				return new HttpResponseMessage(HttpStatusCode.Accepted);
			}

			return new HttpResponseMessage(HttpStatusCode.BadRequest);
		}
	}
}