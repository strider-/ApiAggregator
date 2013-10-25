using System.Data.Common;
using System.Data.SqlClient;

namespace ApiAggregator.Core.Data.Concrete
{
    public class SqlConnectionBuilder : IConnectionBuilder
    {
        private string _connString;

        public SqlConnectionBuilder(string connString)
        {
            _connString = connString;
        }

        public DbConnection FetchConnection()
        {
            return new SqlConnection(_connString);
        }

        public string ConnectionString { get { return _connString; } }
    }
}
