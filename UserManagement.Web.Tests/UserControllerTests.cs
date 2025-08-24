using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Users;
using UserManagement.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;

namespace UserManagement.Data.Tests;

public class UserControllerTests
{
    [Fact]
    public void List_WhenServiceReturnsUsers_ModelMustContainUsers()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var users = SetupUsers();

        // Act: Invokes the method under test with the arranged parameters.
        var result = controller.List();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Model
            .Should().BeOfType<UserListViewModel>()
            .Which.Items.Should().BeEquivalentTo(users);
    }

    [Fact]
    public void AddUser_Post_ValidModel_SavesAndRedirects()
    {

        var controller = CreateController();
        // Arrange
        var userViewModel = new UserViewModel
        {
            Forename = "John",
            Surname = "Doe",
            Email = "john.doe@example.com",
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        // Act
        var result = controller.AddUser(userViewModel);

        // Assert
        result.Should().BeOfType<RedirectToActionResult>()
            .Which.ActionName.Should().Be("List");

        _userService.Verify(s => s.Create(It.Is<User>(u =>
            u.Forename == userViewModel.Forename &&
            u.Surname == userViewModel.Surname &&
            u.Email == userViewModel.Email &&
            u.DateOfBirth == userViewModel.DateOfBirth)), Times.Once());
    }

    [Fact]
    public void ViewUser_ValidId_ReturnsViewWithUser()
    {
        var controller = CreateController();

        // Arrange
        var user = new User
        {
            Id = 1,
            Forename = "John",
            Surname = "Doe",
            Email = "john.doe@example.com",
            DateOfBirth = new DateTime(1990, 1, 1)
        };
        _userService.Setup(s => s.GetById(1)).Returns(user);

        // Act
        var result = controller.ViewUser(1);

        // Assert
        result.Should().BeOfType<ViewResult>()
            .Which.Model.Should().BeOfType<UserViewModel>()
            .Which.Should().BeEquivalentTo(new UserViewModel
            {
                Id = user.Id,
                Forename = user.Forename,
                Surname = user.Surname,
                Email = user.Email,
                DateOfBirth = user.DateOfBirth
            });
    }

    [Fact]
    public void EditUser_Get_ValidId_ReturnsViewWithUser()
    {
        var controller = CreateController();
        var user = new User
        {
            Id = 1,
            Forename = "John",
            Surname = "Doe",
            Email = "john.doe@example.com",
            DateOfBirth = new DateTime(1990, 1, 1)
        };
        _userService.Setup(s => s.GetById(1)).Returns(user);

        var result = controller.EditUser(1);

        result.Should().BeOfType<ViewResult>()
            .Which.Model.Should().BeOfType<UserViewModel>()
            .Which.Should().BeEquivalentTo(new UserViewModel
            {
                Id = user.Id,
                Forename = user.Forename,
                Surname = user.Surname,
                Email = user.Email,
                DateOfBirth = user.DateOfBirth
            });
    }

    [Fact]
    public void EditUser_Post_ValidModel_UpdatesAndRedirects()
    {
        var controller = CreateController();
        var userViewModel = new UserViewModel
        {
            Id = 1,
            Forename = "John",
            Surname = "Doe",
            Email = "john.doe@example.com",
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        var result = controller.EditUser(1, userViewModel);

        result.Should().BeOfType<RedirectToActionResult>()
            .Which.ActionName.Should().Be("List");
        _userService.Verify(s => s.Update(It.Is<User>(u =>
            u.Id == userViewModel.Id &&
            u.Forename == userViewModel.Forename &&
            u.Surname == userViewModel.Surname &&
            u.Email == userViewModel.Email &&
            u.DateOfBirth == userViewModel.DateOfBirth)), Times.Once());
    }

    [Fact]
    public void DeleteUserConfirmed_ValidId_DeletesAndRedirects()
    {
        var controller = CreateController();
        var user = new User { Id = 1 };
        _userService.Setup(s => s.GetById(1)).Returns(user);
        var result = controller.DeleteUserConfirmed(1);
        result.Should().BeOfType<RedirectToActionResult>()
            .Which.ActionName.Should().Be("List");
        _userService.Verify(s => s.Delete(1), Times.Once());
    }

    private User[] SetupUsers(string forename = "Johnny", string surname = "User", string email = "juser@example.com", bool isActive = true)
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
        };

        _userService
            .Setup(s => s.GetAll())
            .Returns(users);

        return users;
    }

    private readonly Mock<IUserService> _userService = new();
    private UsersController CreateController() => new(_userService.Object);
}
