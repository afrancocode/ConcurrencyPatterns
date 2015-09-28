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
	public sealed class CookieFilterAttribute : ActionFilterAttribute
	{
		private IManagerContext ManagerContext { get { return ApplicationContextHolder.Instance.Context; } }
	}

	[CookieFilter]
	public class HomeController : Controller
	{
		private IUserRepository repo;
		private IProductRepository products;
		private CookieSession cookie;

		protected IManagerContext ManagerContext { get { return ApplicationContextHolder.Instance.Context; } }

		public HomeController(IUserRepository repo, IProductRepository products)
		{
			this.repo = repo;
			this.products = products;
			this.cookie = new CookieSession();
		}

		public ActionResult Index()
		{
			var actualCookie = cookie.GetCookie();
			if (actualCookie != null)
			{
				var userId = new Guid(actualCookie["Owner"]);
				var user = repo.FindBy(userId);
				ViewBag.UserName = user.Name;
				return View("Home");
			}
			return View(repo.FindAll().ToList());
		}

		public ActionResult Home()
		{
			return View();
		}

		public ActionResult LoginUser(Guid id)
		{
			var user = repo.FindBy(id);
			if (user == null)
			{
				//TODO: Define error that will be displayed
				return HttpNotFound();
			}
			return LoginUser(user);
		}

		public ActionResult Logout()
		{
			cookie.DeleteCookie();
			return RedirectToAction("Index", "Home");
		}

		public ActionResult Store()
		{
			return RedirectToRoute(new { controller = "Store", action = "index" });
		}

		private ActionResult LoginUser(User user)
		{
			if (ModelState.IsValid)
			{
				Debug.Assert(cookie != null);
				cookie.Initialize(user.Id);
			}
			ViewBag.Username = user.Name;
			return View("Home");
		}
	}
}