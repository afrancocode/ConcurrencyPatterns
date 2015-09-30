using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ConcurrencyPatterns.Infrastructure.Domain;

namespace ConcurrencyPatterns.Model.Core
{
	public abstract class Entity : EntityBase
	{
		private DateTime modified;
		private string modifiedBy;
		private Version version;

		protected Entity(Guid id)
		{
			this.id = id;
		}

		public Version Version { get { return this.version; } }
		public DateTime Modified { get { return this.modified; } }
		public string ModifiedBy { get { return this.modifiedBy; } }

		public void SetSystemFields(Version version, DateTime modified, string modifiedBy)
		{
			this.version = version;
			this.modified = modified;
			this.modifiedBy = modifiedBy;
		}
	}
}
