using Ans.Net8.Common;
using Guap.Net8.Web.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace Guap.Net8.Web.Services
{

	/*
	 *	LibStartup.Add_GuapNet8Web()
	 *		_addSsoAuthentication()
	 *			if (options.AuthApi != null)
	 *				builder.Services.AddScoped
	 *					<IGuapUsersProvider, GuapUsersProvider_AuthApi>();
	 */



	public class GuapUsersProvider_AuthApi(
		IHttpClientFactory httpClientFactory,
		IMemoryCache memoryCache,
		IConfiguration configuration)
		: IGuapUsersProvider
	{

		private readonly HttpClient _httpClient = httpClientFactory.CreateClient();
		private readonly IMemoryCache _memoryCache = memoryCache;
		private readonly LibGuapOptions _options = configuration.GetLibGuapOptions();


		/* functions */


		public GuapUserModel[] GetUsers()
		{
			var url1 = $"{_options.AuthApi.Url}/get-list-users";
			var profile1 = new WebApiCachedHelper<GuapUserModel[]>(
				_httpClient, _memoryCache, url1, url1, null, null)
					.SendQuery().Content;
			return profile1;
		}


		public GuapUserProfileModel GetUserProfile(
			string application,
			string nameIdentifier)
		{
			var application1 = _options.AppName;
			var url1 = $"{_options.AuthApi.Url}/get-user/?application={application1}&nameIdentifier={nameIdentifier}";
			var profile1 = new WebApiCachedHelper<GuapUserProfileModel>(
				_httpClient, _memoryCache, url1, url1, null, null)
					.SendQuery().Content;
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
			var application1 = _options.AppName;
			Debug.WriteLine($"GuapUsersProvider_AuthApi.UpdateUser(\"{application1}\", \"{nameIdentifier}\", \"{idUsername}\", \"{name}\", \"{email}\", \"{displayedName}\")");
			var url1 = $"{_options.AuthApi.Url}/set-user?application={application1}&nameIdentifier={nameIdentifier}&idUsername={idUsername}&name={name}&surname={surname}&givenName={givenName}&email={email}&displayedName={displayedName}";
			var result1 = new WebApiHelper<object>(
				_httpClient, url1, null)
					.SendQuery().Content;
			Debug.WriteLine($"GuapUsersProvider_AuthApi.UpdateUser: '{result1}'");
		}

	}

}
