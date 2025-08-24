using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;


namespace UserManagement.Services.Domain.Implementations;

public class UserService : IUserService
{
    private readonly IDataContext _dataAccess;
    public UserService(IDataContext dataAccess) => _dataAccess = dataAccess;

    /// <summary>
    /// Return users by active state
    /// </summary>
    /// <param name="isActive"></param>
    /// <returns></returns>
    public async Task<IEnumerable<User>> FilterByActiveAsync(bool isActive)
    {
        var users = await _dataAccess.GetAllAsync<User>();
        return users.Where(u => u.IsActive == isActive);
    }
    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _dataAccess.GetAllAsync<User>();
    }

    public async Task<User?> GetByIdAsync(long id)
    {
        var users = await _dataAccess.GetAllAsync<User>();
        return users.FirstOrDefault(u => u.Id == id);
    }
    public async Task CreateAsync(User user)
    {
        await _dataAccess.CreateAsync(user);
    }

    public async Task UpdateAsync(User user) => await _dataAccess.UpdateAsync(user);

    public async Task DeleteAsync(long id)
    {
        var user = await GetByIdAsync(id);
        if (user != null)
        {
            await _dataAccess.DeleteAsync(user);
        }
    }
}
