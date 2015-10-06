using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using ConcurrencyPatterns.Infrastructure.Context;
using ConcurrencyPatterns.Infrastructure.Session;
using ConcurrencyPatterns.Model.Users;

namespace ConcurrencyPatterns.Presentation.Web.Infrastructure
{
	public class LoginManager
	{
		private IUserRepository repo;
		private IManagerContext ManagerContext { get { return ApplicationContextHolder.Instance.Context; } }
		private ISession Session { get { return ManagerContext.Session; } }

		public LoginManager(IUserRepository repo)
		{
			this.repo = repo;
			ManagerContext.Session.Initialize();
		}

		public void Login(Guid id)
		{
			CoreLogin(id);
		}

		public bool IsLoggedIn()
		{
			return Session.IsLoggedIn;
		}

		public void Logout()
		{
			CoreLogout();
		}

		private void CoreLogin(Guid id)
		{
			var user = GetUser(id);
			ManagerContext.Session.InitSession(user.Id, user.Name);
		}

		private void CoreLogout()
		{
			Session.EndSession();
		}

		private User GetUser(Guid id)
		{
			var user = repo.FindBy(id);
			if (user == null)
			{
				//TODO: ERROR
			}
			return user;
		}
	}
}