using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dependencies;
using ApiAggregator.Core.Data;
using ApiAggregator.Web.Extensions;

namespace ApiAggregator.Web
{
    public static class Datastore
    {
        public static void Initialize(HttpConfiguration config)
        {
            var deployer = config.DependencyResolver.GetService<IDatabaseDeployer>();

            if(!deployer.DatabaseExists())
            {
                deployer.DeploySchema();
                deployer.Seed();
            }
        }
    }
}