using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrencyPatterns.Infrastructure.Db
{
	public interface IDataConnection
	{
		IDbConnection Connection { get; }
		IDbTransaction Transaction { get; set; }
	}
}
