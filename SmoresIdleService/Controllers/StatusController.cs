using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.SignalR;
using SmoresIdleService.Hubs;
using SmoresIdleService.Models;

namespace SmoresIdleService.Controllers
{
	public class StatusController : ApiController
	{
		public UserStatusModel Get(string token)
		{
			return StatusService.GetStatus(token);
		}

		public HttpResponseMessage Post(UserStatusModel userStatus)
		{
			if (ModelState.IsValid && StatusService.UpdateStatus(userStatus))
			{
				// tell the hub connections about this event during the transition!
				IHubContext context = GlobalHost.ConnectionManager.GetHubContext<StatusHub>();
				StatusService.NotifyStatusSubscribers(context.Clients, userStatus);

				return new HttpResponseMessage(HttpStatusCode.Accepted);
			}

			return new HttpResponseMessage(HttpStatusCode.BadRequest);
		}
	}
}