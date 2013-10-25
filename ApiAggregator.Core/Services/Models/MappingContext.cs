using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ApiAggregator.Core.Models;

namespace ApiAggregator.Core.Services.Models
{
    public class MappingContext
    {
        internal MappingContext(ApiMapping mapping)
        {
            Name = mapping.Name;
            Service = mapping.Service;
            MappingEndpoint = mapping.Endpoint;
            ApiUrl = mapping.Api;
            Method = new HttpMethod(mapping.Method);
        }

        public Task<HttpResponseMessage> Execute()
        {
            var client = new HttpClient();
            foreach(var h in ServiceHeaders)
            {
                client.DefaultRequestHeaders.Add(h.Header, h.Value);
            }

            var request = new HttpRequestMessage(Method, InterpolatedApiUrl);
            return client.SendAsync(request);
        }

        public string Name { get; private set; }

        public Service Service { get; private set; }

        public string MappingEndpoint { get; private set; }

        public string ApiUrl { get; private set; }

        public string RequestedEndpoint { get; internal set; }

        public string InterpolatedApiUrl { get; internal set; }

        public HttpMethod Method { get; internal set; }

        public IEnumerable<ServiceHeader> ServiceHeaders { get; internal set; }

        public IEnumerable<ServiceQueryString> ServiceQueryStringAppends { get; internal set; }
    }
}