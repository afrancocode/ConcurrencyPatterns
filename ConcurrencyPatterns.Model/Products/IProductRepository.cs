using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ConcurrencyPatterns.Infrastructure.Domain;
using ConcurrencyPatterns.Model.Core;

namespace ConcurrencyPatterns.Model.Products
{
	public interface IProductRepository : IEntityRepository<Product>
	{
	}
}
