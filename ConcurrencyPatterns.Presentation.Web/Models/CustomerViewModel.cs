using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConcurrencyPatterns.Presentation.Web.Models
{
	public class CustomerViewModel
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public VersionViewModel Version { get; set; }

		public CustomerViewModel() : this(Guid.Empty, string.Empty) { }

		public CustomerViewModel(Guid id, string name)
		{
			this.Id = id;
			this.Name = name;
		}
	}
}