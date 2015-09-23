using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConcurrencyPatterns.Infrastructure.Domain;

namespace ConcurrencyPatterns.Infrastructure.Data
{
	public interface IMapper
	{
		EntityBase Find(Guid id);
		IEnumerable<EntityBase> FindAll();
		void Insert(EntityBase entity);
		void Update(EntityBase entity);
		void Delete(EntityBase entity);
	}
}
