using System.Data.SqlServerCe;
using ApiAggregator.Core.Services;

namespace ApiAggregator.Core.Data.Concrete
{
    public class SqlCeDatabaseDeployer : BaseSqlDatabaseDeployer
    {
        private SqlCeEngine _engine;

        public SqlCeDatabaseDeployer(IConnectionBuilder connBuilder, IHashingService hasher) : base(connBuilder, hasher)
        {
            _engine = new SqlCeEngine(_connBuilder.ConnectionString);
        }

        protected override void CreateDatabase()
        {
            _engine.CreateDatabase();
        }

        public override bool DatabaseExists()
        {
            return _engine.Verify();
        }
    }
}
