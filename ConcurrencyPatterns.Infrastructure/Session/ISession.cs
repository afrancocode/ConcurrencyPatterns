using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ConcurrencyPatterns.Infrastructure.Map;

namespace ConcurrencyPatterns.Infrastructure.Session
{
	public interface ISession
	{
		Guid Id { get; }
		Guid Owner { get; }
		string OwnerName { get; }
		bool IsLoggedIn { get; }
		void Initialize();
		void InitSession(Guid owner, string ownerName);
		void EndSession();
	}
}
