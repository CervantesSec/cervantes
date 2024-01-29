using Cervantes.CORE;
using Cervantes.CORE.Entities;

namespace Cervantes.Contracts;

public interface IUserManager : IGenericManager<ApplicationUser>
{
    ApplicationUser GetByUserId(string id);
    ApplicationUser GetByEmail(string email);
}