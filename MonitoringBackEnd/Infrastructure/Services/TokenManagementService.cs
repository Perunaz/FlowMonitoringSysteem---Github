using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;

public class TokenManagementService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public TokenManagementService(IConfiguration configuration)
    {
        _configuration = configuration;
        _httpClient = new HttpClient();
    }

    public async Task<string> GetAccessTokenAsync(string url)
    {
        var authority = $"https://login.microsoftonline.com/{_configuration["AzureAD:TenantId"]}/oauth2/authorize";
        var certificate = GetCertificate(_configuration["AzureAD:ClientId"], _configuration["AzureAD:ThumbPrint"]);

        IConfidentialClientApplication app = ConfidentialClientApplicationBuilder.Create(_configuration["AzureAD:ClientId"])
            .WithAuthority(authority)
            .WithCertificate(certificate)
            .Build();

        //var result = await app.AcquireTokenForClient(new[] { $"{url}/.default" }).ExecuteAsync();
        var result = await app.AcquireTokenForClient(["dfe7a574-74d2-43c4-ade9-ab097c0cec3f/.default"]).ExecuteAsync();
        return result.AccessToken;
    }

    private X509Certificate2 GetCertificate(string clientId, string thumbprint)
    {
        //var certificate = Debugger.IsAttached ? GetCertificateFromDirectory(_configuration["AzureAD:DebugCertPath"], _configuration["AzureAD:DebugCertPasswordPath"]) : GetCertificateFromStore(thumbprint);
        var certificate = GetCertificateFromDirectory(_configuration["AzureAD:DebugCertPath"], _configuration["AzureAD:DebugCertPasswordPath"]);
        return certificate;
    }

    private static X509Certificate2 GetCertificateFromDirectory(string path, string password)
    {
        return new X509Certificate2(System.IO.Path.GetFullPath(path), password, X509KeyStorageFlags.MachineKeySet);
    }

    private static X509Certificate2 GetCertificateFromStore(string thumbprint)
    {
        var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
        store.Open(OpenFlags.ReadOnly);
        var certificates = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
        store.Close();
        return certificates[0];
    }
}