using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using ConcurrencyPatterns.Infrastructure.Context;
using ConcurrencyPatterns.Infrastructure.Map;
using ConcurrencyPatterns.Infrastructure.Session;
using ConcurrencyPatterns.Model.Users;

namespace ConcurrencyPatterns.Presentation.Web.Infrastructure
{
	public sealed class CookieSession : ISession
	{
		private Guid id = Guid.Empty;
		private Guid ownerId = Guid.Empty;
		private bool initialized;
		private string ownerName;

		public CookieSession()
		{
			initialized = false;
		}

		public Guid Id { get { return this.id; } }

		public Guid Owner { get { return this.ownerId; } }

		public string OwnerName { get { return this.ownerName; } }

		public bool IsLoggedIn { get { return CoreGetCookie() != null; } }

		public void Initialize()
		{
			if (this.initialized) return;
			var cookie = CoreGetCookie();
			if (cookie != null)
			{
				this.id = new Guid(cookie["Id"]);
				var userId = new Guid(cookie["Owner"]);
				this.ownerId = userId;
				this.ownerName = Users.FindBy(userId).Name;
				//this.ownerId = new Guid(cookie["Owner"]);
				this.initialized = true;
			}
		}

		public void InitSession(Guid owner, string ownerName)
		{
			var cookie = CoreGetCookie() ?? CreateCookie(owner);
			Debug.Assert(cookie != null);
			this.id = new Guid(cookie["Id"]);
			this.ownerId = new Guid(cookie["Owner"]);
			this.ownerName = ownerName;
		}

		public void EndSession()
		{
			var cookie = CoreGetCookie();
			if (cookie != null)
			{
				cookie.Expires = DateTime.Now.AddDays(-1);
				HttpContext.Current.Response.Cookies.Add(cookie);
			}
			this.id = Guid.Empty;
			this.ownerId = Guid.Empty;
			this.ownerName = null;
		}

		private IUserRepository Users
		{
			get { return ApplicationContextHolder.Instance.GetService<IUserRepository>(typeof(IUserRepository)); }
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