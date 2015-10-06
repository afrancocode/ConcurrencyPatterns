using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ConcurrencyPatterns.Presentation.Web.Controllers
{
	public sealed class CookieFilterAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);
			if (!IsLoggedIn())
				filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Home" }, { "action", "Index" } });
		}

		private bool IsLoggedIn()
		{
			return HttpContext.Current.Request.Cookies.AllKeys.Contains("SessionInfo");
		}
	}

	public class BaseController : Controller { }
}