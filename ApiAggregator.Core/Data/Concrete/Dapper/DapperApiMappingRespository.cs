using System.Collections.Generic;
using System.Linq;
using ApiAggregator.Core.Models;
using Dapper;

namespace ApiAggregator.Core.Data.Concrete.Dapper
{
    public class DapperApiMappingRepository : DapperRepository<ApiMapping>, IApiMappingRepository
    {
        public DapperApiMappingRepository(IConnectionBuilder connectionBuilder) : base(connectionBuilder) { }

        public override IEnumerable<ApiMapping> All()
        {
            var sql = "SELECT * FROM [ApiMapping] a JOIN [Service] s ON a.ServiceId = s.Id";
            return ConnectionContext(c => c.Query<ApiMapping, Service, ApiMapping>(sql, (a, s) =>
            {
                a.Service = s;
                return a;
            }));
        }

        public IEnumerable<ApiMapping> EnabledMappings()
        {
            var sql = "SELECT * FROM [ApiMapping] a JOIN [Service] s ON a.ServiceId = s.Id WHERE a.Enabled = 1 AND s.Enabled = 1";
            return ConnectionContext(c => c.Query<ApiMapping, Service, ApiMapping>(sql, (a, s) =>
            {
                a.Service = s;
                return a;
            }));
        }

        public override ApiMapping FindById(int id)
        {
            var sql = "SELECT * FROM [ApiMapping] a JOIN [Service] s ON a.ServiceId = s.Id WHERE a.Id = @Id";
            return ConnectionContext(c => c.Query<ApiMapping, Service, ApiMapping>(sql, (a, s) =>
            {
                a.Service = s;
                return a;
            }, new { id })).FirstOrDefault();
        }

        public override void Add(ApiMapping item)
        {
            var sql = "INSERT INTO [ApiMapping] (Endpoint, Api, Name, Method, ServiceId, Enabled) VALUES (@Endpoint, @Api, @Name, @Method, @ServiceId, @Enabled)";
            ConnectionContext(c => {                
                c.Execute(sql, item);
                item.Id = GetLastIdentity(c);

                return item.Id;
            });
        }

        public override void Delete(ApiMapping item)
        {
            var sql = "DELETE FROM [ApiMapping] WHERE Id = @Id";
            ConnectionContext(c => c.Execute(sql, new { item.Id }));
        }

        public override void Update(ApiMapping item)
        {
            var sql = "UPDATE [ApiMapping] SET Endpoint = @Endpoint, Api = @Api, Name = @Name, Method = @Method, ServiceId = @ServiceId, Enabled = @Enabled WHERE Id = @Id";
            ConnectionContext(c => c.Execute(sql, item));
        }

        public IDictionary<int, string> AvailableServices()
        {
            var sql = "SELECT Id, Name FROM [Service] ORDER BY Name";
            return ConnectionContext(c => c.Query(sql)).ToDictionary(k => (int)k.Id, v => (string)v.Name);
        }
        
        public IDictionary<int, string> AvailableMappings()
        {
            var sql = "SELECT Id, Name FROM [ApiMapping] ORDER BY Name";
            return ConnectionContext(c => c.Query(sql)).ToDictionary(k => (int)k.Id, v => (string)v.Name);
        }

        public IEnumerable<RecentItem> RecentlyAdded(int count = 10)
        {
            var recentServiceSql = "SELECT Id, Name, 'Service' AS Type, Enabled, Created FROM Service ORDER BY Created DESC";
            var recentMappingSql = "SELECT m.Id, s.Name + ' - ' + m.Name AS Name, 'Mapping' AS Type, m.Enabled, m.Created FROM ApiMapping AS m INNER JOIN Service AS s ON m.ServiceId = s.Id ORDER BY Created DESC";
            return ConnectionContext(c =>
            {
                var services = c.Query<RecentItem>(recentServiceSql);
                var mappings = c.Query<RecentItem>(recentMappingSql);
                return services.Union(mappings).OrderByDescending(r => r.Created).Take(count);
            });
        }
    }
}