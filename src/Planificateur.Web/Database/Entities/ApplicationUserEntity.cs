using Planificateur.Core.Entities;

namespace Planificateur.Web.Database.Entities;

public class ApplicationUserEntity : IEntity<ApplicationUser>
{
    public string Password { get; set; }
    public string Username { get; set; }
    public Guid Id { get; set; }

    public ApplicationUserEntity()
    {
    }

    public ApplicationUserEntity(ApplicationUser applicationUser)
    {
        Id = applicationUser.Id;
        Username = applicationUser.Username;
        Password = applicationUser.Password;
    }

    public ApplicationUser ToDomainObject() => new(Id, Username, Password);
}