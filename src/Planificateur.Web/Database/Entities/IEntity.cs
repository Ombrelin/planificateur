namespace Planificateur.Web.Database.Entities;

public interface IEntity<T>
{
    T ToDomainObject();
}