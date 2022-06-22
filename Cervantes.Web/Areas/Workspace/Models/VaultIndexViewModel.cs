using System.Collections.Generic;
using Cervantes.CORE;

namespace Cervantes.Web.Areas.Workspace.Models;

public class VaultIndexViewModel
{
    public IEnumerable<Vault> Vaults { get; set; }
    public Project Project { get; set; }
}