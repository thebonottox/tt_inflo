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
            IsActive = p.IsActive
        });

        var model = new UserListViewModel
        {
            Items = items.ToList()
        };

        return View(model);
    }


}
