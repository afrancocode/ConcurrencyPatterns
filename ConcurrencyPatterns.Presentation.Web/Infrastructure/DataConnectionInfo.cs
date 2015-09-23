using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;

using ConcurrencyPatterns.Infrastructure.Db;

namespace ConcurrencyPatterns.Presentation.Web.Infrastructure
{
	internal static class DataConnectionInfo
	{
		public static readonly string SQL_CONNECTION = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=D:\GitHub\ConcurrencyPatterns\ConcurrencyPatterns.Presentation.Web\App_Data\ConcurrencyData.mdf;Integrated Security=True";
	}
}