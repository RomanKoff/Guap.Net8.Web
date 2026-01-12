using Ans.Net8.Web;
using Guap.Net8.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using NLog;
using NLog.Web;
using System.Diagnostics;

namespace Guap.Net8.Web
{

	public static class LibGuapStartup
	{

		/* methods */


		public static void Init_GuapNet8Web(
			Action<WebApplicationBuilder> builderAction,
			Action<WebApplication> applicationAction)
		{
			var logger1 = LogManager.Setup()
				.LoadConfigurationFromAppSettings()
				.GetCurrentClassLogger();
			logger1.Info("APP: Init");

			try
			{

				/*
				 * Builder
				 */

				var builder1 = WebApplication.CreateBuilder();
				var configuration1 = builder1.Configuration;
				var ans1 = configuration1.GetLibWebOptions();
				var guap1 = configuration1.GetLibGuapOptions();

				/* nlog */
				builder1.Logging.ClearProviders();
				builder1.Host.UseNLog();

				/* ans */
				builder1.Add_AnsNet8Web(configuration1);

				/* guap */
				builder1.Add_GuapNet8Web(configuration1);

				/* db */
				//AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
				//builder1.Services.AddDbContext<AppDbContext>(o =>
				//{
				//	o.ConfigureWarnings(x => x.Ignore(RelationalEventId.BoolWithDefaultWarning));
				//	o.UseNpgsql(builder1.Configuration.GetConnectionString("AppDbConnection"));
				//});

				builderAction?.Invoke(builder1);

				/*
				 * Application
				 */

				var app1 = builder1.Build();

				/* ans */
				app1.Use_AnsNet8Web(configuration1);

				/* guap */
				app1.Use_GuapNet8Web(configuration1);

				//* db */
				//app1.AppDbContext_Prepare(null);

				applicationAction?.Invoke(app1);

				app1.Run();

			}
			catch (Exception exception)
			{
				logger1.Error(exception, "APP: Stopped program because of exception");
				throw;
			}
			finally
			{
				LogManager.Shutdown();
			}
		}


		public static void Add_GuapNet8Web(
			this WebApplicationBuilder builder,
			IConfiguration configuration)
		{
			var options1 = configuration.GetLibGuapOptions();

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
			var options1 = configuration.GetLibGuapOptions();

			if (options1.Sso != null)
				app.UseAuthorization();
		}


		/* privates */


		private static void _addSsoAuthentication(
			WebApplicationBuilder builder,
			LibGuapOptions options)
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
