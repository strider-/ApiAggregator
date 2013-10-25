using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Common;
using ApiAggregator.Core.Models;
using Dapper;

namespace ApiAggregator.Core.Data.Concrete.Dapper
{
    public abstract class DapperRepository<T> : IRepository<T> where T : Root, new()
    {
        private readonly IConnectionBuilder _connBuilder;

        public DapperRepository(IConnectionBuilder connectionBuilder)
        {
            _connBuilder = connectionBuilder;
        }

        public abstract IEnumerable<T> All();

        public abstract T FindById(int id);

        public abstract void Add(T item);

        public abstract void Delete(T item);

        public abstract void Update(T item);

        public void SubmitChanges() { }

        public void Dispose() { }

        protected int GetLastIdentity(DbConnection conn, DbTransaction trans = null)
        {
            return (int)conn.Query("SELECT @@IDENTITY As NewId", transaction: trans).Single().NewId;
        }

        protected TQuery ConnectionContext<TQuery>(Func<DbConnection, TQuery> func)
        {
            using(var conn = _connBuilder.FetchConnection())
            {
                conn.Open();
                return func(conn);
            }
        }
    }
}