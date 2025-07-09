using Ans.Net8.Common;
using Guap.Net8.Web.Models;
using Guap.Net8.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Guap.Net8.Web
{

	public static partial class _e
	{

		/* functions */


		public static GuapUserModel[] GetGuapUsers(
			this HttpContext context)
		{
			if (!context.User.Identity.IsAuthenticated)
				return null;
			var provider1 = context.RequestServices
				.GetService<IGuapUsersProvider>();
			return provider1.GetUsers();
		}


		public static RegistryList GetGuapUsersRegistry(
			this HttpContext context)
		{
			var reg1 = new RegistryList(
				context.GetGuapUsers().Select(
					x => new RegistryItem(
						x.Name, x.DisplayedName, 0, false)));
			reg1.AddNullItem();
			return reg1;
		}

	}

}
