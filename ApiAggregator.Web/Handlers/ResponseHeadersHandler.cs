using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ApiAggregator.Web.Handlers
{
    public class ResponseHeadersHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return base.SendAsync(request, cancellationToken).ContinueWith(task =>
            {
                var response = task.Result;
                response.Content.Headers.Add("X-Men", "Welcome To Die");
                return response;
            });
        }
    }
}