using System.Text.Json;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Infrastructure.IServices;

public class TokenManagementService : ITokenManagementService
{
	private readonly HttpClient _httpClient;
	private string _accessToken;
	private string _refreshToken;

	public TokenManagementService()
	{
		_httpClient = new HttpClient();
	}

	public async Task<string> GetAccessTokenAsync()
	{
		if (IsTokenExpired(_accessToken))
		{
			await RefreshAccessTokenAsync();
		}

		return _accessToken;
	}

	public async Task<string> GetTokensAsync()
	{
		var parameters = new Dictionary<string, string>
		{
			{ "client_id", Environment.GetEnvironmentVariable("CLIENT_ID") },
			{ "client_secret", Environment.GetEnvironmentVariable("CLIENT_SECRET") },
			{ "username", Environment.GetEnvironmentVariable("API_USERNAME") },
			{ "password", Environment.GetEnvironmentVariable("API_PASSWORD") },
			{ "grant_type", "password" },
			{ "scope", "https://service.flow.microsoft.com//.default offline_access" }
		};

        var content = new FormUrlEncodedContent(parameters);
		var response = await _httpClient.PostAsync($"https://login.microsoftonline.com/{Environment.GetEnvironmentVariable("TENANT_ID")}/oauth2/v2.0/token", content);

		if (!response.IsSuccessStatusCode)
		{
			throw new Exception("Failed to acquire token. StatusCode: " + response);
		}

		var responseContent = await response.Content.ReadAsStringAsync();
		var tokenJson = JsonDocument.Parse(responseContent);

		_accessToken = tokenJson.RootElement.GetProperty("access_token").GetString();
		_refreshToken = tokenJson.RootElement.GetProperty("refresh_token").GetString();

		return _accessToken;
	}

	public async Task<string> RefreshAccessTokenAsync()
	{
		if (_refreshToken == null)
		{
			return await GetTokensAsync();
		}

		var parameters = new Dictionary<string, string>
		{
			{ "client_id", Environment.GetEnvironmentVariable("CLIENT_ID") },
			{ "client_secret", Environment.GetEnvironmentVariable("CLIENT_SECRET") },
			{ "refresh_token", _refreshToken },
			{ "grant_type", "refresh_token" },
			{ "scope", "https://service.flow.microsoft.com//.default" },
		};

		var content = new FormUrlEncodedContent(parameters);

		var response = await _httpClient.PostAsync($"https://login.microsoftonline.com/{Environment.GetEnvironmentVariable("TENANT_ID")}/oauth2/v2.0/token", content);

		if (!response.IsSuccessStatusCode)
		{
			return await GetTokensAsync();
		}

		var responseContent = await response.Content.ReadAsStringAsync();
		var tokenJson = JsonDocument.Parse(responseContent);

		_accessToken = tokenJson.RootElement.GetProperty("access_token").GetString();
		_refreshToken = tokenJson.RootElement.GetProperty("refresh_token").GetString();

		return _accessToken;
	}

	private bool IsTokenExpired(string accessToken)
	{
		if (accessToken == null)
		{
			return true;
		}

		var handler = new JwtSecurityTokenHandler();
		var jsonToken = handler.ReadToken(accessToken) as JwtSecurityToken;

		var expirationDateUtc = jsonToken.ValidTo;

		TimeSpan cetOffset = TimeSpan.FromHours(2);

		if (TimeZoneInfo.Local.IsDaylightSavingTime(expirationDateUtc))
		{
			cetOffset = TimeSpan.FromHours(2);
		}
		else
		{
			cetOffset = TimeSpan.FromHours(1);
		}

		DateTime cetExpirationDate = expirationDateUtc.Add(cetOffset);

		DateTime cetCurrentTime = DateTime.UtcNow.Add(cetOffset);

		return cetExpirationDate < cetCurrentTime;
	}
}