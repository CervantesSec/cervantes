using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace Cervantes.Contracts;

public interface IRoleManager : IGenericManager<IdentityRole>
{
    IQueryable<IdentityRole> GetByName(string name);
}