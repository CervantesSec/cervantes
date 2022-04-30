using Cervantes.CORE;

namespace Cervantes.Contracts
{
    public interface IUserManager : IGenericManager<ApplicationUser>
    {
        ApplicationUser GetByUserId(string id);
    }
}
