using System;
using System.Linq;
using System.Web;
using ApiAggregator.Core.Data;
using ApiAggregator.Core.Models;
using ApiAggregator.Core.Services.Models;

namespace ApiAggregator.Core.Services.Concrete
{
    public class ContextGenerator : IContextGenerator
    {
        private readonly IServiceRepository _repo;
        private readonly IMatchingService _service;

        public ContextGenerator(IServiceRepository serviceRepo, IMatchingService matchService)
        {
            _repo = serviceRepo;
            _service = matchService;

            if(serviceRepo == null)
            {
                throw new ArgumentNullException("serviceRepo");
            }

            if(matchService == null)
            {
                throw new ArgumentNullException("matchService");
            }
        }

        public MappingContext GetMappingContext(ApiMapping mapping, string requestedEndpoint)
        {            
            if(mapping == null)
            {
                throw new ArgumentNullException("mapping");
            }

            if(mapping.Service == null)
            {
                throw new ArgumentException("ApiMapping service cannot be null.", "mapping");
            }

            if(string.IsNullOrWhiteSpace(requestedEndpoint))
            {
                throw new ArgumentNullException("requestedEndpoint");
            }
            
            var context = new MappingContext(mapping);
            context.ServiceHeaders = _repo.GetHeaders(mapping.Service);
            context.ServiceQueryStringAppends = _repo.GetQueryStringAppends(mapping.Service);
            context.RequestedEndpoint = requestedEndpoint;

            var pathAndQuery = _service.GetInterpolatedApiPath(mapping, requestedEndpoint).Split('?');
            var queryString = context.ServiceQueryStringAppends.Select(x => string.Format("{0}={1}", x.Name, HttpUtility.UrlEncode(x.Value))).ToList();
            
            var builder = new UriBuilder(new Uri(mapping.Service.RootUrl));
            builder.Path = string.Format("{0}/{1}", VirtualPathUtility.RemoveTrailingSlash(builder.Path).TrimStart('/'), pathAndQuery.First().TrimStart('/'));

            if(pathAndQuery.Length > 1)
            {
                queryString.AddRange(pathAndQuery.Last().Split('&'));
            }
            builder.Query = string.Join("&", queryString);
            context.InterpolatedApiUrl = builder.Uri.AbsoluteUri;

            return context;
        }
    }
}
