using System.Data.Common;

namespace ApiAggregator.Core.Data
{
    public interface IConnectionBuilder
    {
        DbConnection FetchConnection();
        string ConnectionString { get; }
    }
}
