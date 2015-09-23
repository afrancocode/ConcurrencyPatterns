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
		private Guid owner = Guid.Empty;
		private bool initialized;

		public CookieSession()
		{
			initialized = false;
		}

		public Guid Id { get { Initialize(); return this.id; } }

		public Guid Owner
		{
			get { Initialize(); return this.owner; }
			set { Initialize(); this.SetOwner(value); }
		}

		private void Initialize()
		{
			if (this.initialized) return;
			var cookie = GetCookie() ?? CreateCookie();
			Debug.Assert(cookie != null);
			this.id = new Guid(cookie["Id"]);
			this.owner = new Guid(cookie["Owner"]);
			this.initialized = true;
		}

		private HttpCookie GetCookie()
		{
			var cookies = HttpContext.Current.Request.Cookies;
			if (cookies.AllKeys.Contains("SessionInfo"))
				return cookies["SessionInfo"];
			return null;
		}

		private HttpCookie CreateCookie()
		{
			var cookie = new HttpCookie("SessionInfo");
			cookie.Values.Add("Id", Guid.NewGuid().ToString());
			cookie.Values.Add("Owner", owner.ToString()); // Initialize later
			HttpContext.Current.Response.Cookies.Add(cookie);
			return cookie;
		}

		private void SetOwner(Guid owner)
		{
			if (this.owner == Guid.Empty)
			{
				var cookie = HttpContext.Current.Response.Cookies["SessionInfo"];
				cookie.Values.Set("Owner", owner.ToString());
				this.owner = owner;
			}
		}
	}
}