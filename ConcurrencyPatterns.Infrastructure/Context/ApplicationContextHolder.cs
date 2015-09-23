using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConcurrencyPatterns.Infrastructure.Session;

namespace ConcurrencyPatterns.Infrastructure.Context
{
	public sealed class ApplicationContextHolder
	{
		private static readonly ApplicationContextHolder instance = new ApplicationContextHolder();

		public static ApplicationContextHolder Instance { get { return instance; } }

		private Func<IManagerContext> contextResolver;
		private Func<Type, object> serviceResolver;

		private ApplicationContextHolder() { }

		private bool IsInitialized() { return this.contextResolver != null; }

		public IManagerContext Context
		{
			get
			{
				if (!IsInitialized())
					throw new InvalidOperationException("Application not initialized!");
				return this.contextResolver();
			}
		}

		public void Initialize(Func<IManagerContext> contextResolver, Func<Type, object> serviceResolver)
		{
			if (IsInitialized())
				throw new InvalidOperationException("Application already initialized!");
			this.contextResolver = contextResolver;
			this.serviceResolver = serviceResolver;
		}

		public T GetService<T>(Type type)
		{
			if (!IsInitialized())
					throw new InvalidOperationException("Application not initialized!");
			return (T)this.serviceResolver(type);
		}
	}
}
