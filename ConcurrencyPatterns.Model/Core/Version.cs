using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ConcurrencyPatterns.Infrastructure.Domain;
using ConcurrencyPatterns.Infrastructure.Data;
using ConcurrencyPatterns.Infrastructure.Context;

namespace ConcurrencyPatterns.Model.Core
{
	public sealed class Version : EntityBase
	{
		public static Version Create(string owner)
		{
			var version = new Version(Guid.NewGuid(), 0, owner, DateTime.UtcNow);
			version.isNew = true;
			return version;
		}

		public static Version Activate(Guid id, int value, string modifiedBy, DateTime modified)
		{
			return new Version(id, value, modifiedBy, modified);
		}

		private int value;
		private string modifiedBy;
		private DateTime modified;
		private bool locked;
		private bool isNew;

		internal IVersionStorage Storage
		{
			get { return ApplicationContextHolder.Instance.GetService<IVersionStorage>(typeof(IVersionStorage)); }
		}

		private Version(Guid id, int value, string modifiedBy, DateTime modified)
		{
			this.id = id;
			this.value = value;
			this.modifiedBy = modifiedBy;
			this.modified = modified;
		}

		public bool IsNew { get { return this.isNew; } }

		public bool IsLocked { get { return this.locked; } }

		public int Value { get { return this.value; } }

		public string ModifiedBy { get { return this.modifiedBy; } }

		public DateTime Modified { get { return this.modified; } }

		public void Insert()
		{
			if (IsNew)
			{
				this.Storage.Insert(this);
				isNew = false;
			}
		}

		public void Increment()
		{
			if (!IsLocked)
			{
				this.Storage.Increment(this);
				this.value++;
				this.locked = true;
			}
		}

		public void Delete()
		{
			if (locked && !isNew)
			{
				this.Storage.Delete(this);
			}
		}
	}
}
