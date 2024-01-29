using System;
using Cervantes.CORE.Entities;

namespace Cervantes.CORE.ViewModel.Mastg;

public class MastgViewModel
{
    public Guid Id { get; set; }
    public MobileAppPlatform MobilePlatform { get; set; }
    public MASTGStorage Storage { get; set; }
    public MASTGCrypto Crypto { get; set; }
    public MASTGAuth Auth { get; set; }
    public MASTGNetwork Network { get; set; }
    public MASTGPlatform Platform { get; set; }
    public MASTGCode Code { get; set; }
    public MASTGResilience Resilience { get; set; }
}