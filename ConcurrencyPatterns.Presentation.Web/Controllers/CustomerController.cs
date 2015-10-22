using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ConcurrencyPatterns.Model.Customers;
using ConcurrencyPatterns.Presentation.Web.Models;

namespace ConcurrencyPatterns.Presentation.Web.Controllers
{
	[CookieFilter]
	public class CustomerController : BaseController
	{
		private ICustomerRepository customers;

		public CustomerController(ICustomerRepository customers)
		{
			this.customers = customers;
		}

		//
		// GET: /Customer/
		public ActionResult Index()
		{
			var customerList = customers.FindAll().ToList().OrderBy(x => x.Name);
			return View(customerList);
		}

		public PartialViewResult Create()
		{
			return PartialView();
		}

		[HttpPost]
		public ActionResult Create(CustomerViewModel customer)
		{
			CoreCreate(customer);
			return RedirectToAction("Index");
		}

		public PartialViewResult Delete(Guid id)
		{
			return PartialView(GetCustomerViewModel(id));
		}

		[HttpPost]
		public ActionResult Delete(CustomerViewModel customer)
		{
			CoreDelete(customer);
			return RedirectToAction("Index");
		}

		#region Private Methods

		private CustomerViewModel GetCustomerViewModel(Guid id)
		{
			var customer = customers.FindBy(id);
			var version = customer.Version;
			var customerViewModel = new CustomerViewModel(id, customer.Name);
			customerViewModel.Version = new VersionViewModel(version.Id, version.Value, version.ModifiedBy, version.Modified);
			return customerViewModel;
		}

		private void CoreCreate(CustomerViewModel customer)
		{
			var newCustomer = Customer.Create(customer.Name);
			customers.Add(newCustomer);
			this.ManagerContext.UnitOfWork.Commit();
		}

		private void CoreDelete(CustomerViewModel customerView)
		{
			var customer = ActivateCustomer(customerView);
			customers.Remove(customer);
			this.ManagerContext.UnitOfWork.Commit();
		}

		private Customer ActivateCustomer(CustomerViewModel customerView)
		{
			var customer = Customer.Activate(customerView.Id, customerView.Name, true);
			var version = ConcurrencyPatterns.Model.Core.Version.Activate(customerView.Version.Id, customerView.Version.Value, customerView.Version.ModifiedBy, customerView.Version.Modified);
			customer.SetSystemFields(version, DateTime.Now, UserName);
			return customer;
		}

		#endregion
	}
}