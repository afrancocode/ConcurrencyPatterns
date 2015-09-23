using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrencyPatterns.Infrastructure.Locking
{
	public sealed class ConcurrencyException : Exception
	{
		public ConcurrencyException(string message)
			: base(message)
		{ }
	}
}
