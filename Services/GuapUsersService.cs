using Ans.Net8.Common;
using System.Diagnostics;
using System.Security.Claims;

namespace Guap.Net8.Web.Services
{

	/*
	 *	LibStartup.Add_GuapNet8Web()
	 *		_addSsoAuthentication()
	 *			builder.Services.AddSingleton<IUsersService, GuapUsersService>();
	 */



	public interface IUsersService
	{
		void TicketReceived(ClaimsPrincipal principal);
		void SignedOutCallbackRedirect(ClaimsPrincipal principal);
	}



	public class GuapUsersService(
		IGuapUsersProvider users)
		: IUsersService
	{

		private readonly IGuapUsersProvider _users = users;


		/* methods */


		public void TicketReceived(
			ClaimsPrincipal principal)
		{
			var name1 = principal.GetNameFromClaim();
			var id1 = principal.GetNameIdentifierFromClaim();
			Debug.WriteLine($"Login: \"{name1} ({id1})\"");
			_users.UpdateUser(
				id1,
				principal.GetIdUsernameFromClaim(),
				principal.Identity.Name,
				principal.GetSurnameFromClaim(),
				principal.GetGivenNameFromClaim(),
				principal.GetEmailFromClaim(),
				name1);
		}


		public void SignedOutCallbackRedirect(
			ClaimsPrincipal principal)
		{
			Debug.WriteLine($"Logout: \"{principal.GetNameFromClaim()} ({principal.GetNameIdentifierFromClaim()})\"");
		}

	}

}
