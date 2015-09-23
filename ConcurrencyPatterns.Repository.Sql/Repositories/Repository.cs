using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConcurrencyPatterns.Infrastructure.Context;
using ConcurrencyPatterns.Infrastructure.Domain;
using ConcurrencyPatterns.Infrastructure.UnitOfWork;

namespace ConcurrencyPatterns.Repository.Sql.Repositories
{
	public abstract class Repository<T> : ReadOnlyRepository<T>, IRepository<T>, IUnitOfWorkRepository where T :  EntityBase, IAggregateRoot
	{
		protected IManagerContext Context { get { return ApplicationContextHolder.Instance.Context; } }

		protected IUnitOfWork UnitOfWork { get { return Context.UnitOfWork; } }

		public void Add(T entity)
		{
			this.UnitOfWork.RegisterNew(entity, this);
		}

		public void Save(T entity)
		{
			this.UnitOfWork.RegisterAmended(entity, this);
		}

		public void Remove(T entity)
		{
			this.UnitOfWork.RegisterRemoved(entity, this);
		}

		void IUnitOfWorkRepository.PersistCreationOf(IAggregateRoot entity)
		{
			this.Mapper.Insert((EntityBase)entity);
		}

		void IUnitOfWorkRepository.PersistUpdateOf(IAggregateRoot entity)
		{
			this.Mapper.Update((EntityBase)entity);
		}

		void IUnitOfWorkRepository.PersistDeletionOf(IAggregateRoot entity)
		{
			this.Mapper.Delete((EntityBase)entity);
		}
	}
}
