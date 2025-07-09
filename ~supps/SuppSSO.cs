using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace Guap.Net8.Web
{

	public static class SuppSSO
	{

		public static SignOutResult GetSignOutResult()
		{
			return new SignOutResult([
				OpenIdConnectDefaults.AuthenticationScheme,
				CookieAuthenticationDefaults.AuthenticationScheme
			]);
		}

	}

}
