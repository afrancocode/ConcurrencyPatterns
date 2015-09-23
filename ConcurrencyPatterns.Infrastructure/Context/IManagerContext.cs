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
	public interface IManagerContext
	{
		IDataConnection Data { get; }
		ISession Session { get; }
		IIdentityMap IdentityMap { get; }
		IUnitOfWork UnitOfWork { get; }
	}
}
