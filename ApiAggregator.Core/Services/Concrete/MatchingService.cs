using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ApiAggregator.Core.Models;

namespace ApiAggregator.Core.Services.Concrete
{
    public class MatchingService : IMatchingService
    {
        private const string RX_VARIABLE = "\\{(.*?)\\}";
        private const string RX_NAMED_REPLACEMENT = "(?<$1>.*)?";

        private RegexOptions _rxOptions = RegexOptions.IgnoreCase;

        public int GetMappingWeight(ApiMapping mapping, string url)
        {
            var rp = GetRoutePattern(mapping);
            var match = Regex.Match(url, rp, _rxOptions);
            return match.Success ? match.Groups.Count : 0;
        }

        public string[] GetVariableNames(ApiMapping mapping)
        {
            return Regex.Matches(mapping.Endpoint, RX_VARIABLE, _rxOptions)
                        .OfType<Match>()
                        .Select(m => m.Groups[1].Value)
                        .ToArray();
        }

        public string GetInterpolatedApiPath(ApiMapping mapping, string url)
        {
            var rp = GetRoutePattern(mapping);
            var rx = new Regex(rp, _rxOptions);
            var m = rx.Match(url);
            var dict = rx.GetGroupNames().Skip(1).ToDictionary(k => k, v => m.Groups[v].Value);

            return Regex.Replace(mapping.Api, RX_VARIABLE, (match) => dict[match.Groups[1].Value], _rxOptions);
        }

        public string GetInterpolatedEndpoint(ApiMapping mapping, IDictionary<string, string> variables)
        {
            return Regex.Replace(mapping.Endpoint, RX_VARIABLE, (match) => variables[match.Groups[1].Value], _rxOptions);            
        }

        private string GetRoutePattern(ApiMapping mapping)
        {
            return string.Format("^{0}$", Regex.Replace(mapping.Endpoint, RX_VARIABLE, RX_NAMED_REPLACEMENT));
        }
    }
}
