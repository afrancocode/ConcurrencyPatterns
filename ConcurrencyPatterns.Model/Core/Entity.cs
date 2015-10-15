using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ConcurrencyPatterns.Infrastructure.Domain;
using ConcurrencyPatterns.Infrastructure.Context;

namespace ConcurrencyPatterns.Model.Core
{
	public abstract class Entity : EntityBase
	{
		internal static IManagerContext CurrentContext { get { return ApplicationContextHolder.Instance.Context; } }

		private DateTime modified;
		private string modifiedBy;
		private Version version;
		protected bool isNew;
		private bool isDirty;

		protected Entity(Guid id)
		{
			this.id = id;
		}

		protected IManagerContext Context { get { return Entity.CurrentContext; } }

		public Version Version { get { return this.version; } }
		public DateTime Modified { get { return this.modified; } }
		public string ModifiedBy { get { return this.modifiedBy; } }

		public bool IsNew { get { return this.isNew; } }
		public bool IsDirty { get { return this.isDirty && !this.isNew; } }

		public void SetSystemFields(Version version, DateTime modified, string modifiedBy)
		{
			this.version = version;
			this.modified = modified;
			this.modifiedBy = modifiedBy;
		}

		protected void SetDirty()
		{
			if (this.isDirty || this.isNew) return;
			this.isDirty = true;
		}
	}
}
