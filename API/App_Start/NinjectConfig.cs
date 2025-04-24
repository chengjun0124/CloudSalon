using System;
using System.Web;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Ninject;
using System.Web.Http.Dependencies;
using Ninject.Syntax;
using Ninject.Activation;
using Ninject.Parameters;
using System.Collections.Generic;
using CloudSalon.DAL;

namespace CloudSalon.API
{
    public class NinjectDependencyResolverForWebApi : NinjectDependencyScope, IDependencyResolver
    {
        private IKernel kernel;
        public NinjectDependencyResolverForWebApi(IKernel kernel)
            : base(kernel)
        {
            if (kernel == null)
            {
                throw new ArgumentNullException("kernel");
            }
            this.kernel = kernel;
        }
        public IDependencyScope BeginScope()
        {
            return new NinjectDependencyScope(kernel);
        }
    }

    public class NinjectDependencyScope : IDependencyScope
    {
        private IResolutionRoot resolver;
        internal NinjectDependencyScope(IResolutionRoot resolver)
        {
            System.Diagnostics.Contracts.Contract.Assert(resolver != null);
            this.resolver = resolver;
        }
        public void Dispose()
        {
            resolver = null;
        }
        public object GetService(Type serviceType)
        {
            return resolver.TryGet(serviceType);
        }
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return resolver.GetAll(serviceType);
        }
    }

    public class NinjectRegister
    {
        public static readonly IKernel Kernel;
        static NinjectRegister()
        {
            Kernel = new StandardKernel();
            AddBindings();
        }
        private static void AddBindings()
        {

            Kernel.Bind<SalonContext>().To<SalonContext>().InScope(cx => HttpContext.Current);

            Kernel.Bind<EmployeeDAL>().To<EmployeeDAL>().InScope(ctx => HttpContext.Current);
            Kernel.Bind<SalonDAL>().To<SalonDAL>().InScope(ctx => HttpContext.Current);
            Kernel.Bind<ServiceDAL>().To<ServiceDAL>().InScope(ctx => HttpContext.Current);
            Kernel.Bind<ServiceEffectImageDAL>().To<ServiceEffectImageDAL>().InScope(ctx => HttpContext.Current);
            Kernel.Bind<UserDAL>().To<UserDAL>().InScope(ctx => HttpContext.Current);
            Kernel.Bind<PurchasedServiceDAL>().To<PurchasedServiceDAL>().InScope(ctx => HttpContext.Current);
            Kernel.Bind<ServiceSnapShotDAL>().To<ServiceSnapShotDAL>().InScope(ctx => HttpContext.Current);
            Kernel.Bind<ConsumedServiceDAL>().To<ConsumedServiceDAL>().InScope(ctx => HttpContext.Current);
            Kernel.Bind<ServiceTypeTagDAL>().To<ServiceTypeTagDAL>().InScope(ctx => HttpContext.Current);
            Kernel.Bind<ServiceFunctionalityTagDAL>().To<ServiceFunctionalityTagDAL>().InScope(ctx => HttpContext.Current);
        }

        public static void RegisterFovWebApi(System.Web.Http.HttpConfiguration config)
        {
            config.DependencyResolver = new NinjectDependencyResolverForWebApi(Kernel);
        }
    }
}
