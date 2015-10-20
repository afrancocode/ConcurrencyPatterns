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
using ConcurrencyPatterns.Presentation.Web.Infrastructure;

namespace ConcurrencyPatterns.Presentation.Web.Controllers
{
	public class HomeController : Controller
	{
		private IUserRepository repo;
		private IProductRepository products;
		private LoginManager login;
		private IManagerContext ManagerContext { get { return ApplicationContextHolder.Instance.Context; } }

		public HomeController(IUserRepository repo, IProductRepository products)
		{
			this.repo = repo;
			this.products = products;
			this.login = new LoginManager(repo);
		}

		public ActionResult Index()
		{
			if (login.IsLoggedIn())
			{
				ViewBag.UserName = ManagerContext.Session.OwnerName;
				//return RedirectToAction("Index","store");
				return RedirectToAction("Home");
			}
			return View(repo.FindAll().ToList());
		}

		public ActionResult Home()
		{
			return View();
		}

		public ActionResult LoginUser(Guid id)
		{
			login.Login(id);
			ViewBag.Username = ManagerContext.Session.OwnerName;
			return RedirectToAction("Home");
		}

		public ActionResult Logout()
		{
			login.Logout();
			return RedirectToAction("Index", "Home");
		}

		public ActionResult Store()
		{
			return RedirectToAction("Index", "Store");
		}
	}
}