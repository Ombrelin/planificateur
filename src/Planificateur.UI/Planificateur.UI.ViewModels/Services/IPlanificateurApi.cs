using Planificateur.Core.Contracts;

namespace Planificateur.UI.ViewModels.Services;

public interface IPlanificateurApi
{
    public Task<LoginResponse?> Login(string baseUrl, LoginRequest loginRequest);
}