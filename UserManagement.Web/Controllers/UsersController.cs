using System.Linq;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Users;

namespace UserManagement.Web.Controllers;

[Route("users")]
public class UsersController(IUserService userService) : Controller
{
    private readonly IUserService _userService = userService;

    [HttpGet]
    public ViewResult List(string filter = "")
    {
        IEnumerable<User> users;
        if (filter == "active")
            users = _userService.FilterByActive(true);
        else if (filter == "nonactive")
            users = _userService.FilterByActive(false);
        else
            users = _userService.GetAll();


        var items = users.Select(p => new UserListItemViewModel
        {
            Id = p.Id,
            Forename = p.Forename,
            Surname = p.Surname,
            Email = p.Email,
            IsActive = p.IsActive,
            DateOfBirth = p.DateOfBirth
        });

        var model = new UserListViewModel
        {
            Items = items.ToList()
        };

        return View(model);
    }

    [HttpGet]
    [Route("addUser")]
    public ViewResult AddUser()
    {
        return View(new UserViewModel());
    }

    [HttpPost]
    [Route("addUser")]
    public IActionResult AddUser(UserViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new User
            {
                Forename = model.Forename,
                Surname = model.Surname,
                Email = model.Email,
                IsActive = model.IsActive,
                DateOfBirth = model.DateOfBirth
            };
            _userService.Create(user);
            return RedirectToAction("List");
        }
        return View(model);
    }

    [HttpGet]
    [Route("viewUser/{id}")]
    public IActionResult ViewUser(long id)
    {
        var user = _userService.GetById(id);
        if (user == null)
            return NotFound();
        var model = new UserViewModel
        {
            Id = user.Id,
            Forename = user.Forename,
            Surname = user.Surname,
            Email = user.Email,
            IsActive = user.IsActive,
            DateOfBirth = user.DateOfBirth
        };
        return View(model);
    }

    [HttpGet]
    [Route("editUser/{id}")]
    public IActionResult EditUser(long id)
    {
        var user = _userService.GetById(id);
        if (user == null)
            return NotFound();

        var model = new UserViewModel
        {
            Id = user.Id,
            Forename = user.Forename,
            Surname = user.Surname,
            Email = user.Email,
            IsActive = user.IsActive,
            DateOfBirth = user.DateOfBirth
        };
        return View(model);
    }

    [HttpPost]
    [Route("editUser/{id}")]
    public IActionResult EditUser(long id, UserViewModel model)
    {
        if (id != model.Id)
            return BadRequest();

        if (ModelState.IsValid)
        {
            var user = new User
            {
                Id = model.Id,
                Forename = model.Forename,
                Surname = model.Surname,
                Email = model.Email,
                IsActive = model.IsActive,
                DateOfBirth = model.DateOfBirth
            };
            _userService.Update(user);
            return RedirectToAction("List");
        }
        return View(model);
    }

    [HttpGet]
    [Route("deleteUser/{id}")]
    public IActionResult DeleteUser(long id)
    {
        var user = _userService.GetById(id);
        if (user == null)
            return NotFound();

        var model = new UserViewModel
        {
            Id = user.Id,
            Forename = user.Forename,
            Surname = user.Surname,
            Email = user.Email,
            IsActive = user.IsActive,
            DateOfBirth = user.DateOfBirth
        };
        return View(model);
    }

    [HttpPost, ActionName("DeleteUser")]
    [Route("deleteUser/{id}")]
    public IActionResult DeleteUserConfirmed(long id)
    {
        var user = _userService.GetById(id);
        if (user == null) return NotFound();
        _userService.Delete(id);
        return RedirectToAction("List");
    }
}
