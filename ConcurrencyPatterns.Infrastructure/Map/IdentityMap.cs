using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ConcurrencyPatterns.Infrastructure.Domain;

namespace ConcurrencyPatterns.Infrastructure.Map
{
	public sealed class IdentityMap : IIdentityMap
	{
		private Dictionary<Guid, EntityBase> entities;

		public IdentityMap()
		{
			this.entities = new Dictionary<Guid, EntityBase>();
		}

		public EntityBase Get(Guid key)
		{
			EntityBase entity = null;
			this.entities.TryGetValue(key, out entity);
			return entity;
		}

		public void Add(Guid key, EntityBase value)
		{
			if (this.entities.ContainsKey(key))
				this.entities[key] = value;
			else
				this.entities.Add(key, value);
		}

		public void Remove(Guid key)
		{
			this.entities.Remove(key);
		}

		public void Clear()
		{
			this.entities.Clear();
		}
	}
}
