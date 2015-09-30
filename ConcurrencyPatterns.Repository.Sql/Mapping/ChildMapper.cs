using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ConcurrencyPatterns.Infrastructure.Context;
using ConcurrencyPatterns.Model.Core;

namespace ConcurrencyPatterns.Repository.Sql.Mapping
{
	public abstract class ChildMapper
	{
		private static readonly string LOAD_SQL = "SELECT * FROM {0} WHERE ParentId = '{1}'";

		internal IManagerContext Context { get { return ApplicationContextHolder.Instance.Context; } }

		protected abstract string Table { get; }

		public void Load(Entity parent)
		{
			try
			{
				var connection = Context.Data.Open();
				try
				{
					using (var command = connection.CreateCommand())
					{
						command.CommandType = CommandType.Text;
						command.CommandText = GetLoadSQL(parent.Id);
						var reader = command.ExecuteReader();
						while (reader.Read())
						{
							var id = new Guid(reader["Id"].ToString());
							var modifiedBy = reader["ModifiedBy"].ToString();
							var modified = DateTime.Parse(reader["Modified"].ToString());

							var item = Load(id, reader, parent);
							item.SetSystemFields(parent.Version, modified, modifiedBy);
						}
					}
				}
				finally
				{
					Context.Data.Close();
				}
			}
			catch (DbException dbe)
			{
				throw new Exception(dbe.Message);
			}
		}

		protected abstract Entity Load(Guid id, IDataReader reader, Entity parent);

		protected virtual string GetLoadSQL(Guid parentId)
		{
			return string.Format(LOAD_SQL, Table, parentId.ToString());
		}

		public void Insert(Entity entity)
		{
			entity.Version.Increment();
			var connection = Context.Data.Open();
			try
			{
				using (var command = connection.CreateCommand())
				{
					command.CommandType = CommandType.Text;
					command.CommandText = GetInsertSQL(entity);
					command.Transaction = Context.Data.Transaction;
					command.ExecuteNonQuery();
				}
			}
			catch (DbException dbe)
			{
				throw new Exception(dbe.Message);
			}
			finally
			{
				Context.Data.Close();
			}
		}

		protected abstract string GetInsertSQL(Entity entity);

		public void Update(Entity entity)
		{
			entity.Version.Increment();
			var connection = Context.Data.Open();
			try
			{
				using (var command = connection.CreateCommand())
				{
					command.CommandType = CommandType.Text;
					command.CommandText = GetUpdateSQL(entity);
					command.Transaction = Context.Data.Transaction;
					int rows = command.ExecuteNonQuery();
					if (rows == 0)
						ThrowConcurrencyException(entity);
				}
			}
			catch (DbException dbe)
			{
				throw new Exception(dbe.Message);
			}
			finally
			{
				Context.Data.Close();
			}
		}

		protected abstract string GetUpdateSQL(Entity entity);

		public void Delete(Entity entity)
		{
			entity.Version.Increment();
			var connection = Context.Data.Open();
			try
			{
				using (var command = connection.CreateCommand())
				{
					command.CommandType = CommandType.Text;
					command.CommandText = GetDeleteSQL(entity);
					command.Transaction = Context.Data.Transaction;
					command.ExecuteNonQuery();
				}
			}
			catch (DbException dbe)
			{
				throw new Exception(dbe.Message);
			}
			finally
			{
				Context.Data.Close();
			}
		}

		protected abstract string GetDeleteSQL(Entity entity);

		protected void ThrowConcurrencyException(Entity entity)
		{
			throw new Exception("Concurrency exception for entity '{0}'" + entity.Id.ToString());
		}
	}
}
