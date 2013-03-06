using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using SmoresIdleService.Hubs;

namespace SmoresIdleService.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult HealthCheck()
		{
			return new ContentResult { Content = "Feeling good!", ContentType = "text/plain" };
		}

		public ActionResult HubHealthCheck()
		{
			try
			{
				IHubContext context = GlobalHost.ConnectionManager.GetHubContext<StatusHub>();
				////SubscriptionService service = StatusService.GetSubscriptionsFromCache();
				////if (service != null)
				////{
				////	int connections = service.UserSubscriptions.Count;
				////	if (connections > 0)
				////	{
				////		double average = service.UserSubscriptions.Average(x => x.Value.Count);
				////		int floor = service.UserSubscriptions.Min(x => x.Value.Count);
				////		int ceiling = service.UserSubscriptions.Max(x => x.Value.Count);
				////		int subscriptions = service.UserSubscriptions.Sum(x => x.Value.Count);

				////		StringBuilder healthContent = new StringBuilder();
				////		healthContent.AppendLine("Connection Health");
				////		healthContent.AppendLine(string.Format("{0} total connections", connections));
				////		healthContent.AppendLine(string.Format("{0} total subscriptions", subscriptions));
				////		healthContent.AppendLine(string.Format("{0} average subscriptions per connection", average));
				////		healthContent.AppendLine(string.Format("{0} subscriptions is the least", floor));
				////		healthContent.AppendLine(string.Format("{0} subscriptions is the most", ceiling));

				////		return new ContentResult { Content = healthContent.ToString(), ContentType = "text/plain" };
				////	}
				////	return new ContentResult { Content = "No connections", ContentType = "text/plain" };
				////}
				if (context == null)
					return new ContentResult { Content = "Could not resolve hub", ContentType = "text/plain" };

				return new ContentResult { Content = "Feeling good!", ContentType = "text/plain" };
			}
			catch (Exception e)
			{
				return new ContentResult { Content = "Fail! " + e.Message + "\r\n" + e.StackTrace, ContentType = "text/plain" };
			}

		}
	}
}
