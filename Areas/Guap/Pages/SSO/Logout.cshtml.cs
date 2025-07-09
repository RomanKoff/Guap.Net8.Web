using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Guap.Net8.Web.Areas.Guap.Pages.SSO
{

	[Authorize]
	public class LogoutModel
		: PageModel
	{

		public IActionResult OnGet()
		{
			if (User.Identity.IsAuthenticated)
				return SuppSSO.GetSignOutResult();
			return Redirect("~/");
		}

	}

}
