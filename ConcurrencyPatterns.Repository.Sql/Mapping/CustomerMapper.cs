using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ConcurrencyPatterns.Infrastructure.Domain;
using ConcurrencyPatterns.Model.Core;
using ConcurrencyPatterns.Model.Customers;

namespace ConcurrencyPatterns.Repository.Sql.Mapping
{
	public sealed class CustomerMapper : EntityMapper<Customer>
	{
		private static readonly string LOAD_SQL = "SELECT Customers.Id, Customers.Name, Customers.ModifiedBy, Customers.Modified, Customers.VersionId, v.Value as vValue, v.ModifiedBy as vModifiedBy, v.Modified as vModified FROM Customers INNER JOIN Version AS v ON Customers.VersionId = v.Id WHERE Customers.Id='{0}'";
		private static readonly string LOAD_ALL_SQL = "SELECT Customers.Id, Customers.Name, Customers.ModifiedBy, Customers.Modified, Customers.VersionId, v.Value as vValue, v.ModifiedBy as vModifiedBy, v.Modified as vModified FROM Customers INNER JOIN Version AS v ON Customers.VersionId = v.Id";
		private static readonly string INSERT_SQL = "INSERT INTO Customers VALUES ('{0}', '{1}', '{2}', '{3}', '{4}')";
		private static readonly string UPDATE_SQL = "UPDATE Customers SET Name = '{0}', ModifiedBy = '{1}', Modified = '{2}' WHERE Id = '{3}' AND VersionId = '{4}'";
		private static readonly string DELETE_SQL = "DELETE FROM Customers WHERE Id = '{0}' AND VersionId = '{1}'";

		private CustomerAddressesMapper addresses;

		public CustomerMapper()
		{
			addresses = new CustomerAddressesMapper();
		}

		protected override string Table { get { return "Customers"; } }

		protected override Customer LoadEntity(Guid id, IDataReader reader)
		{
			return Customer.Activate(id, reader["Name"].ToString());
		}

		protected override void OnLoad(Guid id, EntityBase entity)
		{
			addresses.Load((Entity)entity);
		}

		protected override void OnInsert(Customer entity)
		{
			foreach(var address in entity.GetAddresses())
			{
				addresses.Insert(address);
			}
		}

		protected override void OnUpdate(Customer entity)
		{
			OnDelete(entity);
			OnInsert(entity);
		}

		protected override void OnDelete(Customer entity)
		{
			foreach(var address in entity.GetAddresses())
			{
				addresses.Delete(address);
			}
		}

		protected override string GetLoadSQL(Guid id)
		{
			return string.Format(LOAD_SQL, id.ToString());
		}

		protected override string GetLoadAllSQL()
		{
			return string.Format(LOAD_ALL_SQL);
		}

		protected override string GetInsertSQL(Customer entity)
		{
			return string.Format(INSERT_SQL, entity.Id.ToString(), entity.Name, entity.ModifiedBy, entity.Modified.ToString(), entity.Version.Id.ToString());
		}

		protected override string GetUpdateSQL(Customer entity)
		{
			return string.Format(UPDATE_SQL, entity.Name, entity.ModifiedBy, entity.Modified.ToString(), entity.Id.ToString(), entity.Version.Id.ToString());
		}

		protected override string GetDeleteSQL(Customer entity)
		{
			return string.Format(DELETE_SQL, entity.Id.ToString(), entity.Version.Id.ToString());
		}
	}
}
