using System.Linq;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.Services.Domain.Implementations;

namespace UserManagement.Services.Tests;

public class UserServiceTests
{
    [Fact]
    public void GetAll_WhenContextReturnsEntities_MustReturnSameEntities()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var service = CreateService();
        var users = SetupUsers();

        // Act: Invokes the method under test with the arranged parameters.
        var result = service.GetAll();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().BeSameAs(users);
    }

    [Fact]
    public void FilterByActive_WhenActiveIsTrue_MustReturnOnlyActiveUsers()
    {
        // Arrange
        var service = CreateService();
        var users = SetupUsersWithMixedActiveStatus();

        // Act
        var result = service.FilterByActive(true);

        // Assert
        result.Should().AllSatisfy(u => u.IsActive.Should().BeTrue());

    }

    [Fact]
    public void FilterByActive_WhenActiveIsFalse_MustReturnOnlyNonActiveUsers()
    {
        // Arrange
        var service = CreateService();
        var users = SetupUsersWithMixedActiveStatus();

        // Act
        var result = service.FilterByActive(false);

        // Assert
        result.Should().AllSatisfy(u => u.IsActive.Should().BeFalse());
    }

    private IQueryable<User> SetupUsers(string forename = "Johnny", string surname = "User", string email = "juser@example.com", bool isActive = true)
    {
        var users = new[]
        {
            new User
            {
                Forename = forename,
                Surname = surname,
                Email = email,
                IsActive = isActive
            }
        }.AsQueryable();

        _dataContext
            .Setup(s => s.GetAll<User>())
            .Returns(users);

        return users;
    }

    private IQueryable<User> SetupUsersWithMixedActiveStatus()
    {
        var users = new[] {
            new User{
                Forename = "Active", Surname= "User", Email = "email@email.com", IsActive = true,
            },
            new User{
                Forename = "NonActive", Surname= "User", Email = "email@email.com", IsActive = false,
            }
        }.AsQueryable();

        _dataContext
        .Setup(s => s.GetAll<User>()).Returns(users);

        return users;
    }

    private readonly Mock<IDataContext> _dataContext = new();
    private UserService CreateService() => new(_dataContext.Object);
}
