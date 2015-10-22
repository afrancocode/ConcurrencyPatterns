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

		public StoreController(IProductRepository products)
		{
			this.products = products;
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

		public PartialViewResult Edit(Guid id)
		{
			return PartialView(GetProductViewModel(id));
		}

		[HttpPost]
		public ActionResult Edit(ProductViewModel product)
		{
			CoreSave(product);
			return RedirectToAction("Index");
		}

		public PartialViewResult Create()
		{
			return PartialView();
		}

		[HttpPost]
		public ActionResult Create(ProductViewModel product)
		{
			CoreCreate(product);
			return RedirectToAction("Index");
		}

		public PartialViewResult Delete(Guid id)
		{
			return PartialView(GetProductViewModel(id));
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
				filterContext.Controller.TempData["concurrencyError"] = true;
				filterContext.Result = View("Edit");
			}
			base.OnException(filterContext);
		}

		#region Private methods (possible refactoring needed)

		private ProductViewModel GetProductViewModel(Guid id)
		{
			var product = products.FindBy(id);
			var version = product.Version;
			var productView = new ProductViewModel(product.Id, product.Name, product.Description, product.Stock);
			productView.Version = new VersionViewModel(version.Id, version.Value, version.ModifiedBy, version.Modified);
			return productView;
		}

		private Product ActivateProduct(ProductViewModel productView)
		{
			var productModel = Product.Activate(productView.Id, productView.Name, productView.Description, productView.Stock, true);
			var version = ConcurrencyPatterns.Model.Core.Version.Activate(productView.Version.Id, productView.Version.Value, productView.Version.ModifiedBy, productView.Version.Modified);
			//TODO: Date and name will change
			productModel.SetSystemFields(version, DateTime.Now, UserName);
			return productModel;
		}

		private void CoreCreate(ProductViewModel productView)
		{
			var newProduct = Product.Create(productView.Name, productView.Description, productView.Stock);
			products.Add(newProduct);
			this.ManagerContext.UnitOfWork.Commit();
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