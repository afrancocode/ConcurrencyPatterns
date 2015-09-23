using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ConcurrencyPatterns.Infrastructure.Domain;

namespace ConcurrencyPatterns.Model.Core
{
	public interface IEntityRepository<T> : IRepository<T> where T : Entity
	{
	}
}
