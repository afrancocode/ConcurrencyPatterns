using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ConcurrencyPatterns.Infrastructure.Context;
using ConcurrencyPatterns.Infrastructure.Session;
using ConcurrencyPatterns.Model.Products;
using ConcurrencyPatterns.Model.Users;

namespace ConcurrencyPatterns.Presentation.Web.Controllers
{
	public class HomeController : Controller
	{
		private IUserRepository repo;
		private IProductRepository products;

		protected IManagerContext ManagerContext { get { return ApplicationContextHolder.Instance.Context; } }

		public HomeController(IUserRepository repo, IProductRepository products)
		{
			this.repo = repo;
			this.products = products;
		}

		public string GetUserInfo()
		{
			var id = new Guid("5ed0e574-8093-4020-ab9e-039bb8e19853");
			var entity = repo.FindBy(id);
			return string.Format("Id = '{0}' Name = {1}", entity.Id, entity.Name);
		}

		public string CreateProduct()
		{
			var product = Product.Create("My Product 02", "Product description 02", 100, "sysadmin");
			products.Add(product);
			this.ManagerContext.UnitOfWork.Commit();
			return product.Id.ToString();
		}

		public string GetProduct()
		{
			var id = new Guid("5ecddc56-3c95-4249-af07-15d97e5e1202");
			var product = products.FindBy(id);
			return string.Format("Id = '{0}' Name = '{1}' Description = '{2}'", product.Id, product.Name, product.Description);
		}

		public string EditProduct()
		{
			var id = new Guid("5ecddc56-3c95-4249-af07-15d97e5e1202");
			var product = products.FindBy(id);
			product.Name = "New Name";
			products.Save(product);
			this.ManagerContext.UnitOfWork.Commit();
			return "Saved";
		}
	}
}