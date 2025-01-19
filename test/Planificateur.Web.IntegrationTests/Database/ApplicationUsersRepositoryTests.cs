using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Planificateur.Core.Entities;
using Planificateur.Tests.Shared;
using Planificateur.Web.Database;
using Planificateur.Web.Database.Entities;
using Planificateur.Web.Database.Repositories;

namespace Planificateur.Web.Tests.Database;

[Collection("Database Tests")]
public class ApplicationUsersRepositoryTests : DatabaseTests
{

    private ApplicationUsersRepository applicationUsersRepository = null!;
    private readonly DataFactory dataFactory = new();

    public ApplicationUsersRepositoryTests(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        applicationUsersRepository = new ApplicationUsersRepository(DbContext);
    }

    [Fact]
    public async Task Insert_InsertsInDb()
    {
        // Given
        ApplicationUser user = dataFactory.BuildTestUser();

        // When
        await applicationUsersRepository.Insert(user);

        // Then
        ApplicationUserEntity userInDb = await DbContext.Users.FirstAsync(record => record.Id == user.Id);
        userInDb.Username.Should().Be(userInDb.Username);
        userInDb.Password.Should().NotBeEmpty();
    }

    [Fact]
    public async Task FindByUsername_ExistingUser_ReturnsUser()
    {
        // Given
        ApplicationUser user = dataFactory.BuildTestUser();
        await DbContext.Users.AddAsync(new ApplicationUserEntity(user));
        await DbContext.SaveChangesAsync();

        // When
        ApplicationUser? result = await applicationUsersRepository.FindByUsername(user.Username);

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
        await DbContext.Users.AddAsync(new ApplicationUserEntity(user));
        await DbContext.SaveChangesAsync();

        // When
        ApplicationUser? result = await applicationUsersRepository.FindByUsername("non existing username");

        // Then
        Assert.Null(result);
    }


}