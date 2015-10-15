using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ConcurrencyPatterns.Model.Core;

namespace ConcurrencyPatterns.Model.Customers
{
	using Version = ConcurrencyPatterns.Model.Core.Version;

	public sealed class Address : Entity
	{
		internal static Address Create(Customer customer, Version version, string line1, string line2, string phone)
		{
			var createdBy = CurrentContext.Session.OwnerName;
			var address = new Address(Guid.NewGuid(), customer, line1, line2, phone);
			address.SetSystemFields(version, DateTime.UtcNow, createdBy);
			address.isNew = true;
			return address;
		}

		internal static Address Activate(Guid id, Customer customer, string line1, string line2, string phone)
		{
			return new Address(id, customer, line1, line2, phone);
		}

		private Customer customer;
		private string line1;
		private string line2;
		private string phone;

		private Address(Guid id, Customer customer, string line1, string line2, string phone)
			: base(id)
		{
			this.customer = customer;
			this.line1 = line1;
			this.line2 = line2;
			this.phone = phone;
		}

		public Customer Customer { get { return this.customer; } }

		public string AddressLine1
		{
			get { return this.line1; }
			set
			{
				this.line1 = value;
				SetDirty();
			}
		}

		public string AddressLine2
		{
			get { return this.line2; }
			set
			{
				this.line2 = value;
				SetDirty();
			}
		}

		public string Phone
		{
			get { return this.phone; }
			set
			{
				this.phone = value;
				SetDirty();
			}
		}
	}
}
