using System.Collections.Generic;
using ApiAggregator.Core.Models;

namespace ApiAggregator.Core.Data
{
    public interface IServiceRepository : IRepository<Service>
    {
        IEnumerable<ServiceHeader> GetHeaders(Service service);
        
        IEnumerable<ServiceQueryString> GetQueryStringAppends(Service service);
    }
}
