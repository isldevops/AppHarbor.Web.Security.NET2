using System;
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

                _authenticator.SetCookie("72201781859E67D4F633C34381EFE4BC928656AEE324A4B00CADA968ACD6CF33047E47479B0B68050FF4A0DB13688B5C78DAFDF53252A94E7F1D7B58A6FFD95D747F3D3AA761DECA7B6358A2E78B85D868833A9420316BDA8A5A0425D543AC1148CB69B902195C20065446A5E5F7A8E4C94A04A22304680E1211F00A12DF5E8777A343D08D0F8C0A3BFC471381E9B070E0F0608ADAEBCA8E233A21251BF57A03B52C1F03B7169CFC7C98216E7217EA649C4EDBD35E07F11A2444D40BE303BFFA28BAA921CDCC298D09A6E0297ED7D6E8");

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
