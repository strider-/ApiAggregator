using System.Collections.Generic;
using ApiAggregator.Core.Models;

namespace ApiAggregator.Core.Data
{
    public interface IApiMappingRepository : IRepository<ApiMapping>
    {
        IEnumerable<ApiMapping> EnabledMappings();

        IDictionary<int, string> AvailableServices();

        IDictionary<int, string> AvailableMappings();

        IEnumerable<RecentItem> RecentlyAdded(int count);
    }
}
