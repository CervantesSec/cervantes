using System;
using Cervantes.CORE.Entities;

namespace Cervantes.CORE.ViewModel.Wstg;

public class WSTGViewModel
{
    public Guid Id { get; set; }
    public WSTGInfo Info { get; set; }
    public WSTGConf Conf { get; set; }
    public WSTGIdnt Idnt { get; set; }
    public WSTGAuth Auth { get; set; }
    public WSTGAthz Athz { get; set; }
    public WSTGSess Sess { get; set; }
    public WSTGInpv Inpv { get; set; }
    public WSTGApit Apit { get; set; }
    public WSTGErrh Errh { get; set; }
    public WSTGCryp Cryp { get; set; }
    public WSTGBusl Busl { get; set; }
    public WSTGClnt Clnt { get; set; }
}