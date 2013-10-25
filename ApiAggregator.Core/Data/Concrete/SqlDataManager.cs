using ApiAggregator.Core.Services;

namespace ApiAggregator.Core.Data.Concrete
{
    public class SqlDataManager : IDataManager
    {
        private string _connString;
        private SqlConnectionBuilder _builder;
        private readonly IHashingService _hasher;

        public SqlDataManager(IConfigurationProvider config, IHashingService hasher)
        {
            _connString = config.GetSetting("ConnectionString.SqlExpress");
            _builder = new SqlConnectionBuilder(_connString);
            _hasher = hasher;
        }

        public IConnectionBuilder GetConnectionBuilder()
        {
            return _builder;
        }

        public IDatabaseDeployer GetDatabaseDeployer()
        {
            return new SqlDatabaseDeployer(_builder, _hasher);
        }
    }
}
