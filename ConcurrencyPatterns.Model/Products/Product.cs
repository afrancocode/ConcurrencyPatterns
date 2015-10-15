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
		public static Product Create(string name, string description, int stock)
		{
			var createdBy = CurrentContext.Session.OwnerName;
			var product = new Product(Guid.NewGuid(), name, description, stock);
			product.SetSystemFields(Version.Create(createdBy), DateTime.UtcNow, createdBy);
			product.isNew = true;
			return product;
		}

		public static Product Activate(Guid id, string name, string description, int stock, bool update = false)
		{
			var product = new Product(id, name, description, stock);
			if (update) product.SetDirty();
			return product;
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
			set
			{
				this.name = value;
				SetDirty();
			}
		}

		public string Description
		{
			get { return this.description; }
			set
			{
				this.description = value;
				SetDirty();
			}
		}

		public int Stock
		{
			get { return this.stock; }
			set
			{
				this.stock = value;
				SetDirty();
			}
		}
	}
}
