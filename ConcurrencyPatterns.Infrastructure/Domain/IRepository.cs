using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrencyPatterns.Infrastructure.Domain
{
	public interface IRepository<T> : IReadOnlyRepository<T> where T : IAggregateRoot
	{
		void Add(T entity);
		void Save(T entity);
		void Remove(T entity);
	}
}
