using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConcurrencyPatterns.Infrastructure.Domain;
using ConcurrencyPatterns.Model.Core;

namespace ConcurrencyPatterns.Repository.Sql.Mapping
{
	using Version = ConcurrencyPatterns.Model.Core.Version;

	public abstract class EntityMapper<T> : BaseMapper where T : Entity
	{
		protected EntityMapper() { }

		protected sealed override EntityBase Load(Guid id, IDataReader reader)
		{
			var entity = LoadEntity(id, reader);
			var versionId = new Guid(reader["VersionId"].ToString());
			var value = int.Parse(reader["vValue"].ToString());
			var vModifiedBy = reader["vModifiedBy"].ToString();
			var vModified = DateTime.Parse(reader["vModified"].ToString());
			
			var version = Version.Activate(versionId, value, vModifiedBy, vModified);

			var modifiedBy = reader["ModifiedBy"].ToString();
			var modified = DateTime.Parse(reader["Modified"].ToString());

			entity.SetSystemFields(version, modified, modifiedBy);

			return entity;
		}

		protected abstract T LoadEntity(Guid id, IDataReader reader);

		protected sealed override void Insert(EntityBase entity)
		{
			var connection = Context.Data.Connection;
			var open = (connection.State == ConnectionState.Open);
			if (!open)
				connection.Open();
			try
			{
				using(var command = connection.CreateCommand())
				{
					command.CommandType = CommandType.Text;
					command.CommandText = GetInsertSQL((T)entity);
					command.Transaction = Context.Data.Transaction;
					command.ExecuteNonQuery();
				}
				OnInsert((T)entity);
			}
			catch(DbException dbe)
			{
				throw new Exception(dbe.Message);
			}
			finally
			{
				if(!open)
					connection.Close();
			}
		}

		protected virtual void OnInsert(T entity)
		{
			entity.Version.Insert();
		}

		protected sealed override void Update(EntityBase entity)
		{
			var connection = Context.Data.Connection;
			var open = (connection.State == ConnectionState.Open);
			if (!open)
				connection.Open();
			try
			{
				using(var command = connection.CreateCommand())
				{
					command.CommandType = CommandType.Text;
					command.CommandText = GetUpdateSQL((T)entity);
					command.Transaction = Context.Data.Transaction;
					int rows = command.ExecuteNonQuery();
					if (rows == 0)
						ThrowConcurrencyException((T)entity);
				}
				OnUpdate((T)entity);
			}
			catch(DbException dbe)
			{
				throw new Exception(dbe.Message);
			}
			finally
			{
				if(!open)
					connection.Close();
			}
		}

		protected virtual void OnUpdate(T entity) 
		{
			entity.Version.Increment();
		}

		protected sealed override void Delete(EntityBase entity)
		{
			var connection = Context.Data.Connection;
			var open = (connection.State == ConnectionState.Open);
			if (!open)
				connection.Open();
			try
			{
				using(var command = connection.CreateCommand())
				{
					command.CommandType = CommandType.Text;
					command.CommandText = GetDeleteSQL((T)entity);
					command.Transaction = Context.Data.Transaction;
					command.ExecuteNonQuery();
				}
				OnDelete((T)entity);
			}
			catch (DbException dbe)
			{
				throw new Exception(dbe.Message);
			}
			finally 
			{
				if(!open)
					connection.Close();
			}
		}

		protected virtual void OnDelete(T entity) 
		{
			entity.Version.Delete();
		}

		protected void ThrowConcurrencyException(T entity)
		{
			throw new Exception("Concurrency exception for entity '{0}'" + entity.Id.ToString());
		}

		protected abstract string GetInsertSQL(T entity);
		protected abstract string GetUpdateSQL(T entity);
		protected abstract string GetDeleteSQL(T entity);
	}
}
