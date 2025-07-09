using Ans.Net8.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace Guap.Net8.Web.Services
{

	/*
	 *	LibStartup.Add_GuapNet8Web()
	 *		_addSsoAuthentication()
	 *			builder.Services.AddSingleton
	 *				<IClaimsTransformation, GuapClaimsTransformation>();
	 */



	public class GuapClaimsTransformation(
		IGuapUsersProvider users,
		IConfiguration configuration)
		: IClaimsTransformation
	{

		private readonly IGuapUsersProvider _users = users;
		private readonly LibOptions _options = configuration.GetOptions_GuapNet8Web();


		/* functions */


		public Task<ClaimsPrincipal> TransformAsync(
			ClaimsPrincipal principal)
		{
			if (principal.Identity.IsAuthenticated)
			{
				var profile1 = _users.GetUserProfile(
					_options.AppName,
					principal.GetNameIdentifierFromClaim());
				if (profile1 != null)
				{
					// right
					principal.AddClaims(
						Ans.Net8.Web._Consts.CLAIM_AUTH_POLICY_TYPE,
						profile1.Right.ToString());
					// roles
					if (!string.IsNullOrEmpty(profile1.Roles))
						principal.AddClaimsRoles(
							profile1.Roles.Split(';'));
					// actions
					if (!string.IsNullOrEmpty(profile1.Actions))
						principal.AddClaims(
							Ans.Net8.Web._Consts.CLAIM_ACTIONS_TYPE,
							profile1.Actions.Split(';'));
					// props
				}
			}
			return Task.FromResult(principal);
		}

	}

}
