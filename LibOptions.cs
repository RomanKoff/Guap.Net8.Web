using Ans.Net8.Common;
using Guap.Net8.Web.Models;
using Microsoft.Extensions.Configuration;

namespace Guap.Net8.Web
{

	public static partial class _e
	{
		private static LibOptions _libOptions;
		public static LibOptions GetOptions_GuapNet8Web(
			this IConfiguration configuration)
		{
			return _libOptions ??= configuration.GetOptions<LibOptions>("Guap.Net8.Web");
		}
	}



	public class LibOptions
		: _AppSettingsOptions_Proto
	{
		public override void Test()
		{
			if (AppName == null)
				throw GetExceptionParamRequired(nameof(AppName));
			if (AppTitle == null)
				throw GetExceptionParamRequired(nameof(AppTitle));
		}

		public string AppName { get; set; }
		public string AppTitle { get; set; }
		public string HostDocs { get; set; }
		public SsoOptions Sso { get; set; }
		public AppUsers[] Users { get; set; }
		public AuthApi AuthApi { get; set; }
	}



	public class SsoOptions
	{
		public string CookieName { get; set; }
		public bool RequireHttpsMetadata { get; set; }
		public string Authority { get; set; }
		public string ClientId { get; set; }
		public string ClientSecret { get; set; }
	}



	public class AppUsers
		: IGuapUserProfile
	{
		public string NameIdentifier { get; set; }
		public string DisplayedName { get; set; }
		public int Right { get; set; }
		public string Roles { get; set; }
		public string Actions { get; set; }
		public string Resources { get; set; }
	}



	public class AuthApi
	{
		public string Url { get; set; }
	}

}
