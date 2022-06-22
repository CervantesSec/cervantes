using Cervantes.Contracts;
using Cervantes.CORE;

namespace Cervantes.Application;

public class VaultManager : GenericManager<Vault>, IVaultManager
{
    /// <summary>
    /// Data Manager Constructor
    /// </summary>
    /// <param name="context"></param>
    public VaultManager(IApplicationDbContext context) : base(context)
    {
    }
}