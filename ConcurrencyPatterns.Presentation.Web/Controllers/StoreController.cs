using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ConcurrencyPatterns.Infrastructure.Context;
using ConcurrencyPatterns.Infrastructure.Locking;
using ConcurrencyPatterns.Model.Products;
using ConcurrencyPatterns.Presentation.Web.Models;

namespace ConcurrencyPatterns.Presentation.Web.Controllers
{
	[CookieFilter]
	public class StoreController : BaseController
	{
		private IProductRepository products;
		private IManagerContext ManagerContext { get { return ApplicationContextHolder.Instance.Context; } }

		public StoreController(IProductRepository products)
		{
			this.products = products;
			//TODO: Check if this is correct
			ManagerContext.Session.Initialize();
		}
		//
		// GET: /Store/
		public ActionResult Index()
		{
			var list = products.FindAll().ToList().OrderBy(x => x.Name);
			return View(list);
		}

		public ActionResult Details(Guid id)
		{
			var product = products.FindBy(id);
			return View(product);
		}

		public ActionResult Edit(Guid id)
		{
			return View(GetProductViewModel(id));
		}

		[HttpPost]
		public ActionResult Edit(ProductViewModel product)
		{
			CoreSave(product);
			return RedirectToAction("Index");
		}

		public ActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Create(ProductViewModel product)
		{
			CoreCreate(product);
			return RedirectToAction("Index");
		}

		public ActionResult Delete(Guid id)
		{
			return View(GetProductViewModel(id));
		}

		[HttpPost]
		public ActionResult Delete(ProductViewModel productViewModel)
		{
			CoreDelete(productViewModel);
			return RedirectToAction("Index");
		}

		protected override void OnException(ExceptionContext filterContext)
		{
			if (filterContext.ExceptionHandled)
				return;
			Exception e = filterContext.Exception;
			if (e is ConcurrencyException)
			{
				filterContext.ExceptionHandled = true;
				filterContext.Result = View("Edit");
			}
			base.OnException(filterContext);
		}

		#region Private methods (possible refactoring needed)

		private string UserName { get { return ManagerContext.Session.OwnerName; } }

		private ProductViewModel GetProductViewModel(Guid id)
		{
			var product = products.FindBy(id);
			var version = product.Version;
			var productView = new ProductViewModel(product.Id, product.Name, product.Description, product.Stock);
			productView.Version = new VersionViewModel(version.Id, version.Value, version.ModifiedBy, version.Modified);
			return productView;
		}

		private Product ActivateProduct(ProductViewModel product)
		{
			var productModel = Product.Activate(product.Id, product.Name, product.Description, product.Stock, true);
			var version = ConcurrencyPatterns.Model.Core.Version.Activate(product.Version.Id, product.Version.Value, product.Version.ModifiedBy, product.Version.Modified);
			//TODO: Date and name will change
			productModel.SetSystemFields(version, DateTime.Now, UserName);
			return productModel;
		}

		private void CoreCreate(ProductViewModel product)
		{
			var newProduct = Product.Create(product.Name, product.Description, product.Stock);
			try
			{
				products.Add(newProduct);
				this.ManagerContext.UnitOfWork.Commit();
			}
			catch
			{
				throw;
			}
		}

		private void CoreSave(ProductViewModel editedProduct)
		{
			var product = ActivateProduct(editedProduct);
			products.Save(product);
			this.ManagerContext.UnitOfWork.Commit();
		}

		private void CoreDelete(ProductViewModel deletedProduct)
		{
			var product = ActivateProduct(deletedProduct);
			products.Remove(product);
			this.ManagerContext.UnitOfWork.Commit();

		}

		#endregion
	}
}