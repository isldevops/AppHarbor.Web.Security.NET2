﻿using System;
using System.Linq;
using System.Web.Mvc;
using AppHarbor.Web.Security;
using AuthenticationExample.Web.Model;
using AuthenticationExample.Web.PersistenceSupport;
using AuthenticationExample.Web.ViewModels;

namespace AuthenticationExample.Web.Controllers
{
	public class SessionController : Controller
	{
		private IAuthenticator _authenticator;
		private IRepository _repository;
		private const string errorMessage = "Invalid username or password";

	    public SessionController()
	    {
	        
	    }

		public SessionController(IAuthenticator authenticator, IRepository repository)
		{
			_authenticator = authenticator;
			_repository = repository;
		}

		[HttpGet]
		public ActionResult New(string returnUrl)
		{
			return View(new SessionViewModel { ReturnUrl = returnUrl });
		}

		[HttpPost]
		public ActionResult Create(SessionViewModel sessionViewModel)
		{
			User user = null;
            _repository = new InMemoryRepository();
            _authenticator = new CookieAuthenticator();
			if (ModelState.IsValid)
			{
				user = _repository.GetAll<User>().SingleOrDefault(x => x.Username == sessionViewModel.Username);
				if (user == null)
				{
					ModelState.AddModelError(string.Empty, errorMessage);
				}
			}

			if (ModelState.IsValid)
			{
				if (!BCrypt.Net.BCrypt.Verify(sessionViewModel.Password, user.Password))
				{
					ModelState.AddModelError(string.Empty, errorMessage);
				}
			}

			if (ModelState.IsValid)
			{
				_authenticator.SetCookie(user.Username);
				var returnUrl = sessionViewModel.ReturnUrl;
				if (returnUrl != null)
				{
					Uri returnUri;
					if (Uri.TryCreate(returnUrl, UriKind.Relative, out returnUri))
					{
						return Redirect(sessionViewModel.ReturnUrl);
					}
				}

				return RedirectToAction("Index", "Home");
			}

			return View("New", sessionViewModel);
		}

		[HttpPost]
		public ActionResult Destroy()
		{
			_authenticator.SignOut();
			Session.Abandon();
			return RedirectToAction("Index", "Home");
		}
	}
}
