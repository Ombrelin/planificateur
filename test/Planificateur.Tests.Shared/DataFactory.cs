using Planificateur.Core.Entities;

namespace Planificateur.Tests.Shared;

public class DataFactory
{
    private static readonly object UserCountLock = new();
    private static int userCount;

    public readonly string Username = "John Shepard";
    public readonly string Password = "Test1";

    
    public string GetNewUsername()
    {
        lock (UserCountLock)
        {
            userCount++;
        }

        return $"{Username}-{userCount}";
    }

    
    public ApplicationUser BuildTestUser() => new(GetNewUsername(), Password);
}