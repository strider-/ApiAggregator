using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Mvc;
using ApiAggregator.Core.Services.Models;

namespace ApiAggregator.Web.Extensions
{
    public static class RequestExtensions
    {
        public static HttpResponseMessage FailureResponse(this HttpRequestMessage request, HttpStatusCode code, string message)
        {
            return request.CreateResponse(code, new
            {
                Success = false,
                Message = message
            });
        }

        public static HttpResponseMessage UnauthorizedResponse(this HttpRequestMessage request)
        {
            return FailureResponse(request, HttpStatusCode.Unauthorized, "Authorization for the request has been denied.");
        }

        public static HttpResponseMessage DebugResponse(this HttpRequestMessage request, MappingContext context)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            var rep = request.GetUrlHelper().Route("DynamicRoute", new { endpoint = context.RequestedEndpoint });
            var content = CreateDebugObject(context, rep);
            response.Content = new ObjectContent(content.GetType(), content, new JsonMediaTypeFormatter());

            return response;
        }

        public static JsonResult DebugResponse(this HttpRequestBase request, MappingContext context, UrlHelper helper)
        {
            var result = new JsonResult();
            var rep = helper.HttpRouteUrl("DynamicRoute", new { endpoint = context.RequestedEndpoint });
            result.ContentType = "application/json";
            result.JsonRequestBehavior = JsonRequestBehavior.DenyGet;
            result.Data = CreateDebugObject(context, rep);

            return result;
        }

        private static object CreateDebugObject(MappingContext context, string requestedEndpoint)
        {
            return new
            {
                Success = true,
                Message = string.Empty,
                MappingName = context.Name,
                ServiceName = context.Service.Name,
                Method = context.Method.Method,
                context.MappingEndpoint,
                context.ApiUrl,
                RoutedEndpoint = requestedEndpoint,
                context.InterpolatedApiUrl,
            };
        }
    }
}