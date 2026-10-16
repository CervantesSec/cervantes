using Cervantes.CORE;
using Cervantes.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using Cervantes.CORE.Entities;

namespace Cervantes.Application;

public class UserManager : GenericManager<ApplicationUser>, IUserManager, IUserStore<ApplicationUser>
{
    private UserManager<ApplicationUser> _userManager = null;

    /// <summary>
    /// User Manager Constructor
    /// </summary>
    /// <param name="context"></param>
    public UserManager(IApplicationDbContext context, UserManager<ApplicationUser> userManager) : base(context)
    {
        _userManager = userManager;
    }

    /// <summary>
    /// Get user by his id
    /// </summary>
    /// <param name="id">id</param>
    /// <returns>User</returns>
    public ApplicationUser GetByUserId(string id)
    {
        return Context.Set<ApplicationUser>().Find(id);
    }
    
    public ApplicationUser GetByEmail(string email)
    {
        return Context.Set<ApplicationUser>().Where(e => e.Email == email).FirstOrDefault();
    }

    public Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    // IUserStore<ApplicationUser> is IDisposable. The DbContext and the Identity UserManager held here are owned
    // and disposed by the DI container, so there is nothing to release. Throwing here made every DI scope that
    // resolved this manager fail on disposal: Hangfire then marked email jobs Failed after the mail was sent and
    // retried them, sending duplicates.
    public void Dispose()
    {
    }


    public Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public System.Threading.Tasks.Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public System.Threading.Tasks.Task SetUserNameAsync(ApplicationUser user, string userName,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}