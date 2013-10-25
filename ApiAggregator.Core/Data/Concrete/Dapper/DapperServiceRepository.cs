using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using ApiAggregator.Core.Models;
using Dapper;

namespace ApiAggregator.Core.Data.Concrete.Dapper
{
    public class DapperServiceRepository : DapperRepository<Service>, IServiceRepository
    {
        public DapperServiceRepository(IConnectionBuilder connectionBuilder) : base(connectionBuilder) { }

        public override IEnumerable<Service> All()
        {
            var sql = "SELECT * FROM [Service]";
            return ConnectionContext(c => c.Query<Service>(sql));
        }

        public override Service FindById(int id)
        {
            return ConnectionContext(c =>
            {
                var sql = "SELECT * FROM [Service] WHERE Id = @Id";
                var service = c.Query<Service>(sql, new { id }).FirstOrDefault();

                if(service != null)
                {
                    sql = "SELECT * FROM [ServiceHeaders] WHERE ServiceId = @Id";
                    service.Headers = c.Query<ServiceHeader>(sql, new { id }).ToList();
                    
                    sql = "SELECT * FROM [ServiceQueryStrings] WHERE ServiceId = @Id";
                    service.QueryStringAppends = c.Query<ServiceQueryString>(sql, new { id }).ToList();
                }
                return service;
            });
        }

        public override void Add(Service item)
        {
            ConnectionContext(c =>
            {
                using(var trans = c.BeginTransaction())
                {
                    try
                    {
                        var sql = "INSERT INTO [Service] (Name, RootUrl, Enabled) VALUES (@Name, @RootUrl, @Enabled)";
                        c.Execute(sql, item, trans);

                        item.Id = GetLastIdentity(c, trans);
                        UpdateHeaders(c, item, trans);
                        UpdateQueryStrings(c, item, trans);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
                return item.Id;
            });
        }

        public override void Delete(Service item)
        {
            var sql = "DELETE FROM [Service] WHERE Id = @Id";
            ConnectionContext(c => c.Execute(sql, new { item.Id }));
        }

        public override void Update(Service item)
        {
            ConnectionContext(c =>
            {
                using(var trans = c.BeginTransaction())
                {
                    try
                    {
                        var sql = "UPDATE [Service] SET Name = @Name, RootUrl = @RootUrl, Enabled = @Enabled WHERE Id = @Id";
                        c.Execute(sql, item, trans);

                        UpdateHeaders(c, item, trans);
                        UpdateQueryStrings(c, item, trans);
                        
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }

                return 1;
            });
        }

        private void UpdateHeaders(DbConnection conn, Service item, DbTransaction trans)
        {
            var delHeaderSql = "DELETE FROM [ServiceHeaders] WHERE ServiceId = @ServiceId AND Id NOT IN @Ids";
            var insHeaderSql = "INSERT INTO [ServiceHeaders] (ServiceId, Header, Value) VALUES (@ServiceId, @Header, @Value)";
            var updHeaderSql = "UPDATE [ServiceHeaders] SET Header = @Header, Value = @Value WHERE Id = @Id";

            conn.Execute(delHeaderSql, new { ServiceId = item.Id, Ids = item.Headers.Where(h => h.Id > 0).Select(h => h.Id) }, trans);

            foreach(var header in item.Headers)
            {
                if(header.Id == 0)
                {
                    conn.Execute(insHeaderSql, new { ServiceId = item.Id, header.Header, header.Value }, trans);
                }
                else
                {
                    conn.Execute(updHeaderSql, header, trans);
                }
            }
        }

        private void UpdateQueryStrings(DbConnection conn, Service item, DbTransaction trans)
        {
            // Query Strings
            var delQSsql = "DELETE FROM [ServiceQueryStrings] WHERE ServiceId = @ServiceId AND Id NOT IN @Ids";
            var insQSSql = "INSERT INTO [ServiceQueryStrings] (ServiceId, Name, Value) VALUES (@ServiceId, @Name, @Value)";
            var updQSSql = "UPDATE [ServiceQueryStrings] SET Name = @Name, Value = @Value WHERE Id = @Id";

            conn.Execute(delQSsql, new { ServiceId = item.Id, Ids = item.QueryStringAppends.Where(q => q.Id > 0).Select(q => q.Id).ToArray() }, trans);

            foreach(var qs in item.QueryStringAppends)
            {
                if(qs.Id == 0)
                {
                    conn.Execute(insQSSql, new { ServiceId = item.Id, qs.Name, qs.Value }, trans);
                }
                else
                {
                    conn.Execute(updQSSql, qs, trans);
                }
            }
        }

        public IEnumerable<ServiceHeader> GetHeaders(Service service)
        {
            var sql = "SELECT * FROM [ServiceHeaders] sh JOIN [Service] s ON s.Id = sh.ServiceId WHERE ServiceId = @Id";
            return ConnectionContext(c => c.Query<ServiceHeader, Service, ServiceHeader>(sql, (sh, s) => {
                sh.Service = s;
                return sh;
            }, new { service.Id }));
        }

        public IEnumerable<ServiceQueryString> GetQueryStringAppends(Service service)
        {
            var sql = "SELECT * FROM [ServiceQueryStrings] sq JOIN [Service] s ON s.Id = sq.ServiceId WHERE ServiceId = @Id";
            return ConnectionContext(c => c.Query<ServiceQueryString, Service, ServiceQueryString>(sql, (sq, s) =>
            {
                sq.Service = s;
                return sq;
            }, new { service.Id }));
        }
    }
}