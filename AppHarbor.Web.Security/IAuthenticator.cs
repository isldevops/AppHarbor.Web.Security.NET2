
namespace AppHarbor.Web.Security
{
	public interface IAuthenticator
	{
        void SetCookie(string username);

		void SetCookie(string username, bool persistent, string[] roles, byte[] tag);
        
		void SignOut();
	}
}
