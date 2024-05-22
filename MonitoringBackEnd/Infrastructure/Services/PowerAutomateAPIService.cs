using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

public class PowerAutomateAPIService
{
	private readonly TokenManagementService _tokenManagementService;
	private readonly IConfiguration _configuration;
	private readonly string _tenantId;

	public PowerAutomateAPIService(TokenManagementService tokenManagementService, IConfiguration configuration)
	{
		_tokenManagementService = tokenManagementService;
		_configuration = configuration;

		_tenantId = _configuration["AzureAD:TenantId"];
	}

	private async Task<string> GetResourceAsync(string endpoint)
	{
		var accessToken = await _tokenManagementService.GetAccessTokenAsync("https://service.flow.microsoft.com/");

		using (var httpClient = new HttpClient())
		{
			httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

			var response = await httpClient.GetAsync(endpoint);

			//if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
			//{
			//	// Refresh token
			//	accessToken = await _tokenManagementService.RefreshAccessTokenAsync();
			//	httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
			//	response = await httpClient.GetAsync(endpoint);
			//}

			if (!response.IsSuccessStatusCode)
			{
				throw new Exception($"Failed to fetch resource from {endpoint}. Response: {response}");
			}

			return await response.Content.ReadAsStringAsync();
		}
	}

	public async Task<string> GetResourceFromPowerAutomateFlowsAsync()
	{
		return await GetResourceAsync($"https://api.flow.microsoft.com/providers/Microsoft.ProcessSimple/environments/Default-{_tenantId}/flows/");
	}

	public async Task<string> GetResourceFromPowerAutomateFlowRunsAsync(string flowId)
	{
		return await GetResourceAsync($"https://api.flow.microsoft.com/providers/Microsoft.ProcessSimple/environments/Default-{_tenantId}/flows/{flowId}/runs");
	}
}
