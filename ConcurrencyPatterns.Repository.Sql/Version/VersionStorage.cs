using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ConcurrencyPatterns.Model.Core;
using ConcurrencyPatterns.Infrastructure.Context;
using ConcurrencyPatterns.Infrastructure.Session;
using ConcurrencyPatterns.Infrastructure.Locking;

namespace ConcurrencyPatterns.Repository.Sql.Version
{
	using Version = ConcurrencyPatterns.Model.Core.Version;
	using System.Data;

	public sealed class VersionStorage : IVersionStorage
	{
		private static readonly string LOAD_SQL = "SELECT Id, Value, ModifiedBy, Modified FROM Version WHERE Id = '{0}'";
		private static readonly string INSERT_SQL = "INSERT INTO Version VALUES ('{0}', {1}, '{2}', '{3}')";
		private static readonly string UPDATE_SQL = "UPDATE Version SET  VALUE = {0}, ModifiedBy = '{1}', Modified = '{2}' WHERE Id = '{3}' AND Value = {4}";
		private static readonly string DELETE_SQL = "DELETE FROM Version WHERE Id = '{0}' AND Value = {1}";

		internal IManagerContext Context { get { return ApplicationContextHolder.Instance.Context; } }

		public Version Load(Guid id)
		{
			Version version = null;
			var connection = Context.Data.Open();
			try
			{
				using (var command = connection.CreateCommand())
				{
					command.CommandType = CommandType.Text;
					command.CommandText = string.Format(LOAD_SQL, id.ToString());
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						int value = int.Parse(reader["Value"].ToString());
						string modifiedBy = reader["ModifiedBy"].ToString();
						DateTime modified = DateTime.Parse(reader["Modified"].ToString());
						version = Version.Activate(id, value, modifiedBy, modified);
						Context.IdentityMap.Add(id, version);
					}
				}
			}
			catch (DbException dbe)
			{
				throw new ConcurrencyException("Unexpected sql error loading version: " + dbe.Message);
			}
			finally
			{
				Context.Data.Close();
			}

			return version;
		}

		public void Insert(Version version)
		{
			if (!version.IsNew)
				throw new InvalidOperationException("Version object is not new!");

			var connection = Context.Data.Open();
			try
			{
				using (var command = connection.CreateCommand())
				{
					command.CommandType = CommandType.Text;
					command.CommandText = string.Format(INSERT_SQL, version.Id.ToString(), version.Value, version.ModifiedBy, version.Modified.ToString());
					command.Transaction = Context.Data.Transaction;
					command.ExecuteNonQuery();
					Context.IdentityMap.Add(version.Id, version);
				}
			}
			catch (DbException dbe)
			{
				throw new Exception("Unexpected sql error inserting version: " + dbe.Message);
			}
			finally
			{
				Context.Data.Close();
			}
		}

		public void Increment(Version version)
		{
			if (version.IsLocked)
				throw new InvalidOperationException("Version object is already locked!");
			var connection = Context.Data.Open();
			try
			{
				using (var command = connection.CreateCommand())
				{
					command.CommandType = CommandType.Text;
					command.CommandText = string.Format(UPDATE_SQL, (version.Value + 1), version.ModifiedBy, version.Modified.ToString(), version.Id, version.Value);
					command.Transaction = Context.Data.Transaction;
					int rowCount = command.ExecuteNonQuery();
					if (rowCount == 0)
						throw new ConcurrencyException("Concurrency exception");
				}
			}
			catch (DbException dbe)
			{
				throw new Exception("Unexpected sql error incrementing version: " + dbe.Message);
			}
			finally
			{
				Context.Data.Close();
			}
		}

		public void Delete(Version version)
		{
			if (!version.IsNew || version.IsLocked)
				throw new InvalidOperationException("Invalid Version object to delete!");
			var connection = Context.Data.Open();
			try
			{
				using (var command = connection.CreateCommand())
				{
					command.CommandType = CommandType.Text;
					command.CommandText = string.Format(DELETE_SQL, version.Id.ToString(), version.Value);
					command.Transaction = Context.Data.Transaction;
					int rowCount = command.ExecuteNonQuery();
					if (rowCount == 0)
						throw new ConcurrencyException("Concurrency exception");
				}
			}
			catch (DbException dbe)
			{
				throw new Exception("Unexpected sql error incrementing version: " + dbe.Message);
			}
			finally
			{
				Context.Data.Close();
			}
		}
	}
}
