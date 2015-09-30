using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ConcurrencyPatterns.Model.Core;
using ConcurrencyPatterns.Model.Customers;

namespace ConcurrencyPatterns.Repository.Sql.Mapping
{
	public sealed class CustomerAddressesMapper : ChildMapper
	{
		public static readonly string INSERT_SQL = "INSERT INTO CustomerAddresses VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}')";
		public static readonly string UPDATE_SQL = "UPDATE CustomerAddresses SET Line1 = '{0}', Line2 = '{1}', Phone = '{2}', ModifiedBy = '{3}', Modified = '{4}' WHERE Id = '{5}'";
		public static readonly string DELETE_SQL = "DELETE FROM CustomerAddresses WHERE Id = '{0}'";

		internal static IChildMapper CreateMapper()
		{
			return new OptimisticLockingChildMapper(new CustomerAddressesMapper());
		}

		private CustomerAddressesMapper() { }

		protected override string Table
		{
			get { return "CustomerAddresses"; }
		}

		protected override Entity Load(Guid id, IDataReader reader, Entity parent)
		{
			var customer = (Customer)parent;
			var line1 = reader["Line1"].ToString();
			var line2 = reader["Line2"].ToString();
			var phone = reader["Phone"].ToString();
			return customer.LoadAddress(id, line1, line2, phone);
		}

		protected override string GetInsertSQL(Entity entity)
		{
			var address = (Address)entity;
			return string.Format(INSERT_SQL, address.Id.ToString(), address.Customer.Id.ToString(), address.AddressLine1, address.AddressLine2, address.Phone, address.ModifiedBy, address.Modified.ToString());
		}

		protected override string GetUpdateSQL(Entity entity)
		{
			var address = (Address)entity;
			return string.Format(UPDATE_SQL, address.AddressLine1, address.AddressLine2, address.Phone, address.ModifiedBy, address.Modified.ToString(), address.Id.ToString());
		}

		protected override string GetDeleteSQL(Entity entity)
		{
			return string.Format(DELETE_SQL, entity.Id.ToString());
		}
	}
}
