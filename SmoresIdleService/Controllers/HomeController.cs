using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using SmoresIdleService.Hubs;

namespace SmoresIdleService.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult HealthCheck()
		{
			return new ContentResult { Content = "Good to go!", ContentType = "text/plain" };
		}

		public ActionResult HubHealthCheck()
		{
			IHubContext context = GlobalHost.ConnectionManager.GetHubContext<StatusHub>();
			context.Clients.All.HealthCheck("Feeling good!");
			return new ContentResult { Content = "Good to go!", ContentType = "text/plain" };
		}
	}
}
