using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ConcurrencyPatterns.Model.Core;

namespace ConcurrencyPatterns.Model.Customers
{
	public interface ICustomerRepository : IEntityRepository<Customer>
	{
	}
}
