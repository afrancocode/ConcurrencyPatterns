using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConcurrencyPatterns.Infrastructure.Domain;

namespace ConcurrencyPatterns.Infrastructure.UnitOfWork
{
	public interface IUnitOfWorkRepository
	{
		void PersistCreationOf(IAggregateRoot entity);
		void PersistUpdateOf(IAggregateRoot entity);
		void PersistDeletionOf(IAggregateRoot entity);
	}
}
