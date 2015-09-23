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
	public sealed class DataConnection : IDataConnection
	{
		private static readonly string SQL_CONNECTION = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=D:\GitHub\ConcurrencyPatterns\ConcurrencyPatterns.Presentation.Web\App_Data\ConcurrencyData.mdf;Integrated Security=True";

		private IDbTransaction transaction;
		private IDbConnection connection;

		public DataConnection() { }

		public IDbConnection Connection
		{
			get 
			{
				if (this.connection == null)
					this.connection = new SqlConnection(SQL_CONNECTION);
				return this.connection;
			}
		}

		public IDbTransaction Transaction
		{
			get { return transaction; }
			set
			{
				if (transaction != null && value != null)
					throw new InvalidOperationException("Transaction has value");
				transaction = value;
			}
		}
	}
}