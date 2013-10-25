[assembly: WebActivator.PreApplicationStartMethod(typeof(ApiAggregator.Web.NinjectWebCommon), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(ApiAggregator.Web.NinjectWebCommon), "Stop")]

namespace ApiAggregator.Web
{
    using System;
    using System.Web;
    using System.Web.Http;
    using System.Web.Mvc;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using ApiAggregator.Web.Controllers;
    using ApiAggregator.Core.Data;
    using ApiAggregator.Core.Data.Concrete;
    using ApiAggregator.Core.Services;
    using ApiAggregator.Core.Services.Concrete;
    using ApiAggregator.Web.Framework;
    using ApiAggregator.Web.Handlers;
    using Ninject;
    using Ninject.Web.Common;
    using ApiAggregator.Core.Data.Concrete.Dapper;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
            
            RegisterServices(kernel);
            return kernel;
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            // Configuration
            kernel.Bind<IConfigurationProvider>().To<ConfigFileConfigurationProvider>();            

            // Data            
            kernel.Bind<IDataManager>().To<SqlCeDataManager>();
            kernel.Bind<IConnectionBuilder>().ToMethod(ctx => kernel.Get<IDataManager>().GetConnectionBuilder());
            kernel.Bind<IDatabaseDeployer>().ToMethod(ctx => kernel.Get<IDataManager>().GetDatabaseDeployer());
            kernel.Bind<IApiMappingRepository>().To<DapperApiMappingRepository>().InRequestScope();
            kernel.Bind<IServiceRepository>().To<DapperServiceRepository>().InRequestScope();
            kernel.Bind<IConfigurationRepository>().To<DapperConfigurationRepository>().InRequestScope();

            // Services
            kernel.Bind<IHashingService>().To<BCryptHashingService>();
            kernel.Bind<IAuthenticationService>().To<AuthenticationService>();
            kernel.Bind<IMatchingService>().To<MatchingService>();
            kernel.Bind<IContextGenerator>().To<ContextGenerator>();

            // Framework Extensions
            kernel.Bind<ApiMappingHandler>().ToSelf();
            kernel.Bind<SecurityHandler>().ToSelf();
            kernel.Bind<ResponseHeadersHandler>().ToSelf();
            kernel.Bind<IControllerFactory>().ToMethod<NotFoundControllerFactory>(ctx =>
            {
                var factory = new NotFoundControllerFactory();
                factory.NotFoundAction<HomeController>(c => c.NotFound());
                return factory;
            });

            GlobalConfiguration.Configuration.DependencyResolver = new NinjectResolver(kernel);
        }
    }
}
