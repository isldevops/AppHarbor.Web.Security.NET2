using System;
using System.Web;

namespace AppHarbor.Web.Security
{
	public sealed class CookieAuthenticator : IAuthenticator
	{
		private readonly ICookieAuthenticationConfiguration _configuration;
		private readonly HttpContext _context;
		
		public CookieAuthenticator()
			: this(new ConfigFileAuthenticationConfiguration(), HttpContext.Current)
		{
		}

		public CookieAuthenticator(ICookieAuthenticationConfiguration configuration, HttpContext context)
		{
			_configuration = configuration;
			_context = context;
		}

        public void SetCookie(string username)
        {
            SetCookie(username,false,null, null);
        }
		
		public void SetCookie(string username, bool persistent , string[] roles, byte[] tag)
		{
			var cookie = new AuthenticationCookie(0, Guid.NewGuid(), persistent, username, roles, tag);
			using (var protector = new CookieProtector(_configuration))
			{
				var httpCookie = new HttpCookie(_configuration.CookieName, protector.Protect(cookie.Serialize()))
				{
					HttpOnly = true,
					Secure = _configuration.RequireSSL,
                    Domain = _configuration.Domain
				};
				if (!persistent)
				{
					httpCookie.Expires = cookie.IssueDate + _configuration.Timeout;
				}

				_context.Response.Cookies.Add(httpCookie);
			}
		}

		public void SignOut()
		{
			_context.Response.Cookies.Remove(_configuration.CookieName);
			_context.Response.Cookies.Add(new HttpCookie(_configuration.CookieName, "")
			{
				Expires = DateTime.UtcNow.AddMonths(-100),
			});
		}


	}
}
