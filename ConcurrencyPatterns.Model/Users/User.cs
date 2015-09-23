using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ConcurrencyPatterns.Infrastructure.Domain;

namespace ConcurrencyPatterns.Model.Users
{
	public sealed class User : EntityBase, IAggregateRoot
	{
		public static User Activate(Guid id, string name)
		{
			return new User() { id = id, Name = name};
		}

		private User() { }

		public string Name { get; private set; }
	}
}
