using ApiAggregator.Core.Models;
using ApiAggregator.Core.Services.Models;

namespace ApiAggregator.Core.Services
{
    public interface IContextGenerator
    {
        MappingContext GetMappingContext(ApiMapping mapping, string requestedEndpoint);
    }
}
