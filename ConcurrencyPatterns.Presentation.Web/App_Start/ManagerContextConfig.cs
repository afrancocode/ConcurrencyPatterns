using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace ConcurrencyPatterns.Presentation.Web.App_Start
{
	public class ManagerContextConfig
	{
		public static void Initialize()
		{
			ConcurrencyPatterns.Infrastructure.Context.ApplicationContextHolder.Instance.Initialize(ResolveContext, Resolve);
		}

		private static ConcurrencyPatterns.Infrastructure.Context.IManagerContext ResolveContext()
		{
			var resolver = System.Web.Mvc.DependencyResolver.Current;
			return (ConcurrencyPatterns.Infrastructure.Context.IManagerContext)resolver.GetService(typeof(ConcurrencyPatterns.Infrastructure.Context.IManagerContext));
		}

		private static object Resolve(Type service)
		{
			var resolver = System.Web.Mvc.DependencyResolver.Current;
			return resolver.GetService(service);
		}
	}
}