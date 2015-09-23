using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ConcurrencyPatterns.Infrastructure.Domain;

namespace ConcurrencyPatterns.Infrastructure.UnitOfWork
{
	public interface IUnitOfWork
	{
		void RegisterAmended(IAggregateRoot entity, IUnitOfWorkRepository repository);
		void RegisterNew(IAggregateRoot entity, IUnitOfWorkRepository repository);
		void RegisterRemoved(IAggregateRoot entity, IUnitOfWorkRepository repository);
		void Commit();
	}
}
