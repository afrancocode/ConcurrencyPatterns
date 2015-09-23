using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrencyPatterns.Model.Core
{
	public interface IVersionStorage
	{
		Version Load(Guid id);
		void Insert(Version version);
		void Increment(Version version);
		void Delete(Version version);
	}
}
