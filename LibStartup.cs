using Ans.Net8.Web;
using Guap.Net8.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Diagnostics;

namespace Guap.Net8.Web
{

	public static class LibStartup
	{

		/* methods */


		public static void Add_GuapNet8Web(
			this WebApplicationBuilder builder,
			IConfiguration configuration)
		{
			var options1 = configuration.GetOptions_GuapNet8Web();

			// IServiceCollection

			if (options1.Sso != null)
			{
				_addSsoAuthentication(builder, options1);
				builder.Services.AddAuthorization(o =>
				{
					o.AddAnsPolicies();
				});
			}
		}


		public static void Use_GuapNet8Web(
			this WebApplication app,
			IConfiguration configuration)
		{
			var options1 = configuration.GetOptions_GuapNet8Web();

			if (options1.Sso != null)
				app.UseAuthorization();
		}


		/* privates */


		private static void _addSsoAuthentication(
			WebApplicationBuilder builder,
			LibOptions options)
		{
			if (options.Users != null)
				builder.Services.AddSingleton
					<IGuapUsersProvider, GuapUsersProvider_AppSettings>();
			else if (options.AuthApi != null)
				builder.Services.AddSingleton
					<IGuapUsersProvider, GuapUsersProvider_AuthApi>();

			builder.Services.AddSingleton<IClaimsTransformation, GuapClaimsTransformation>();
			builder.Services.AddSingleton<IUsersService, GuapUsersService>();

			var auth1 = builder.Services.AddAuthentication(o =>
			{
				o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				o.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
			});
			auth1.AddCookie(o =>
			{
				if (options.Sso.CookieName != null)
					o.Cookie.Name = options.Sso.CookieName;
				o.Cookie.MaxAge = TimeSpan.FromMinutes(60);
				o.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
				o.SlidingExpiration = true;
				o.AccessDeniedPath = new PathString("/Guap/SSO/AccessDenied");
			});
			auth1.AddOpenIdConnect(o =>
			{
				o.RequireHttpsMetadata = options.Sso.RequireHttpsMetadata;
				o.Authority = options.Sso.Authority;
				o.ClientId = options.Sso.ClientId;
				o.ClientSecret = options.Sso.ClientSecret;
				o.ResponseType = OpenIdConnectResponseType.CodeIdToken;
				o.GetClaimsFromUserInfoEndpoint = true;
				o.Scope.Add("openid");
				o.Scope.Add("profile");
				o.Scope.Add("roles");
				o.Scope.Add("email");
				o.SaveTokens = true;
				o.TokenValidationParameters.NameClaimType = "preferred_username";
				o.Events = new OpenIdConnectEvents
				{
					// Login events:
					//	OnMessageReceived
					//	OnTokenValidated
					//	OnAuthorizationCodeReceived
					//	OnTokenResponseReceived
					//	OnUserInformationReceived
					OnTicketReceived = x =>
					{
						Debug.WriteLine($"OnTicketReceived '{x.Principal?.Identity?.Name}'");
						var _usersService1 = x.HttpContext.RequestServices
							.GetService<IUsersService>();
						_usersService1?.TicketReceived(x.Principal);
						return Task.CompletedTask;
					},

					// Logout events:
					//	OnAuthorizationCodeReceived
					//	OnRedirectToIdentityProvider
					OnSignedOutCallbackRedirect = x =>
					{
						Debug.WriteLine($"OnSignedOutCallbackRedirect '{x.Principal?.Identity?.Name}'");
						var _usersService1 = x.HttpContext.RequestServices
							.GetService<IUsersService>();
						_usersService1?.SignedOutCallbackRedirect(x.Principal);
						return Task.CompletedTask;
					},

					// Other events:
					//	OnAccessDenied
					//	OnAuthenticationFailed
					//	OnRemoteFailure
					//	OnRemoteSignOut
					//	OnRedirectToIdentityProviderForSignOut
				};
			});
		}

	}

}
