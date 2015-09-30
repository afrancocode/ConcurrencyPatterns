using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ConcurrencyPatterns.Infrastructure.Context;
using ConcurrencyPatterns.Infrastructure.Data;
using ConcurrencyPatterns.Infrastructure.Domain;
using ConcurrencyPatterns.Infrastructure.Session;


namespace ConcurrencyPatterns.Repository.Sql.Mapping
{
	public abstract class BaseMapper : IMapper
	{
		private static readonly string LOAD_SQL = "SELECT * FROM {0} WHERE Id = '{1}'";
		private static readonly string LOAD_ALL_SQL = "SELECT * FROM {0}";

		protected BaseMapper() { }

		protected abstract string Table { get; }

		internal IManagerContext Context { get { return ApplicationContextHolder.Instance.Context; } }

		public EntityBase Find(Guid id)
		{
			var entity = Context.IdentityMap.Get(id);
			if (entity == null)
			{
				try
				{
					var connection = Context.Data.Open();
					try
					{
						using (var command = connection.CreateCommand())
						{
							command.CommandType = CommandType.Text;
							command.CommandText = GetLoadSQL(id);
							var reader = command.ExecuteReader();
							if (reader.Read())
							{
								entity = Load(id, reader);
								Context.IdentityMap.Add(id, entity);
							}
						}
					}
					finally
					{
						Context.Data.Close();
					}
					OnLoad(id, entity);
				}
				catch (DbException dbe)
				{
					throw new Exception(dbe.Message);
				}
			}
			return entity;
		}

		protected virtual void OnLoad(Guid id, EntityBase entity) { }

		public IEnumerable<EntityBase> FindAll()
		{
			var connection = Context.Data.Open();
			try
			{
				using (var command = connection.CreateCommand())
				{
					command.CommandType = CommandType.Text;
					command.CommandText = GetLoadAllSQL();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						var id = new Guid(reader["Id"].ToString());
						var entity = Load(id, reader);
						Context.IdentityMap.Add(id, entity);
						yield return entity;
					}
				}
			}
			finally
			{
				Context.Data.Close();
			}
		}

		protected abstract EntityBase Load(Guid id, IDataReader reader);

		protected abstract void Insert(EntityBase entity);
		protected abstract void Update(EntityBase entity);
		protected abstract void Delete(EntityBase entity);

		protected virtual string GetLoadSQL(Guid id)
		{
			return string.Format(LOAD_SQL, Table, id.ToString());
		}

		protected virtual string GetLoadAllSQL()
		{
			return string.Format(LOAD_ALL_SQL, Table);
		}

		#region IMapper

		EntityBase IMapper.Find(Guid id) { return this.Find(id); }
		IEnumerable<EntityBase> IMapper.FindAll() { return this.FindAll(); }
		void IMapper.Insert(EntityBase entity) { this.Insert(entity); }
		void IMapper.Update(EntityBase entity) { this.Update(entity); }
		void IMapper.Delete(EntityBase entity) { this.Delete(entity); }

		#endregion
	}
}
