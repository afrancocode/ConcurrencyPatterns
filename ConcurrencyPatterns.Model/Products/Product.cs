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
	using Version = ConcurrencyPatterns.Model.Core.Version;

	public sealed class Product : Entity, IAggregateRoot
	{
		public static Product Create(string name, string description, int stock, string modifiedBy)
		{
			var product = new Product(Guid.NewGuid(), name, description, stock);
			product.SetSystemFields(Version.Create(modifiedBy), DateTime.UtcNow, modifiedBy);
			return product;
		}

		public static Product Activate(Guid id, string name, string description, int stock)
		{
			return new Product(id, name, description, stock);
		}

		private string name;
		private string description;
		private int stock;

		private Product(Guid id, string name, string description, int stock)
			: base(id)
		{
			this.name = name;
			this.description = description;
			this.stock = stock;
		}

		public string Name
		{
			get { return this.name; }
			set { this.name = value; }
		}

		public string Description
		{
			get { return this.description; }
			set { this.description = value; }
		}

		public int Stock
		{
			get { return this.stock; }
			set { this.stock = value; }
		}
	}
}
