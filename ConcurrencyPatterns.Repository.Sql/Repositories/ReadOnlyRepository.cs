using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ConcurrencyPatterns.Infrastructure.Data;
using ConcurrencyPatterns.Infrastructure.Domain;
using ConcurrencyPatterns.Infrastructure.Session;

namespace ConcurrencyPatterns.Repository.Sql.Repositories
{
	public abstract class ReadOnlyRepository<T> : IReadOnlyRepository<T> where T : EntityBase, IAggregateRoot
	{
		private IMapper mapper;

		protected IMapper Mapper
		{
			get
			{
				if (mapper == null)
					mapper = CreateMapper();
				return mapper;
			}
		}

		protected abstract IMapper CreateMapper();

		public T FindBy(Guid id)
		{
			return (T)Mapper.Find(id);
		}

		public IEnumerable<T> FindAll()
		{
			return Mapper.FindAll().Cast<T>();
		}
	}
}
