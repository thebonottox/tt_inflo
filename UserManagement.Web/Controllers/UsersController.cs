using System.Linq;
using System.Threading.Tasks;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Users;

namespace UserManagement.Web.Controllers;

[Route("users")]
public class UsersController(IUserService userService) : Controller
{
    private readonly IUserService _userService = userService;

    [HttpGet]
    public async Task<IActionResult> List(string filter = "")
    {
        IEnumerable<User> users;
        if (filter == "active")
            users = await _userService.FilterByActiveAsync(true);
        else if (filter == "nonactive")
            users = await _userService.FilterByActiveAsync(false);
        else
            users = await _userService.GetAllAsync();


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
    public async Task<IActionResult> AddUser(UserViewModel model)
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
            await _userService.CreateAsync(user);
            return RedirectToAction("List");
        }
        return View(model);
    }

    [HttpGet]
    [Route("viewUser/{id}")]
    public async Task<IActionResult> ViewUser(long id)
    {
        var user = await _userService.GetByIdAsync(id);
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
    public async Task<IActionResult> EditUser(long id)
    {
        var user = await _userService.GetByIdAsync(id);
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
    public async Task<IActionResult> EditUser(long id, UserViewModel model)
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
            await _userService.UpdateAsync(user);
            return RedirectToAction("List");
        }
        return View(model);
    }

    [HttpGet]
    [Route("deleteUser/{id}")]
    public async Task<IActionResult> DeleteUser(long id)
    {
        var user = await _userService.GetByIdAsync(id);
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
    public async Task<IActionResult> DeleteUserConfirmed(long id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null) return NotFound();
        await _userService.DeleteAsync(id);
        return RedirectToAction("List");
    }
}
