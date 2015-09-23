using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ConcurrencyPatterns.Infrastructure.Domain;

namespace ConcurrencyPatterns.Infrastructure.Map
{
	public interface IIdentityMap
	{
		EntityBase Get(Guid key);
		void Add(Guid key, EntityBase value);
		void Remove(Guid key);
		void Clear();
	}
}
