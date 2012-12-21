﻿using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AppHarbor.Web.Security;
using AuthenticationExample.Web.Model;
using AuthenticationExample.Web.PersistenceSupport;
using AuthenticationExample.Web.ViewModels;

namespace AuthenticationExample.Web.Controllers
{
	public class UserController : Controller
	{
		private IAuthenticator _authenticator;
        private IRepository _repository;

	    public UserController()
	    {
	        
	    }

		public UserController(IAuthenticator authenticator, IRepository repository)
		{
			_authenticator = authenticator;
			_repository = repository;
		}

		[HttpGet]
		public ActionResult New()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Create(UserInputModel userInputModel)
		{
            _repository = new InMemoryRepository();
            _authenticator = new CookieAuthenticator();

			if (_repository.GetAll<User>().Any(x => x.Username == userInputModel.Username))
			{
				ModelState.AddModelError("Username", "Username is already in use");
			}

			if (ModelState.IsValid)
			{
				var user = new User
				{
					Id = Guid.NewGuid(),
					Username = userInputModel.Username,
					Password = HashPassword(userInputModel.Password),
				};

				_repository.SaveOrUpdate(user);

				_authenticator.SetCookie(user.Username);

				return RedirectToAction("Index", "Home");
			}

			return View("New", userInputModel);
		}

		[HttpGet]
		[Authorize]
		public ActionResult Show()
		{
			var user = _repository.GetAll<User>().SingleOrDefault(x => x.Username == User.Identity.Name);
			if (user == null)
			{
				throw new HttpException(404, "Not found");
			}

			return View(user);
		}

		private static string HashPassword(string value)
		{
			string salt = BCrypt.Net.BCrypt.GenerateSalt();
			return BCrypt.Net.BCrypt.HashPassword(value, salt);
		}
	}
}
