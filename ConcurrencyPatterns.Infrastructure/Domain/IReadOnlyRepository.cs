using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrencyPatterns.Infrastructure.Domain
{
	public interface IReadOnlyRepository<T> where T : IAggregateRoot
	{
		T FindBy(Guid id);
		IEnumerable<T> FindAll();
	}
}
