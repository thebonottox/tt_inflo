using System;
using System.Linq;
using FluentAssertions;
using UserManagement.Models;

namespace UserManagement.Data.Tests;

public class DataContextTests
{
    [Fact]
    public void GetAll_WhenNewEntityAdded_MustIncludeNewEntity()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var context = CreateContext();

        var entity = new User
        {
            Forename = "Brand New",
            Surname = "User",
            Email = "brandnewuser@example.com"
        };
        context.Create(entity);

        // Act: Invokes the method under test with the arranged parameters.
        var result = context.GetAll<User>();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result
            .Should().Contain(s => s.Email == entity.Email)
            .Which.Should().BeEquivalentTo(entity);
    }

    [Fact]
    public void GetAll_WhenDeleted_MustNotIncludeDeletedEntity()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var context = CreateContext();
        var entity = context.GetAll<User>().First();
        context.Delete(entity);

        // Act: Invokes the method under test with the arranged parameters.
        var result = context.GetAll<User>();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().NotContain(s => s.Email == entity.Email);
    }

    [Fact]
    public void CreateAndGetAll_WhenUserWithDateOfBirthAdded_RetrievesCorrectDate()
    {
        var context = CreateContext();
        var entity = new User
        {
            Forename = "Test",
            Surname = "User",
            Email = "test@example.com",
            DateOfBirth = new DateTime(1992, 1, 21)
        };

        context.Create(entity);

        // Act
        var result = context.GetAll<User>();

        result.Should().Contain(u => u.Email == entity.Email && u.DateOfBirth == entity.DateOfBirth)
            .Which.Should().BeEquivalentTo(entity);
    }
    private DataContext CreateContext() => new();
}
