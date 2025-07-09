using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Guap.Net8.Web.Areas.Guap.Pages.SSO
{

	public class AccessDeniedModel
		: PageModel
	{

		public void OnGet()
		{
			HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
		}

	}

}
