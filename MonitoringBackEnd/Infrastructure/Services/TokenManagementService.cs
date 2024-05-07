using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Azure.Core;

public class TokenManagementService
{
	private readonly IConfiguration _configuration;
	private readonly HttpClient _httpClient;
	private readonly IDictionary<string, TokenData> _tokenStore;
	private readonly string _tenantId;
	private readonly string _clientId;
	private readonly string _clientSecret;

	public TokenManagementService(IConfiguration configuration)
	{
		_configuration = configuration;
		_httpClient = new HttpClient();
		_tokenStore = new Dictionary<string, TokenData>
		{
			{ "dfe7a574-74d2-43c4-ade9-ab097c0cec3f", new TokenData
				{
					AccessToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6IkwxS2ZLRklfam5YYndXYzIyeFp4dzFzVUhIMCIsImtpZCI6IkwxS2ZLRklfam5YYndXYzIyeFp4dzFzVUhIMCJ9.eyJhdWQiOiJodHRwczovL3NlcnZpY2UuZmxvdy5taWNyb3NvZnQuY29tLyIsImlzcyI6Imh0dHBzOi8vc3RzLndpbmRvd3MubmV0L2VlNWZhYjljLWZjMTAtNGZiZi1iOGY2LTM4N2JmZmJiNTE5Ni8iLCJpYXQiOjE3MTQ3NDE0NzgsIm5iZiI6MTcxNDc0MTQ3OCwiZXhwIjoxNzE0NzQ2ODY2LCJhY2N0IjowLCJhY3IiOiIxIiwiYWlvIjoiQVRRQXkvOFdBQUFBQTNMNDhoNStsNkFUQnZBY1c0VTdXTjZQaFZxNTM2cUlSOFFYTVRZTWhzT1pEUjRNaDlRcmgwVFo1TUhPcDgxeSIsImFtciI6WyJwd2QiXSwiYXBwaWQiOiJkZmU3YTU3NC03NGQyLTQzYzQtYWRlOS1hYjA5N2MwY2VjM2YiLCJhcHBpZGFjciI6IjEiLCJmYW1pbHlfbmFtZSI6IjQtVG9wIiwiZ2l2ZW5fbmFtZSI6IkFkbWluIiwiaXBhZGRyIjoiMjAwMTo0YzNjOmVlMDA6MmUwMDo0YzU0OmY2MmQ6MjUzMDo5MTkwIiwibmFtZSI6IkFkbWluIHwgNC1Ub3AubmwiLCJvaWQiOiJjNWZhNmVlYy0yYzgwLTQxMjMtYTUxZS03NDA0Zjk2ZGExZWQiLCJwdWlkIjoiMTAwMzIwMDM2OENCMzkxNSIsInJoIjoiMC5BYThBbkt0ZjdoRDh2MC00OWpoN183dFJsaVdoOEgyLTA1Wk1xbFJaSDRQX1ZCd2RBWDQuIiwic2NwIjoiRmxvd3MuTWFuYWdlLkFsbCIsInNpZ25pbl9zdGF0ZSI6WyJrbXNpIl0sInN1YiI6Im9MNW5qZU1GRGhfN1N1NGJMdFRnYnlLeklwODk2aVV3NnVWNWwxTmVoUWMiLCJ0aWQiOiJlZTVmYWI5Yy1mYzEwLTRmYmYtYjhmNi0zODdiZmZiYjUxOTYiLCJ1bmlxdWVfbmFtZSI6ImFkbWluQDQtdG9wLm5sIiwidXBuIjoiYWRtaW5ANC10b3AubmwiLCJ1dGkiOiJnbDhpWWc2NURVRzJ1c2tlQlRCLUFBIiwidmVyIjoiMS4wIn0.aEnnJOi_4egfQtSMY0UtWP01_mnt6FHZXJFdJuc8cQPn1OOLGHz8IpqnnJWQJwVc7oLE-8K6jVJYhUIDnpi6-7umDQkeKhw8ef82dyK4rkMEKcMv59jxgldhgIH0qQillQpvxCNc456VoVoDAT4Zl4ZIM_QiPu_Z-SL0vXEgmtu2j6grb2frD5Cpz0_d_CtXV2zKeauQayp4GLigpWpBGy97FCH35DSRaGoOYYpkQtJR4H-F_X7QaTDOdGLvjqfpUMRJJZZMtwQJeM47qZenjJKFXlcJ25fSm_om5G13EI3FNjMRGoDE7_adKI2dMPvxSD7gqwH3tbHyUtI6RukIPA",
					RefreshToken = "0.Aa8AnKtf7hD8v0-49jh7_7tRlnSl59_SdMRDremrCXwM7D8dAX4.AgABAwEAAADnfolhJpSnRYB1SVj-Hgd8AgDs_wUA9P8vvqtNBtdPFoS8RH_WzhyVnZ2eDncGk1ItOH91eij-XhmkYXGZAe3v4Nerj_voyN1SCIDkvJtWWGhrBATtJYgVyyGUKdFK8T0Y9BKSYeLR6bcSHpaqd8xPFB8y2KdHVSFfLKMtfRv6pJP7BLGXITloDqQc2wm83GxyXF1DLXhi3htQTxRCQvBV1j7AX0NUR9IKePUntPP-tVZIaG7GykzXrVwxSdwSfbE3z2mPnabTf1cBbdblMTov__vMAxDZrHybV-0qGfUuNCNM3CW_048A0HEw2OGQvIGzayXkhsmyfulyRuFG2LUe2WERwa0HFOSGML3bzgZBiTZpzeMG6NJ9CeM0XJ9jGd1F9bHlZtpZ9JcR2R8I_1xPAx3roQeUumDaPSITyuZzew-gPsVbxW93rmalgEI28NY93evP_uFt6ZJMboM9FrRPU_4pgjkwrNX0_971UJ14DFnJRj0PynJlYnM9hYTfiL7s4xxXs5E3gB83CCVT8mLLB_cm-HL-grybx3j10926fNNNx7TF4XZfkbGLTmFofdxeDXsdtJRkzu4SAL6c6UgeukR0XqxCI0gf-MMNur-IuSUUZ2fnZoehwTXxgE5axHt_EqFBlN37_TxEherAQqx7bW5nf9t5NnMd_uXi7PTLaYrDL7G_gOLtxaZRSmqIKS2nApAAXM-IdnipqJSWczirOQdMZ5gy34f8F5-3DdxPHBUGg3aMpeV1heY4TQf6liA4MQqsgyJeuNrxuTSUucLtJrC0Nv3lxz8I-GREyQLbdmE",
					Expiry = DateTime.UtcNow.AddSeconds(2000)
				} 
			}
		};

		_tenantId = _configuration["AzureAD:TenantId"];
		_clientId = _configuration["AzureAD:ClientId"];
		_clientSecret = _configuration["AzureAD:ClientSecret"];
	}

