using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ninject;
using Ninject.Web.Common;

namespace ConcurrencyPatterns.Presentation.Web.Infrastructure
{
	using ConcurrencyPatterns.Infrastructure.Context;
	using ConcurrencyPatterns.Infrastructure.Db;
	using ConcurrencyPatterns.Infrastructure.Map;
	using ConcurrencyPatterns.Infrastructure.Session;
	using ConcurrencyPatterns.Infrastructure.UnitOfWork;
	using ConcurrencyPatterns.Model.Core;
	using ConcurrencyPatterns.Model.Products;
	using ConcurrencyPatterns.Model.Users;
	using ConcurrencyPatterns.Repository.Sql;
	using ConcurrencyPatterns.Repository.Sql.Repositories;
	using ConcurrencyPatterns.Repository.Sql.UnitOfWork;
	using ConcurrencyPatterns.Repository.Sql.Version;

	public class NinjectDependencyResolver : IDependencyResolver
	{
		private IKernel kernel;

		public NinjectDependencyResolver(IKernel kernel)
		{
			this.kernel = kernel;
			AddBindings();
		}

		public object GetService(Type serviceType)
		{
			return kernel.TryGet(serviceType);
		}

		public IEnumerable<object> GetServices(Type serviceType)
		{
			return kernel.GetAll(serviceType);
		}

		private void AddBindings()
		{
			kernel.Bind<IIdentityMap>().To<IdentityMap>().InRequestScope();
			kernel.Bind<ISession>().To<CookieSession>().InRequestScope();
			kernel.Bind<IDataConnection>().To<DataConnection>().InRequestScope().WithConstructorArgument("connectionString", DataConnectionInfo.SQL_CONNECTION);
			kernel.Bind<IUserRepository>().To<UserRepository>().InRequestScope();
			kernel.Bind<IProductRepository>().To<ProductRepository>().InRequestScope();
			kernel.Bind<IManagerContext>().To<HttpManagerContext>().InRequestScope();
			kernel.Bind<IVersionStorage>().To<VersionStorage>().InRequestScope();
			kernel.Bind<IUnitOfWork>().To<UnitOfWork>().InRequestScope();
		}
	}
}