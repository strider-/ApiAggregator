
namespace ApiAggregator.Core.Data
{
    public interface IDataManager
    {
        IConnectionBuilder GetConnectionBuilder();
        IDatabaseDeployer GetDatabaseDeployer();
    }
}
