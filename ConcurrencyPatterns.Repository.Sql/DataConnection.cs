using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConcurrencyPatterns.Infrastructure.Db;

namespace ConcurrencyPatterns.Repository.Sql
{
	public sealed class DataConnection : IDataConnection
	{
		private IDbTransaction transaction;
		private IDbConnection connection;
		private string connectionString;
		private int openCount;

		public DataConnection(string connectionString)
		{
			this.connectionString = connectionString;
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

		public IDbConnection Open()
		{
			if (++this.openCount != 1) return this.connection;
			this.connection = new SqlConnection(this.connectionString);
			this.connection.Open();
			return connection;
		}

		public void Close()
		{
			if (--this.openCount != 0) return;
			this.connection.Close();
		}
	}
}
