using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Kiota.Abstractions.Authentication;
using Planificateur.UI.ViewModels.Services;

namespace Planificateur.UI.Services;

public class TokenProvider : IAccessTokenProvider
{
    private readonly IStorageService storageService;

    public TokenProvider(IStorageService storageService)
    {
        this.storageService = storageService;
    }

    public async Task<string> GetAuthorizationTokenAsync(Uri uri, Dictionary<string, object>? additionalAuthenticationContext = null,
        CancellationToken cancellationToken = new())
    {
        if (await storageService.ContainsKeyAsync("token"))
        {
            return await storageService.GetAsync("token");
        }

        return string.Empty;
    }

    public AllowedHostsValidator AllowedHostsValidator => new();
}