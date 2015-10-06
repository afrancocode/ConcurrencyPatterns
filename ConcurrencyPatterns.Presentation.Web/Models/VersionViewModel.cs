using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConcurrencyPatterns.Presentation.Web.Models
{
	public class VersionViewModel
	{
		public Guid Id { get; set; }
		public int Value { get; set; }
		public string ModifiedBy { get; set; }
		public DateTime Modified { get; set; }

		public VersionViewModel() : this(Guid.Empty, 0, string.Empty, DateTime.MinValue) { }

		public VersionViewModel(Guid id, int value, string modifiedBy, DateTime modified)
		{
			this.Id = id;
			this.Value = value;
			this.ModifiedBy = modifiedBy;
			this.Modified = modified;
		}
	}
}