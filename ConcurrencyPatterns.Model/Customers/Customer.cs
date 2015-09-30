using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ConcurrencyPatterns.Infrastructure.Domain;
using ConcurrencyPatterns.Model.Core;

namespace ConcurrencyPatterns.Model.Customers
{
	using Version = ConcurrencyPatterns.Model.Core.Version;

	public sealed class Customer : Entity, IAggregateRoot
	{
		public static Customer Create(string name, string createdBy)
		{
			var customer = new Customer(Guid.NewGuid(), name);
			customer.SetSystemFields(Version.Create(createdBy), DateTime.UtcNow, createdBy);
			return customer;
		}

		public static Customer Activate(Guid id, string name)
		{
			return new Customer(id, name);
		}

		private string name;
		private IList<Address> addresses;

		private Customer(Guid id, string name)
			: base(id)
		{
			this.name = name;
			this.addresses = new List<Address>();
		}

		public string Name
		{
			get { return this.name; }
			set { this.name = value; }
		}

		public IEnumerable<Address> GetAddresses()
		{
			return this.addresses;
		}

		public Address AddAddress(string line1, string line2, string phone, string createdBy)
		{
			var address = Address.Create(this, this.Version, line1, line2, phone, createdBy);
			this.addresses.Add(address);
			return address;
		}

		public Address LoadAddress(Guid id, string line1, string line2, string phone)
		{
			var address = Address.Activate(id, this, line1, line2, phone);
			this.addresses.Add(address);
			return address;
		}

		public void RemoveAddress(Address address)
		{
			this.addresses.Remove(address);
		}
	}
}
