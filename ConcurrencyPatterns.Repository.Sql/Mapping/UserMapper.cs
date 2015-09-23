using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ConcurrencyPatterns.Infrastructure.Domain;
using ConcurrencyPatterns.Infrastructure.Session;
using ConcurrencyPatterns.Model.Users;

namespace ConcurrencyPatterns.Repository.Sql.Mapping
{
	public sealed class UserMapper : BaseMapper
	{
		public UserMapper() { }

		protected override string Table { get { return "Users"; } }

		protected override EntityBase Load(Guid id, IDataReader reader)
		{
			return User.Activate(id, reader["Name"].ToString());
		}

		#region Not Supported

		protected override void Insert(EntityBase entity) { throw new NotSupportedException(); }
		protected override void Update(EntityBase entity) { throw new NotSupportedException(); }
		protected override void Delete(EntityBase entity) { throw new NotSupportedException(); }

		#endregion
	}
}
