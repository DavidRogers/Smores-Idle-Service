using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.AspNet.SignalR;

namespace SmoresIdleService
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class WebApiApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			RouteTable.Routes.MapHubs("/streaming", new HubConfiguration { EnableJavaScriptProxies = false });

			AreaRegistration.RegisterAllAreas();

			WebApiConfig.Register(GlobalConfiguration.Configuration);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

			RouteConfig.RegisterRoutes(RouteTable.Routes);

			BundleConfig.RegisterBundles(BundleTable.Bundles);
		}
	}
}