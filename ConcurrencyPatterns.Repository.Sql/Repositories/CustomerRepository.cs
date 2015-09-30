using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ConcurrencyPatterns.Infrastructure.Data;
using ConcurrencyPatterns.Model.Customers;
using ConcurrencyPatterns.Repository.Sql.Mapping;

namespace ConcurrencyPatterns.Repository.Sql.Repositories
{
	public sealed class CustomerRepository : Repository<Customer>, ICustomerRepository
	{
		protected override IMapper CreateMapper()
		{
			return CustomerMapper.CreateMapper();
		}
	}
}
