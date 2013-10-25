using System;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ApiAggregator.Core.Data;
using ApiAggregator.Core.Models;
using ApiAggregator.Web.Extensions;

namespace ApiAggregator.Web.Handlers
{
    public class SecurityHandler : DelegatingHandler
    {
        private readonly IConfigurationRepository _repo;

        public SecurityHandler(IConfigurationRepository repo)
        {
            _repo = repo;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var config = _repo.All().Single();
            Func<string, HttpRequestMessage, bool> authorizedRequest = null;

            switch(config.SecurityOption)
            {
                case SecurityOption.None:
                    authorizedRequest = (k, m) => true;
                    break;
                case SecurityOption.KeyInQueryString:
                    authorizedRequest = ValidKeyInQueryString;
                    break;
                case SecurityOption.KeyInRequestHeader:
                    authorizedRequest = ValidKeyInRequestHeaders;
                    break;
                case SecurityOption.SignedRequest:
                    authorizedRequest = ValidSignedRequest;
                    break;
                default:
                    authorizedRequest = (k, m) => false;
                    break;
            }

            if(authorizedRequest(config.Apikey, request))
            {
                return base.SendAsync(request, cancellationToken);
            }

            return Task.FromResult(request.UnauthorizedResponse());
        }

        private bool ValidKeyInQueryString(string apikey, HttpRequestMessage request)
        {
            return request.GetQueryNameValuePairs().Any(kvp =>
                kvp.Key.Equals("apikey", StringComparison.InvariantCultureIgnoreCase) &&
                kvp.Value.Equals(apikey, StringComparison.InvariantCultureIgnoreCase)
            );
        }

        private bool ValidKeyInRequestHeaders(string apikey, HttpRequestMessage request)
        {
            var authHeader = request.Headers.Authorization;
            if(authHeader == null)
            {
                return false;
            }

            return authHeader.Scheme.Equals("apikey", StringComparison.InvariantCultureIgnoreCase) &&
                   authHeader.Parameter.Equals(apikey, StringComparison.InvariantCultureIgnoreCase);
        }

        private bool ValidSignedRequest(string apikey, HttpRequestMessage request)
        {
            var authHeader = request.Headers.Authorization;
            if(authHeader == null || !authHeader.Scheme.Equals("signature", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            var reqDateValue = request.Headers.FirstOrDefault(k => k.Key.Equals("request-date", StringComparison.InvariantCultureIgnoreCase)).Value;
            if(reqDateValue == null)
            {
                return false;
            }

            DateTimeOffset reqDate;
            if(!DateTimeOffset.TryParse(reqDateValue.First(), out reqDate))
            {
                return false;
            }

            var window = DateTime.UtcNow.Subtract(reqDate.DateTime);
            if(reqDate == null || (window.TotalMinutes > 1 || window.TotalMinutes < -1))
            {
                return false;
            }

            var expectedSig = ComputeSignature(apikey, request.Method, reqDate.DateTime, request.RequestUri.PathAndQuery);

            return expectedSig.Equals(authHeader.Parameter);
        }

        private string ComputeSignature(string apikey, HttpMethod method, DateTime date, string pathAndQuery)
        {
            var toSign = Encoding.UTF8.GetBytes(string.Format("[{0}\n{1}\n{2}]", method.Method, date.ToString("R"), pathAndQuery));
            var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(apikey));
            var sig = hmac.ComputeHash(toSign);

            return Convert.ToBase64String(sig);
        }
    }
}