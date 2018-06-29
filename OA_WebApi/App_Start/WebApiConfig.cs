using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace OA_WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务
            var origin = ConfigurationManager.AppSettings["origin"];
            var methods = ConfigurationManager.AppSettings["methods"];
            var headers = ConfigurationManager.AppSettings["headers"];
            config.EnableCors(new EnableCorsAttribute(origin, headers, methods));

            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
