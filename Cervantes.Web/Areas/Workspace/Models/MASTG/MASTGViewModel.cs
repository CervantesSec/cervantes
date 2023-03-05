using System;
using Cervantes.CORE;

namespace Cervantes.Web.Areas.Workspace.Models.MASTG;

public class MASTGViewModel
{
    /// <summary>
    /// Target Associated
    /// </summary>
    public virtual Target Target { get; set; }

    /// <summary>
    /// Id Target
    /// </summary>
    public Guid TargetId { get; set; }
    public Project Project { get; set; }
    public MASTGArch Arch { get; set; }
    public MASTGStorage Storage { get; set; }
    public MASTGCrypto Crypto { get; set; }
    public MASTGAuth Auth { get; set; }
    public MASTGNetwork Network { get; set; }
    public MASTGPlatform Platform { get; set; }
    public MASTGCode Code { get; set; }
    public MASTGResilience Resilience { get; set; }
}