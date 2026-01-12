using Guap.Net8.Web.Models;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace Guap.Net8.Web.Services
{

	/*
	 *	LibStartup.Add_GuapNet8Web()
	 *		_addSsoAuthentication()
	 *			if (options.Users != null)
	 *				builder.Services.AddScoped
	 *					<IGuapUsersProvider, GuapUsersProvider_AppSettings>();
	 */



	public class GuapUsersProvider_AppSettings(
		IConfiguration configuration)
		: IGuapUsersProvider
	{

		private readonly LibGuapOptions _options
			= configuration.GetLibGuapOptions();


		/* functions */


		public GuapUserModel[] GetUsers()
		{
			var users1 = _options.Users?.Select(x => new GuapUserModel
			{
				NameIdentifier = x.NameIdentifier,
				DisplayedName = x.DisplayedName,
			});
			return [.. users1];
		}


		public GuapUserProfileModel GetUserProfile(
			string application,
			string nameIdentifier)
		{
			var user1 = _options.Users?.FirstOrDefault(x => x.NameIdentifier == nameIdentifier);
			if (user1 == null)
				return null;
			var profile1 = new GuapUserProfileModel
			{
				NameIdentifier = user1.NameIdentifier,
				Right = user1.Right,
				Roles = user1.Roles,
				Actions = user1.Actions,
				Resources = user1.Resources,
			};
			Debug.WriteLine($"GuapUsersProvider_AppSettings.GetUserProfile: \"{profile1.NameIdentifier}\"");
			return profile1;
		}


		/* methods */


		public void UpdateUser(
			string nameIdentifier,
			string idUsername,
			string name,
			string surname,
			string givenName,
			string email,
			string displayedName)
		{
			Debug.WriteLine($"GuapUsersProvider_AppSettings.UpdateUser(\"{nameIdentifier}\", \"{idUsername}\", \"{name}\", \"{surname}\", \"{givenName}\", \"{email}\", \"{displayedName}\")");
		}

	}

}
