using System.Collections.Generic;
using ApiAggregator.Core.Models;

namespace ApiAggregator.Core.Services
{
    public interface IMatchingService
    {
        int GetMappingWeight(ApiMapping mapping, string url);
        string[] GetVariableNames(ApiMapping mapping);
        string GetInterpolatedApiPath(ApiMapping mapping, string url);
        string GetInterpolatedEndpoint(ApiMapping mapping, IDictionary<string, string> variables);
    }
}
