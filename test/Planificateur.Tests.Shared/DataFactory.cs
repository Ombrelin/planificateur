using Planificateur.Core.Entities;

namespace Planificateur.Tests.Shared;

public class DataFactory
{
    private static int testUsersCount = 0;
    
    public readonly string Username = "John Shepard";
    public readonly string Password = "Test1";

    public ApplicationUser BuildTestUser() => new ($"{Username}-{++testUsersCount}", Password);
}