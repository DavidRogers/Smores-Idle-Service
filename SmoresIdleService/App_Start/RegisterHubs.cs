using System.Web;
using System.Web.Routing;
using Microsoft.AspNet.SignalR;

[assembly: PreApplicationStartMethod(typeof(SmoresIdleService.RegisterHubs), "Start")]

namespace SmoresIdleService
{
	public static class RegisterHubs
	{
		public static void Start()
		{
			// Register the default hubs route: ~/signalr/hubs
			RouteTable.Routes.MapHubs("~/streaming");
		}
	}
}
