using System.Linq;
using System.Threading.Tasks;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.Services.Domain.Implementations;

namespace UserManagement.Services.Tests;

public class UserServiceTests
{
    [Fact]
    public async Task GetAll_WhenContextReturnsEntities_MustReturnSameEntities()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var service = CreateService();
        var users = SetupUsers();

        // Act: Invokes the method under test with the arranged parameters.
        var result = await service.GetAllAsync();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().BeSameAs((System.Collections.Generic.IEnumerable<User>)users);
    }

    [Fact]
    public async Task FilterByActive_WhenActiveIsTrue_MustReturnOnlyActiveUsers()
    {
        // Arrange
        var service = CreateService();
        var users = SetupUsersWithMixedActiveStatus();

        // Act
        var result = await service.FilterByActiveAsync(true);

        // Assert
        result.Should().AllSatisfy(u => u.IsActive.Should().BeTrue());

    }

    [Fact]
    public async Task FilterByActive_WhenActiveIsFalse_MustReturnOnlyNonActiveUsers()
    {
        // Arrange
        var service = CreateService();
        var users = SetupUsersWithMixedActiveStatus();

        // Act
        var result = await service.FilterByActiveAsync(false);

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
           .Setup(s => s.GetAllAsync<User>())
           .ReturnsAsync(users);

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
        .Setup(s => s.GetAllAsync<User>()).ReturnsAsync(users);

        return users;
    }

    private readonly Mock<IDataContext> _dataContext = new();
    private UserService CreateService() => new(_dataContext.Object);
}