	public async Task<string> GetAccessTokenAsync()
	{
		if (!_tokenStore.TryGetValue(_clientId, out var tokenData) || IsTokenExpired(tokenData.AccessToken))
		{
			tokenData = await RefreshAccessTokenAsync();
			_tokenStore[_clientId] = tokenData;
		}

		return tokenData.AccessToken;
	}

	public async Task<TokenData> RefreshAccessTokenAsync()
	{
		if (!_tokenStore.TryGetValue(_clientId, out var tokenData) || tokenData.RefreshToken == null)
		{
			throw new Exception("No refresh token available.");
		}

		var refreshToken = tokenData.RefreshToken;

		var parameters = new Dictionary<string, string>
		{
			{ "client_id", _clientId },
			{ "client_secret", _clientSecret },
			{ "refresh_token", refreshToken },
			{ "grant_type", "refresh_token" },
			{ "scope", "https://service.flow.microsoft.com/.default" },
		};

		var content = new FormUrlEncodedContent(parameters);
		var response = await _httpClient.PostAsync($"https://login.microsoftonline.com/{_tenantId}/oauth2/v2.0/token", content);

		if (!response.IsSuccessStatusCode)
		{
			throw new Exception("Failed to refresh token. " + response);
		}

		var responseContent = await response.Content.ReadAsStringAsync();
		var tokenJson = JsonDocument.Parse(responseContent);

		var newAccessToken = tokenJson.RootElement.GetProperty("access_token").GetString();
		var newRefreshToken = tokenJson.RootElement.GetProperty("refresh_token").GetString();

		tokenData = new TokenData
		{
			AccessToken = newAccessToken,
			RefreshToken = newRefreshToken,
			Expiry = DateTime.UtcNow.AddSeconds(tokenJson.RootElement.GetProperty("expires_in").GetInt32())
		};

		return tokenData;
	}

	public async Task<TokenData> GetAccessTokenUsingCodeAsync(
		string authorizationCode)
	{
		var parameters = new Dictionary<string, string>
		{
			{ "client_id", _clientId },
			{ "client_secret", _clientSecret },
			{ "code", authorizationCode },
			{ "grant_type", "authorization_code" },
			{ "redirect_uri", "http://localhost/auth/callback" },
			{ "scope", "https://service.flow.microsoft.com//.default offline_access" }
		};

		var content = new FormUrlEncodedContent(parameters);
		var response = await _httpClient.PostAsync(
			$"https://login.microsoftonline.com/{_tenantId}/oauth2/v2.0/token",
			content);

		if (!response.IsSuccessStatusCode)
		{
			throw new Exception($"Failed to retrieve access token. Status code: {response.StatusCode}");
		}

		var responseContent = await response.Content.ReadAsStringAsync();
		var tokenJson = JsonDocument.Parse(responseContent);

		var newAccessToken = tokenJson.RootElement.GetProperty("access_token").GetString();
		var newRefreshToken = tokenJson.RootElement.GetProperty("refresh_token").GetString();

		return new TokenData
		{
			AccessToken = newAccessToken,
			RefreshToken = newRefreshToken,
			Expiry = DateTime.UtcNow.AddSeconds(
				tokenJson.RootElement.GetProperty("expires_in").GetInt32())
		};
	}

	public void UseNewtoken(
		string accessToken,
		string refreshToken,
		int expiry)
	{
		if (!_tokenStore.ContainsKey(_clientId))
		{
			_tokenStore[_clientId] = new TokenData();
		}

		var expireTime = DateTime.UtcNow.AddSeconds(expiry);

		var tokenData = _tokenStore[_clientId];
		tokenData.AccessToken = accessToken;
		tokenData.RefreshToken = refreshToken;
		tokenData.Expiry = expireTime;
	}

	private bool IsTokenExpired(string accessToken)
	{
		return string.IsNullOrEmpty(accessToken) || DateTime.UtcNow > _tokenStore[_clientId].Expiry;
	}

	public class TokenData
	{
		public string AccessToken { get; set; }
		public string RefreshToken { get; set; }
		public DateTime Expiry { get; set; }
	}
}