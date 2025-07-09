using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Guap.Net8.Web.Areas.Guap.Pages.SSO
{

	[Authorize]
	public class ProfileModel
		: PageModel
	{

		public void OnGet()
		{
			HttpContext.Response.StatusCode = StatusCodes.Status200OK;
		}

	}

}
