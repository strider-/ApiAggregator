using System;
using ApiAggregator.Core.Models;
using ApiAggregator.Core.Services;
using Dapper;

namespace ApiAggregator.Core.Data.Concrete
{
    public abstract class BaseSqlDatabaseDeployer : IDatabaseDeployer
    {
        protected readonly IConnectionBuilder _connBuilder;
        protected readonly IHashingService _hasher;

        public BaseSqlDatabaseDeployer(IConnectionBuilder connBuilder, IHashingService hasher)
        {
            _connBuilder = connBuilder;
            _hasher = hasher;
        }

        public abstract bool DatabaseExists();

        protected abstract void CreateDatabase();

        public virtual void DeploySchema()
        {
            if(!DatabaseExists())
            {
                CreateDatabase();
                using(var conn = _connBuilder.FetchConnection())
                {
                    conn.Open();
                    using(var trans = conn.BeginTransaction())
                    {
                        try
                        {
                            conn.Execute(@"
                             CREATE TABLE Service (
                                [Id]      int            IDENTITY(1, 1) CONSTRAINT PK_Service PRIMARY KEY, 
                                [Name]    nvarchar(255)  NOT NULL, 
                                [RootUrl] nvarchar(1024) NOT NULL, 
                                [Enabled] bit            DEFAULT 1 NOT NULL,
                                [Created] datetime       DEFAULT GetDate() NOT NULL
                             )", transaction: trans);

                            conn.Execute(@"
                             CREATE TABLE ApiMapping (
                                [Id]        int            IDENTITY(1, 1) CONSTRAINT PK_ApiMapping PRIMARY KEY, 
                                [Endpoint]  nvarchar(255)  NOT NULL UNIQUE, 
                                [Api]       nvarchar(4000) NOT NULL, 
                                [ServiceId] int            NOT NULL REFERENCES Service(Id) ON DELETE CASCADE ON UPDATE CASCADE, 
                                [Method]    nvarchar(20)   NOT NULL, 
                                [Name]      nvarchar(100)  NOT NULL, 
                                [Enabled]   bit            DEFAULT 1 NOT NULL,
                                [Created]   datetime       DEFAULT GetDate() NOT NULL
                            )", transaction: trans);

                            conn.Execute(@"
                             CREATE TABLE ServiceHeaders (
                                [Id]        int            IDENTITY(1, 1) CONSTRAINT PK_ServiceHeaders PRIMARY KEY, 
                                [ServiceId] int            NOT NULL REFERENCES Service(Id) ON DELETE CASCADE ON UPDATE CASCADE, 
                                [Header]    nvarchar(100)  NOT NULL, 
                                [Value]     nvarchar(1024) NOT NULL
                            )", transaction: trans);

                            conn.Execute(@"
                             CREATE TABLE ServiceQueryStrings (
                                [Id]        int            IDENTITY(1, 1) CONSTRAINT PK_ServiceQueryStrings PRIMARY KEY, 
                                [ServiceId] int            NOT NULL REFERENCES Service(Id) ON DELETE CASCADE ON UPDATE CASCADE, 
                                [Name]      nvarchar(100)  NOT NULL, 
                                [Value]     nvarchar(1024) NOT NULL
                            )", transaction: trans);

                            conn.Execute(@"
                             CREATE TABLE Configuration (
                                [Id]                   int            IDENTITY(1, 1) CONSTRAINT PK_Configuration PRIMARY KEY,
                                [Apikey]               nvarchar(32)   DEFAULT '' NOT NULL,
                                [SecurityOption]       int            DEFAULT 1 NOT NULL,
                                [Username]             nvarchar(50),
                                [Password]             nvarchar(100),
                                [RequireLogin]         bit            DEFAULT 1 NOT NULL,
                                [RequireAuthenticator] bit            DEFAULT 0 NOT NULL,
                                [AuthenticatorSecret]  nvarchar(32)   DEFAULT '' NOT NULL,
                                [DescribeAtRoot]       bit            DEFAULT 0 NOT NULL
                            )", transaction: trans);

                            trans.Commit();
                        }
                        catch(Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                        finally
                        {
                            conn.Close();
                        }
                    }
                }
            }
        }

        public virtual void Seed()
        {
            using(var conn = _connBuilder.FetchConnection())
            {
                var freshKey = _hasher.GenerateApiKey();
                var qrSecret = _hasher.GenerateQRSecret();

                var sql = "INSERT INTO [Configuration] (Apikey, AuthenticatorSecret) VALUES (@freshKey, @qrSecret)";
                conn.Execute(sql, new { freshKey, qrSecret });
            }
        }

    }
}