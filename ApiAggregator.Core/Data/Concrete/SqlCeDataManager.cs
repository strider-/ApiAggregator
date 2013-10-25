using ApiAggregator.Core.Services;

namespace ApiAggregator.Core.Data.Concrete
{
    public class SqlCeDataManager : IDataManager
    {
        private string _connString;
        private SqlCeConnectionBuilder _builder;
        private readonly IHashingService _hasher;

        public SqlCeDataManager(IConfigurationProvider config, IHashingService hasher)
        {
            _connString = config.GetSetting("ConnectionString.SqlCe");
            _builder = new SqlCeConnectionBuilder(_connString);
            _hasher = hasher;
        }

        public IConnectionBuilder GetConnectionBuilder()
        {
            return _builder;
        }

        public IDatabaseDeployer GetDatabaseDeployer()
        {
            return new SqlCeDatabaseDeployer(_builder, _hasher);
        }
    }
}
