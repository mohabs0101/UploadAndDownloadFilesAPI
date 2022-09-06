using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using System.Web.Http.Cors;
namespace PDFrepoAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("multipart/form-data"));

            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}",
                defaults: new { id = RouteParameter.Optional }
            );
            config.EnableCors(/*cors*/);

        }

    }

}
