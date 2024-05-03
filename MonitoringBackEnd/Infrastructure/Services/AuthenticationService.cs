using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static System.Net.WebRequestMethods;

public static class AuthenticationService
{
	public static IServiceCollection AddOAuthAuthentication(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddAuthentication(options =>
		{
			options.DefaultScheme = "Cookies";
			options.DefaultChallengeScheme = "OAuth";
		})
		.AddCookie("Cookies")
		.AddOAuth("OAuth", options =>
		{
			options.ClientId = configuration["AzureAD:ClientId"];
			options.ClientSecret = configuration["AzureAD:ClientSecret"];
			options.CallbackPath = "/auth/callback";

			var tenantId = configuration["AzureAD:TenantId"];
			var clientId = configuration["AzureAD:ClientId"];
			var redirectUri = "http://localhost/auth/callback";

			options.AuthorizationEndpoint = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/authorize?client_id={clientId}&response_type=code&redirect_uri={redirectUri}&scope=https://service.flow.microsoft.com//.default";
			options.TokenEndpoint = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token";
			options.SaveTokens = true;

			options.Events = new OAuthEvents
			{
				OnCreatingTicket = async context =>
				{
					// Access user info, set claims, etc.
				}
			};
		});

		return services;
	}
}