using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.Mvc;

namespace SmoresIdleService
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			//				routeTemplate: "status/user/{id}",
			//				defaults: new { controller = "Status", action = "UniqueUser", id = RouteParameter.Optional }

			config.Routes.MapHttpRoute("StatusApi", "status/user/{id}",
				new { controller = "Status", action = "UniqueUser" },
				new { httpMethod = new HttpMethodConstraint(HttpMethod.Get, HttpMethod.Post) }
			);
		}
	}
}
