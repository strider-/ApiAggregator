using System;
using System.Data.SqlClient;
using System.Linq;
using ApiAggregator.Core.Services;
using Dapper;

namespace ApiAggregator.Core.Data.Concrete
{
    public class SqlDatabaseDeployer : BaseSqlDatabaseDeployer
    {
        private string _masterConnString;
        private string _dbName;

        public SqlDatabaseDeployer(IConnectionBuilder connBuilder, IHashingService hasher) : base(connBuilder, hasher)
        {
            var connStrBuilder = new SqlConnectionStringBuilder(_connBuilder.ConnectionString);
            if(string.IsNullOrWhiteSpace(connStrBuilder.InitialCatalog))
            {
                throw new ArgumentException("The connection string is missing the Initial Catalog value.");
            }
            _dbName = connStrBuilder.InitialCatalog;
            connStrBuilder.InitialCatalog = "master";
            _masterConnString = connStrBuilder.ToString();
        }

        protected override void CreateDatabase()
        {
            using(var conn = new SqlConnection(_masterConnString))
            {
                conn.Execute(string.Format("CREATE DATABASE [{0}]", _dbName));
            }
        }

        public override bool DatabaseExists()
        {
            using(var conn = new SqlConnection(_masterConnString))
            {                
                conn.Open();
                var existsSql = "SELECT count(name) FROM sys.databases WHERE name = @Name";
                var count = conn.Query<int>(existsSql, new { name = _dbName }).First();
                return count == 1;
            }
        }
    }
}
