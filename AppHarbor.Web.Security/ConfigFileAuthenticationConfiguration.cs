using System;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.Configuration;

namespace AppHarbor.Web.Security
{
	public sealed class ConfigFileAuthenticationConfiguration : ICookieAuthenticationConfiguration
	{
	    private FormsAuthenticationConfiguration _timeout;

		public string CookieName
		{
			get
			{
				return FormsAuthentication.FormsCookieName;
			}
		}

		public bool SlidingExpiration
		{
			get
			{
				return FormsAuthentication.SlidingExpiration;
			}
		}

		public string LoginUrl
		{
			get
			{
				return FormsAuthentication.LoginUrl;
			}
		}

		public string EncryptionAlgorithm
		{
			get
			{
				return "rijndael";
			}
		}

		private static string GetRequiredSetting(string name)
		{
			var setting = ConfigurationManager.AppSettings[name];
			if (setting != null)
			{
				return setting;
			}

			throw new Exception(string.Format("Required setting '{0}' not found.", name));
		}

		public byte[] EncryptionKey
		{
			get
			{
			    return StringExtensions.GetByteArrayFromHexString(GetRequiredSetting("cookieauthentication.encryptionkey"));
				
			}
		}

		public string ValidationAlgorithm
		{
			get
			{
				return "hmacsha256";
			}
		}

		public byte[] ValidationKey
		{
			get
			{
                return StringExtensions.GetByteArrayFromHexString(GetRequiredSetting("cookieauthentication.validationkey"));
			}
		}

		public TimeSpan Timeout
		{
			get
			{
                var authenticationSection = (AuthenticationSection)ConfigurationManager.GetSection("system.web/authentication");
                return authenticationSection.Forms.Timeout;

			}
		}

		public bool RequireSSL
		{
			get
			{
				return FormsAuthentication.RequireSSL;
			}
		}

	}
}
