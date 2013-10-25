using System;
using System.Collections.Generic;
using System.Linq;
using ApiAggregator.Core.Models;
using Dapper;

namespace ApiAggregator.Core.Data.Concrete.Dapper
{
    public class DapperConfigurationRepository : DapperRepository<Configuration>, IConfigurationRepository
    {
        public DapperConfigurationRepository(IConnectionBuilder connBuilder) : base(connBuilder) { }

        public override IEnumerable<Configuration> All()
        {
            var sql = "SELECT * FROM [Configuration]";
            return ConnectionContext(c => c.Query<Configuration>(sql));
        }

        public override Configuration FindById(int id)
        {
            var sql = "SELECT * FROM [Configuration] WHERE Id = @Id";
            return ConnectionContext(c => c.Query<Configuration>(sql, new { id }).FirstOrDefault());
        }

        public override void Add(Configuration item)
        {
            throw new NotImplementedException("Refusing to add new security items.");
        }

        public override void Delete(Configuration item)
        {
            throw new NotImplementedException("Refusing to delete security items.");
        }

        public override void Update(Configuration item)
        {
            var sql = @"UPDATE [Configuration] SET 
                            Apikey = @Apikey, 
                            SecurityOption = @SecurityOption, 
                            Username = @Username, 
                            Password = @Password, 
                            RequireLogin = @RequireLogin,
                            RequireAuthenticator = @RequireAuthenticator,
                            AuthenticatorSecret = @AuthenticatorSecret,
                            DescribeAtRoot = @DescribeAtRoot 
                        WHERE Id = @Id";
            ConnectionContext(c => c.Execute(sql, item));
        }
    }
}