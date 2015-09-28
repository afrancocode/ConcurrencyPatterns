using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ConcurrencyPatterns.Model.Products;

namespace ConcurrencyPatterns.Presentation.Web.Controllers
{
	public class StoreController : Controller
	{
		private IProductRepository products;

		public StoreController(IProductRepository products)
		{
			this.products = products;
		}
		//
		// GET: /Store/
		public ActionResult Index()
		{
			var list = products.FindAll().ToList();
			return View(list);
		}

		public ActionResult Details(Guid id)
		{
			var product = products.FindBy(id);
			return View(product);
		}

		public ActionResult Edit(Guid id)
		{
			var product = products.FindBy(id);
			return View(product);
		}
	}
}