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

		public ActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Create(CustomerViewModel customer)
		{
			var newCustomer = Customer.Create(customer.Name);
			customers.Add(newCustomer);
			this.ManagerContext.UnitOfWork.Commit();
			return RedirectToAction("Index");
		}
	}
}