using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using ApiAggregator.Web.Framework;
using ApiAggregator.Web.Handlers;
using ApiAggregator.Web.Extensions;

namespace ApiAggregator.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var customHandler = GetCustomHandler(config);

            config.Routes.MapHttpRoute(
                name: "DynamicRoute",
                routeTemplate: "api/{*endpoint}",
                defaults: new { controller = "ApiDispatch", action = "Execute" },
                constraints: null,
                handler: customHandler
            );

            var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);
        }

        private static HttpMessageHandler GetCustomHandler(HttpConfiguration config)
        {
            var apiMappingHandler = config.DependencyResolver.GetService<ApiMappingHandler>();
            var responseHeadersHandler = config.DependencyResolver.GetService<ResponseHeadersHandler>();            
            var securityHandler = config.DependencyResolver.GetService<SecurityHandler>();

            return HttpClientFactory.CreatePipeline(new HttpControllerDispatcher(config),
                new DelegatingHandler[]
                { 
                    apiMappingHandler, securityHandler, responseHeadersHandler
                }
            );
        }
    }
}