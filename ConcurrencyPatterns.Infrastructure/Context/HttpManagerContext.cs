using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ConcurrencyPatterns.Infrastructure.Db;
using ConcurrencyPatterns.Infrastructure.Map;
using ConcurrencyPatterns.Infrastructure.Session;
using ConcurrencyPatterns.Infrastructure.UnitOfWork;

namespace ConcurrencyPatterns.Infrastructure.Context
{
	public sealed class HttpManagerContext : IManagerContext
	{
		private static readonly string DATA = "data";
		private static readonly string SESSION = "session";
		private static readonly string MAP = "map";
		private static readonly string UOW = "uow";

		public HttpManagerContext(IDataConnection data, ISession session, IIdentityMap map, IUnitOfWork uow)
		{
			Store(DATA, data);
			Store(SESSION, session);
			Store(MAP, map);
			Store(UOW, uow);
		}

		public IDataConnection Data
		{
			get { return Retrieve<IDataConnection>(DATA); }
		}

		public ISession Session
		{
			get { return Retrieve<ISession>(SESSION); }
		}

		public IIdentityMap IdentityMap
		{
			get { return Retrieve<IIdentityMap>(MAP); }
		}

		public IUnitOfWork UnitOfWork
		{
			get { return Retrieve<IUnitOfWork>(UOW); }
		}

		private void Store(string key, object value)
		{
			System.Web.HttpContext.Current.Items.Add(key, value);
		}

		private T Retrieve<T>(string key)
		{
			return (T)System.Web.HttpContext.Current.Items[key];
		}
	}
}
