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
			var updateEntity = (T)entity;
			if (!updateEntity.IsNew)
				throw new InvalidOperationException("Cannot Insert an Existing Entity");

			var connection = Context.Data.Open();
			try
			{
				using (var command = connection.CreateCommand())
				{
					command.CommandType = CommandType.Text;
					command.CommandText = GetInsertSQL(updateEntity);
					command.Transaction = Context.Data.Transaction;
					command.ExecuteNonQuery();
				}
				OnInsert(updateEntity);
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

		protected virtual void OnInsert(T entity) { }

		protected sealed override void Update(EntityBase entity)
		{
			var updateEntity = (T)entity;
			if (updateEntity.IsNew)
				throw new InvalidOperationException("Cannot Update a New Entity");

			var connection = Context.Data.Open();
			try
			{
				if (updateEntity.IsDirty)
				{
					using (var command = connection.CreateCommand())
					{
						command.CommandType = CommandType.Text;
						command.CommandText = GetUpdateSQL(updateEntity);
						command.Transaction = Context.Data.Transaction;
						int rows = command.ExecuteNonQuery();
						if (rows == 0)
							ThrowConcurrencyException(updateEntity);
					}
				}
				OnUpdate(updateEntity);
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

		protected virtual void OnUpdate(T entity) { }

		protected sealed override void Delete(EntityBase entity)
		{
			var updateEntity = (T)entity;
			if (updateEntity.IsNew)
				throw new InvalidOperationException("Cannot Delete a New Entity");

			var connection = Context.Data.Open();
			try
			{
				OnDelete(updateEntity);
				using (var command = connection.CreateCommand())
				{
					command.CommandType = CommandType.Text;
					command.CommandText = GetDeleteSQL(updateEntity);
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

		protected virtual void OnDelete(T entity) { }

		protected void ThrowConcurrencyException(T entity)
		{
			throw new Exception("Concurrency exception for entity '{0}'" + entity.Id.ToString());
		}

		protected abstract string GetInsertSQL(T entity);
		protected abstract string GetUpdateSQL(T entity);
		protected abstract string GetDeleteSQL(T entity);
	}
}
