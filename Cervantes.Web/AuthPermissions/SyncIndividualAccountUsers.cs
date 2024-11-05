using AuthPermissions.AdminCode;
using Cervantes.CORE.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Cervantes.Web.AuthPermissions;

public class SyncIndividualAccountUsers : ISyncAuthenticationUsers
{
    private readonly UserManager<ApplicationUser> _userManager;

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="userManager"></param>
    public SyncIndividualAccountUsers(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    /// <summary>
    /// This returns the userId, email and UserName of all the users
    /// </summary>
    /// <returns>collection of SyncAuthenticationUser</returns>
    public async Task<IEnumerable<SyncAuthenticationUser>> GetAllActiveUserInfoAsync()
    {
        return await _userManager.Users
            .Select(x => new SyncAuthenticationUser(x.Id, x.Email, x.UserName)).ToListAsync();
    }
}
