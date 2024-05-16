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
		_accessToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6IkwxS2ZLRklfam5YYndXYzIyeFp4dzFzVUhIMCIsImtpZCI6IkwxS2ZLRklfam5YYndXYzIyeFp4dzFzVUhIMCJ9.eyJhdWQiOiJodHRwczovL3NlcnZpY2UuZmxvdy5taWNyb3NvZnQuY29tLyIsImlzcyI6Imh0dHBzOi8vc3RzLndpbmRvd3MubmV0L2VlNWZhYjljLWZjMTAtNGZiZi1iOGY2LTM4N2JmZmJiNTE5Ni8iLCJpYXQiOjE3MTU4NzA0MDQsIm5iZiI6MTcxNTg3MDQwNCwiZXhwIjoxNzE1ODc1MjIzLCJhY2N0IjowLCJhY3IiOiIxIiwiYWlvIjoiQVRRQXkvOFdBQUFBK2JGQlF6VFJ5UXduZm1sdG1YQXlVRnYyTmMwQlJDK3ZPc2lINXJqakVuclZMbTFjamtrWmNWMnFlZ29YUmhldyIsImFtciI6WyJwd2QiXSwiYXBwaWQiOiJkZmU3YTU3NC03NGQyLTQzYzQtYWRlOS1hYjA5N2MwY2VjM2YiLCJhcHBpZGFjciI6IjEiLCJmYW1pbHlfbmFtZSI6IjQtVG9wIiwiZ2l2ZW5fbmFtZSI6IkFkbWluIiwiaXBhZGRyIjoiMjAwMTo0YzNjOmVlMDA6MmUwMDo1YzE6NzU0ODoyMjVjOjUxYzYiLCJuYW1lIjoiQWRtaW4gfCA0LVRvcC5ubCIsIm9pZCI6ImM1ZmE2ZWVjLTJjODAtNDEyMy1hNTFlLTc0MDRmOTZkYTFlZCIsInB1aWQiOiIxMDAzMjAwMzY4Q0IzOTE1IiwicmgiOiIwLkFhOEFuS3RmN2hEOHYwLTQ5amg3Xzd0UmxpV2g4SDItMDVaTXFsUlpINFBfVkJ3ZEFYNC4iLCJzY3AiOiJGbG93cy5NYW5hZ2UuQWxsIiwic2lnbmluX3N0YXRlIjpbImttc2kiXSwic3ViIjoib0w1bmplTUZEaF83U3U0Ykx0VGdieUt6SXA4OTZpVXc2dVY1bDFOZWhRYyIsInRpZCI6ImVlNWZhYjljLWZjMTAtNGZiZi1iOGY2LTM4N2JmZmJiNTE5NiIsInVuaXF1ZV9uYW1lIjoiYWRtaW5ANC10b3AubmwiLCJ1cG4iOiJhZG1pbkA0LXRvcC5ubCIsInV0aSI6IktmV3UwZHhRcEVPcVlKbDA4Q2dtQUEiLCJ2ZXIiOiIxLjAifQ.jq9dZCsD20b_MzjEJZILyCH2qfyj5yDQhQe18-bbqB0xCpp6Sdwu25mOoN9MRhI9QwImCGTdoe4bpaGISv-9KoITTtRD1osc6kKG3IZbehc3ZbiRd8Sw4PHoVAHiaBHCz1SUsREx_uAAM_65GLWH64P-XW56CJxBVDoLFRNZIgrGs7mNJRPnCcISiBs1xhFrIq--HxGuuWkbqDk1Kb9cLli5SYFRogO91bdIELQ_CATBYeZFsM7rCSjg3FFA-lTVAmvwtwE0avGaivJGbgIshZIi95_ktc1YMkSTswnBJcTShki_a8-2BlDs5-yaDksqAxY7Y7QX7i8orKTy3kDhXg";
		_refreshToken = "0.Aa8AnKtf7hD8v0-49jh7_7tRlnSl59_SdMRDremrCXwM7D8dAX4.AgABAwEAAADnfolhJpSnRYB1SVj-Hgd8AgDs_wUA9P9GXA_u8rly61hrFAoGKhufG9aGq0sY4KF1oDHPVjzSk_IHJLyHlZ8TB_ZHYEBy7k_paH0NggWftC7r656jPodL83WAqZyPVgiPr3MCJ1ChbZ6urfc8I3IKduuP0vufVOOk8TERj8LAPfpAJob_iDbvSqyKFSqrkulJEwOOn8xJtzU0IWr_eaFCHuEn0DAu-zJsJRfFcChZu20hk01xoBcu5vqK2zuTVE2P7-h2ZDIL_N7NJH1icvzvrixPyOtJRmm1xXxF6pIYXdMPdWVBo3uGgMdyFzjgKS4Ddq_O349CbBnBfbLZcgG9ECHKpiBto60lUo42iP55eaUg2yFGvTbrtKF6HKB1rs-iBTse7yULrTIk3aHYYE0UD7srhhTryFRwpV6CDPk0c0cRiO5JR7aUq-T5CdIrV-x_xTPE4qLcRIbFAGmTd5GrwdV6WAxwau6Insx1FqfBJATtb56kg4oxleFpdTp-yw5_O5Kww_PwPJU7X4LDLuvbhntuVuDLy7I6fqTQA9xa0pmksaPhHToRHQ00vK_uEVYbw-aStl6pmL791iZnPGkeUkCfo02OMAEoVAIbp3oACm0_MqdAOL6_TQIyvOXx_tNWKel9XdYxZrUTdDdIQ_2_kxH38DsLsGgBFZu-tFkFpxOmCth3uJvYMHf7uybN3E7lm95GNtgACwgeLI-9v9StIKprCRgmQZ9m0iPKH0vy0U4Vt5ZXXVfI1lOfol-8PZDhhq0so6lrid7Ohkpgg8enzZCLQiPPmKMWnU0jaOcMhcW3Q0vp";
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