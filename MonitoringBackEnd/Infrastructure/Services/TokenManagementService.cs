using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Net.Http.Headers;

public class TokenManagementService
{
	private readonly IConfiguration _configuration;
	private readonly HttpClient _httpClient;
	private string _accessToken;
	private string _refreshToken;

	public TokenManagementService(IConfiguration configuration)
	{
		_configuration = configuration;
		_httpClient = new HttpClient();
		_accessToken = _configuration["AzureAD:AccessToken"];
		_refreshToken = _configuration["AzureAD:RefreshToken"];
	}

	public async Task<string> GetAccessTokenAsync()
	{
		if (IsTokenExpired(_accessToken))
		{
			await RefreshAccessTokenAsync();
		}

		return _accessToken;
	}

	public async Task<string> RefreshAccessTokenAsync()
	{
		if (_refreshToken == null)
		{
			throw new Exception("No refresh token available.");
		}

		var parameters = new Dictionary<string, string>
		{
			{ "client_id", _configuration["AzureAD:ClientId"] },
			{ "client_secret", _configuration["AzureAD:ClientSecret"] },
			{ "refresh_token", _refreshToken },
			{ "grant_type", "refresh_token" },
			{ "scope", "https://service.flow.microsoft.com//.default" },
		};

		var content = new FormUrlEncodedContent(parameters);

		var response = await _httpClient.PostAsync($"https://login.microsoftonline.com/{_configuration["AzureAD:TenantId"]}/oauth2/v2.0/token", content);

		if (!response.IsSuccessStatusCode)
		{
			throw new Exception("Failed to refresh token. StatusCode: " + response.StatusCode + ", ReasonPhrase: " + response.ReasonPhrase);
		}

		var responseContent = await response.Content.ReadAsStringAsync();
		var tokenJson = JsonDocument.Parse(responseContent);

		_accessToken = tokenJson.RootElement.GetProperty("access_token").GetString();
		_refreshToken = tokenJson.RootElement.GetProperty("refresh_token").GetString();

		return _accessToken;
	}

	public void SetTokens(string accessToken, string refreshToken)
	{
		_accessToken = accessToken;
		_refreshToken = refreshToken;
	}

	private bool IsTokenExpired(string accessToken)
	{
		return string.IsNullOrEmpty(accessToken);
	}
}