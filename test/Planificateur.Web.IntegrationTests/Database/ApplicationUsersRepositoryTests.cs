using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Planificateur.Core.Entities;
using Planificateur.Tests.Shared;
using Planificateur.Web.Database;
using Planificateur.Web.Database.Entities;
using Planificateur.Web.Database.Repositories;

namespace Planificateur.Web.Tests.Database;

[Collection("Database Tests")]
public class ApplicationUsersRepositoryTests
{
    private readonly ApplicationUsersRepository repository;
    private readonly ApplicationDbContext dbContext;
    private readonly DataFactory dataFactory = new();
    
    public ApplicationUsersRepositoryTests(DatabaseFixture database)
    {
        dbContext = database.DbContext;
        repository = new ApplicationUsersRepository(dbContext);
    }

    [Fact]
    public async Task Insert_InsertsInDb()
    {
        // Given
        ApplicationUser user = dataFactory.BuildTestUser();

        // When
        await repository.Insert(user);

        // Then
        ApplicationUserEntity userInDb = await dbContext.Users.FirstAsync(record => record.Id == user.Id);
        userInDb.Username.Should().Be(userInDb.Username);
        userInDb.Password.Should().NotBeEmpty();
    }

    [Fact]
    public async Task FindByUsername_ExistingUser_ReturnsUser()
    {
        // Given
        ApplicationUser user = dataFactory.BuildTestUser();
        await dbContext.Users.AddAsync(new ApplicationUserEntity(user));
        await dbContext.SaveChangesAsync();

        // When
        ApplicationUser? result = await repository.FindByUsername(user.Username);

        // Then
        Assert.NotNull(result);
        result.Id.Should().Be(user.Id);
        result.Username.Should().Be(user.Username);
        result.Password.Should().NotBeEmpty();
    }

    [Fact]
    public async Task FindByUsername_NonExistingUser_ReturnsNull()
    {
        // Given
        ApplicationUser user = dataFactory.BuildTestUser();
        await dbContext.Users.AddAsync(new ApplicationUserEntity(user));
        await dbContext.SaveChangesAsync();

        // When
        ApplicationUser? result = await repository.FindByUsername("non existing username");

        // Then
        Assert.Null(result);
    }
}