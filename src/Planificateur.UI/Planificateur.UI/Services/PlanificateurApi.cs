using System.Threading.Tasks;
using Microsoft.Kiota.Abstractions;
using Planificateur.ClientSdk;
using Planificateur.ClientSdk.Models;
using Planificateur.UI.ViewModels.Services;

namespace Planificateur.UI.Services;

public class PlanificateurApi : IPlanificateurApi
{
    private readonly IRequestAdapter requestAdapter;
    private readonly PlanificateurClient client;
    private readonly IStorageService storageService;

    public PlanificateurApi(IRequestAdapter requestAdapter, IStorageService storageService)
    {
        this.requestAdapter = requestAdapter;
        this.storageService = storageService;
        this.client = new PlanificateurClient(requestAdapter);
    }

    public async Task<LoginResponse?> Login(string baseUrl, LoginRequest loginRequest)
    {
        await storageService.PutAsync("url", baseUrl);
        requestAdapter.BaseUrl = baseUrl;
        LoginResponse? response = await client.Api.Authentication.Login.PostAsync(loginRequest);
        await storageService.PutAsync("token", response.Token);
        return response;
    }
        
}