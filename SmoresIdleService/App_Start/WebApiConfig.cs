using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;

namespace SmoresIdleService
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			//				routeTemplate: "status/user/{id}",
			//				defaults: new { controller = "Status", action = "UniqueUser", id = RouteParameter.Optional }

			config.Routes.MapHttpRoute("StatusApi", "status/user",
				new { controller = "Status" },
				new { httpMethod = new HttpMethodConstraint(HttpMethod.Post, HttpMethod.Get) }
			);
		}
	}
}
