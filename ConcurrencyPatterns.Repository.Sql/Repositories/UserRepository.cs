using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConcurrencyPatterns.Infrastructure.Data;
using ConcurrencyPatterns.Infrastructure.Session;
using ConcurrencyPatterns.Model.Users;
using ConcurrencyPatterns.Repository.Sql.Mapping;

namespace ConcurrencyPatterns.Repository.Sql.Repositories
{
	public sealed class UserRepository : ReadOnlyRepository<User>, IUserRepository
	{
		protected override IMapper CreateMapper()
		{
			return new UserMapper();
		}
	}
}
