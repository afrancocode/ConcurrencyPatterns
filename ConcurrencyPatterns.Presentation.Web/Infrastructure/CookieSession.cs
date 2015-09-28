using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

using ConcurrencyPatterns.Infrastructure.Map;
using ConcurrencyPatterns.Infrastructure.Session;

namespace ConcurrencyPatterns.Presentation.Web.Infrastructure
{
	public sealed class CookieSession : ISession
	{
		private Guid id = Guid.Empty;
		private Guid ownerId = Guid.Empty;
		private bool initialized;

		public CookieSession()
		{
			initialized = false;
		}

		public Guid Id { get { return this.id; } }

		public Guid Owner { get { return this.ownerId; } }

		public void Initialize(Guid owner)
		{
			if (this.initialized) return;
			var cookie = CoreGetCookie() ?? CreateCookie(owner);
			Debug.Assert(cookie != null);
			this.id = new Guid(cookie["Id"]);
			this.ownerId = new Guid(cookie["Owner"]);
			this.initialized = true;
		}

		public void DeleteCookie()
		{
			var cookie = CoreGetCookie();
			if (cookie != null)
			{
				cookie.Expires = DateTime.Now.AddDays(-1);
				HttpContext.Current.Response.Cookies.Add(cookie);
			}
		}

		public HttpCookie GetCookie()
		{
			return CoreGetCookie();
		}

		private HttpCookie CoreGetCookie()
		{
			var cookies = HttpContext.Current.Request.Cookies;
			if (cookies.AllKeys.Contains("SessionInfo"))
				return cookies["SessionInfo"];
			return null;
		}

		private HttpCookie CreateCookie(Guid owner)
		{
			var cookie = new HttpCookie("SessionInfo");
			cookie.Values.Add("Id", Guid.NewGuid().ToString());
			cookie.Values.Add("Owner", owner.ToString());
			cookie.Expires = DateTime.Now.AddHours(1); // Temporal date
			HttpContext.Current.Response.AppendCookie(cookie);
			return cookie;
		}

		private void SetOwner(Guid owner)
		{
			if (this.ownerId == Guid.Empty)
			{
				var cookie = HttpContext.Current.Response.Cookies["SessionInfo"];
				cookie.Values.Set("Owner", owner.ToString());
				this.ownerId = owner;
			}
		}
	}
}