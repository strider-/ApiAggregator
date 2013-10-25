using System.Data.Common;
using System.Data.SqlServerCe;

namespace ApiAggregator.Core.Data.Concrete
{
    public class SqlCeConnectionBuilder : IConnectionBuilder
    {
        private string _connString;

        public SqlCeConnectionBuilder(string connString)
        {
            _connString = connString;
        }

        public DbConnection FetchConnection()
        {
            return new SqlCeConnection(_connString);
        }

        public string ConnectionString { get { return _connString; } }
    }
}