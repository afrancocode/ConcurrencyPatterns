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
			customer.isNew = true;
			return customer;
		}

		public static Customer Activate(Guid id, string name)
		{
			return new Customer(id, name);
		}

		private string name;
		private AddressModifications modifications;
		private IList<Address> addresses;

		private Customer(Guid id, string name)
			: base(id)
		{
			this.name = name;
			this.addresses = new List<Address>();
			this.modifications = new AddressModifications(GetAddresses);
		}

		public string Name
		{
			get { return this.name; }
			set
			{
				this.name = value;
				SetDirty();
			}
		}

		public IEnumerable<Address> GetAddresses()
		{
			return this.addresses;
		}

		public Address AddAddress(string line1, string line2, string phone, string createdBy)
		{
			var address = Address.Create(this, this.Version, line1, line2, phone, createdBy);
			this.addresses.Add(address);
			this.modifications.Insert(address);
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
			this.modifications.Remove(address);
		}

		public AddressModifications GetAddressModifications()
		{
			return this.modifications;
		}
	}

	public sealed class AddressModifications
	{
		private IList<Address> toRemove = new List<Address>();
		private IList<Address> toInsert = new List<Address>();
		private Func<IEnumerable<Address>> addresses;

		internal AddressModifications(Func<IEnumerable<Address>> addresses)
		{
			this.addresses = addresses;
		}

		internal void Remove(Address address)
		{
			if (address.IsNew) return;
			this.toRemove.Add(address);
		}

		internal void Insert(Address address)
		{
			if (!address.IsNew) return;
			this.toInsert.Add(address);
		}

		public IEnumerable<Address> GetUpdates()
		{
			foreach (var address in addresses())
			{
				if (!address.IsNew && address.IsDirty)
					yield return address;
			}
		}

		public IEnumerable<Address> GetDeletes() { return this.toRemove; }

		public IEnumerable<Address> GetInserts() { return this.toInsert; }
	}
}
