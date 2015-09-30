using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConcurrencyPatterns.Infrastructure.Data;
using ConcurrencyPatterns.Infrastructure.UnitOfWork;
using ConcurrencyPatterns.Model.Products;
using ConcurrencyPatterns.Repository.Sql.Mapping;

namespace ConcurrencyPatterns.Repository.Sql.Repositories
{
	public sealed class ProductRepository : Repository<Product>, IProductRepository
	{
		protected override IMapper CreateMapper()
		{
			return ProductMapper.CreateMapper();
		}
	}
}
