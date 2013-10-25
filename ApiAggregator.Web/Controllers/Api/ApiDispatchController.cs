using System.Net;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using ApiAggregator.Core.Data;
using ApiAggregator.Core.Models;
using ApiAggregator.Core.Services;
using ApiAggregator.Core.Services.Models;
using ApiAggregator.Web.Extensions;
using System.Web;

namespace ApiAggregator.Web.Controllers.Api
{
    public class ApiDispatchController : ApiController
    {
        private readonly IContextGenerator _generator;
        private readonly IApiMappingRepository _repo;

        public ApiDispatchController(IApiMappingRepository repo, IContextGenerator generator)
        {
            _generator = generator;
            _repo = repo;
        }

        [HttpGet, HttpPost]
        public async Task<HttpResponseMessage> Execute()
        {
            var mapping = Request.Properties["Mapping"] as ApiMapping;
            var ep = Request.Properties["Endpoint"] as string;
            var context = _generator.GetMappingContext(mapping, ep);

            if(context == null)
            {
                return Request.FailureResponse(HttpStatusCode.InternalServerError, "Internal error attempting to resolve mapping context.");
            }

            if(IsDebugging())
            {
                return Request.DebugResponse(context);
            }
            else
            {                
                try
                {
                    return await context.Execute();
                }
                catch(HttpRequestException ex)
                {
                    return Request.FailureResponse(HttpStatusCode.InternalServerError, ex.InnerException.Message);
                }
            }
        }

        [HttpGet]
        public HttpResponseMessage Describe()
        {
            var result = _repo.EnabledMappings()
                .OrderBy(m => m.Service.Name)
                .ThenBy(m => m.Endpoint)
                .GroupBy(m => m.Service.Name)
                .Select(m => new
                {
                    Service = m.Key,
                    Endpoints = m.Select(y => new { y.Name, Endpoint = GetRoute(y.Endpoint) })
                });

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        private string GetRoute(string endpoint)
        {
            return HttpUtility.UrlDecode(Url.Route("DynamicRoute", new { endpoint }));
        }

        private bool IsDebugging()
        {
            if(Request == null)
            {
                return false;
            }

            var vars = Request.RequestUri.ParseQueryString();
            bool debug;
            bool.TryParse(vars["debug"], out debug);
            return debug;
        }
    }
}
