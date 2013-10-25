using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ApiAggregator.Core.Data;
using ApiAggregator.Core.Services;
using ApiAggregator.Web.Extensions;

namespace ApiAggregator.Web.Handlers
{
    public class ApiMappingHandler : DelegatingHandler
    {
        private readonly IApiMappingRepository _apiRepo;
        private readonly IMatchingService _service;
        private readonly IConfigurationRepository _configRepo;

        public ApiMappingHandler(IApiMappingRepository apiRepo, IConfigurationRepository configRepo, IMatchingService service)
        {
            _apiRepo = apiRepo;
            _service = service;
            _configRepo = configRepo;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var routeData = request.GetRouteData();
            var ep = routeData.Values["endpoint"] as string;

            if(!string.IsNullOrWhiteSpace(ep))
            {
                var mapping = (from m in _apiRepo.EnabledMappings()
                               let w = _service.GetMappingWeight(m, ep)
                               where w > 0
                               orderby w descending
                               select m).FirstOrDefault();

                if(mapping != null)
                {
                    request.Properties.Add("Mapping", mapping);
                    request.Properties.Add("Endpoint", ep);
                    return base.SendAsync(request, cancellationToken);
                }
            }
            else
            {
                var option = _configRepo.All().Single();
                if(option.DescribeAtRoot)
                {
                    routeData.Values["action"] = "Describe";
                    return base.SendAsync(request, cancellationToken);
                }
            }
            
            return Task.FromResult(request.FailureResponse(HttpStatusCode.NotFound, "No mapping has been specified at this endpoint."));
        }
    }
}