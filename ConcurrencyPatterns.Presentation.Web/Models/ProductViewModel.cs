using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConcurrencyPatterns.Presentation.Web.Models
{
	public class ProductViewModel
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public int Stock { get; set; }
		public VersionViewModel Version { get; set; }

		public ProductViewModel() : this(Guid.Empty, string.Empty, string.Empty, 0) { }

		public ProductViewModel(Guid id, string name, string description, int stock)
		{
			this.Id = id;
			this.Name = name;
			this.Description = description;
			this.Stock = stock;
		}
	}
}