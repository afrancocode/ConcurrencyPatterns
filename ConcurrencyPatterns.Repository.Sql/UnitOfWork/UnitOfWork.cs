using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ConcurrencyPatterns.Infrastructure.Context;
using ConcurrencyPatterns.Infrastructure.Domain;
using ConcurrencyPatterns.Infrastructure.UnitOfWork;

namespace ConcurrencyPatterns.Repository.Sql.UnitOfWork
{
	public sealed class UnitOfWork : IUnitOfWork
	{
		private Dictionary<IUnitOfWorkRepository, List<IAggregateRoot>> add;
		private Dictionary<IUnitOfWorkRepository, List<IAggregateRoot>> update;
		private Dictionary<IUnitOfWorkRepository, List<IAggregateRoot>> remove;

		public UnitOfWork()
		{
			this.add = new Dictionary<IUnitOfWorkRepository, List<IAggregateRoot>>();
			this.update = new Dictionary<IUnitOfWorkRepository, List<IAggregateRoot>>();
			this.remove = new Dictionary<IUnitOfWorkRepository, List<IAggregateRoot>>();
		}

		internal IManagerContext Context { get { return ApplicationContextHolder.Instance.Context; } }

		public void RegisterAmended(IAggregateRoot entity, IUnitOfWorkRepository repository)
		{
			List<IAggregateRoot> items = null;
			if (!update.TryGetValue(repository, out items))
			{
				items = new List<IAggregateRoot>();
				update.Add(repository, items);
			}
			items.Add(entity);
		}

		public void RegisterNew(IAggregateRoot entity, IUnitOfWorkRepository repository)
		{
			List<IAggregateRoot> items = null;
			if (!add.TryGetValue(repository, out items))
			{
				items = new List<IAggregateRoot>();
				add.Add(repository, items);
			}
			items.Add(entity);
		}

		public void RegisterRemoved(IAggregateRoot entity, IUnitOfWorkRepository repository)
		{
			List<IAggregateRoot> items = null;
			if (!remove.TryGetValue(repository, out items))
			{
				items = new List<IAggregateRoot>();
				remove.Add(repository, items);
			}
			items.Add(entity);
		}

		public void Commit()
		{
			var connection = Context.Data.Connection;
			connection.Open();
			Context.Data.Transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);
			try
			{
				InsertNew();
				DeleteRemoved();
				UpdateDirty();
				Context.Data.Transaction.Commit();
			}
			catch
			{
				Context.Data.Transaction.Rollback();
				throw;
			}
			finally
			{
				connection.Close();
			}
		}

		private void InsertNew() 
		{
			foreach (var insertItem in this.add)
			{
				var persistTo = insertItem.Key;
				while (insertItem.Value.Any())
				{
					var entity = insertItem.Value[0];
					persistTo.PersistCreationOf(entity);
					insertItem.Value.RemoveAt(0);
				}
			}
		}

		private void DeleteRemoved()
		{
			foreach (var deleteItem in this.remove)
			{
				var persistTo = deleteItem.Key;
				while (deleteItem.Value.Any())
				{
					var entity = deleteItem.Value[0];
					persistTo.PersistDeletionOf(entity);
					deleteItem.Value.RemoveAt(0);
				}
			}
		}

		private void UpdateDirty()
		{
			foreach (var updateInfo in this.update)
			{
				var persistTo = updateInfo.Key;
				while (updateInfo.Value.Any())
				{
					var entity = updateInfo.Value[0];
					persistTo.PersistUpdateOf(entity);
					updateInfo.Value.RemoveAt(0);
				}
			}
		}
	}
}
