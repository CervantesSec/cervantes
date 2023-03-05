using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.Web.Areas.Workspace.Models;
using Cervantes.Web.Areas.Workspace.Models.MASTG;
using Cervantes.Web.Areas.Workspace.Models.Wstg;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;

namespace Cervantes.Web.Areas.Workspace.Controllers;

[Authorize(Roles = "Admin,SuperUser,User")]
[Area("Workspace")]
public class ChecklistController : Controller
{
    private readonly ILogger<ChecklistController> _logger = null;
    private IProjectManager projectManager = null;
    private IProjectUserManager projectUserManager = null;
    private ITargetManager targetManager = null;
    private IWSTGManager wstgManager = null;
    private IMASTGManager mastgManager = null;
    
    public ChecklistController(IProjectManager projectManager, 
        IProjectUserManager projectUserManager, ITargetManager targetManager, ILogger<ChecklistController> logger, IWSTGManager wstgManager,
        IMASTGManager mastgManager)
    {
        this.projectManager = projectManager;
        this.projectUserManager = projectUserManager;
        this.targetManager = targetManager;
        _logger = logger;
        this.wstgManager = wstgManager;
        this.mastgManager = mastgManager;
    }
    // GET
    public IActionResult Index(Guid project)
    {
        var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
        if (user == null)
        {
            TempData["userProject"] = "User is not in the project";
            return RedirectToAction("Index", "Workspaces",new {area =""});
        }

        ChecklistViewModel model = new ChecklistViewModel
        {
            WSTG = wstgManager.GetAll().Where(x => x.ProjectId == project).ToList(),
            MASTG = mastgManager.GetAll().Where(x => x.ProjectId == project).ToList(),
            Project = projectManager.GetById(project)

        };
        
        return View(model);
    }

    public IActionResult Wstg(Guid project, Guid id)
    {
        try
        {
            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                TempData["userProject"] = "User is not in the project";
                return RedirectToAction("Index", "Workspaces", new {area = ""});
            }

            var wstg = wstgManager.GetTargetId(project, id);
            WSTGInfo info = new WSTGInfo
            {
                Info01Note = wstg.Info01Note,
                Info01Status = wstg.Info01Status,
                Info02Note = wstg.Info02Note,
                Info02Status = wstg.Info02Status,
                Info03Note = wstg.Info03Note,
                Info03Status = wstg.Info03Status,
                Info04Note = wstg.Info04Note,
                Info04Status = wstg.Info04Status,
                Info05Note = wstg.Info05Note,
                Info05Status = wstg.Info05Status,
                Info06Note = wstg.Info06Note,
                Info06Status = wstg.Info06Status,
                Info07Note = wstg.Info07Note,
                Info07Status = wstg.Info07Status,
                Info08Note = wstg.Info08Note,
                Info08Status = wstg.Info08Status,
                Info09Note = wstg.Info09Note,
                Info09Status = wstg.Info09Status,
                Info10Note = wstg.Info10Note,
                Info10Status = wstg.Info10Status,
            };
            WSTGApit apit = new WSTGApit
            {
                Apit01Note = wstg.Apit01Note,
                Apit01Status = wstg.Apit01Status
            };
            WSTGAthz athz = new WSTGAthz
            {
                Athz01Note = wstg.Athz01Note,
                Athz02Note = wstg.Athz02Note,
                Athz03Note = wstg.Athz03Note,
                Athz04Note = wstg.Athz04Note,
                Athz01Status = wstg.Athz01Status,
                Athz02Status = wstg.Athz02Status,
                Athz03Status = wstg.Athz03Status,
                Athz04Status = wstg.Athz04Status
            };
            WSTGAuth auth = new WSTGAuth
            {
                Athn01Note = wstg.Athn01Note,
                Athn02Note = wstg.Athn02Note,
                Athn03Note = wstg.Athn03Note,
                Athn04Note = wstg.Athn04Note,
                Athn05Note = wstg.Athn05Note,
                Athn06Note = wstg.Athn06Note,
                Athn07Note = wstg.Athn07Note,
                Athn08Note = wstg.Athn08Note,
                Athn09Note = wstg.Athn09Note,
                Athn10Note = wstg.Athn10Note,
                Athn01Status = wstg.Athn01Status,
                Athn02Status = wstg.Athn02Status,
                Athn03Status = wstg.Athn03Status,
                Athn04Status = wstg.Athn04Status,
                Athn05Status = wstg.Athn05Status,
                Athn06Status = wstg.Athn06Status,
                Athn07Status = wstg.Athn07Status,
                Athn08Status = wstg.Athn08Status,
                Athn09Status = wstg.Athn09Status,
                Athn10Status = wstg.Athn10Status
            };
            WSTGBusl busl = new WSTGBusl
            {
                Busl01Note = wstg.Busl01Note,
                Busl02Note = wstg.Busl02Note,
                Busl03Note = wstg.Busl03Note,
                Busl04Note = wstg.Busl04Note,
                Busl05Note = wstg.Busl05Note,
                Busl06Note = wstg.Busl06Note,
                Busl07Note = wstg.Busl07Note,
                Busl08Note = wstg.Busl08Note,
                Busl09Note = wstg.Busl09Note,
                Busl01Status = wstg.Busl01Status,
                Busl02Status = wstg.Busl02Status,
                Busl03Status = wstg.Busl03Status,
                Busl04Status = wstg.Busl04Status,
                Busl05Status = wstg.Busl05Status,
                Busl06Status = wstg.Busl06Status,
                Busl07Status = wstg.Busl07Status,
                Busl08Status = wstg.Busl08Status,
                Busl09Status = wstg.Busl09Status
            };
            WSTGClnt clnt = new WSTGClnt
            {
                Clnt01Note = wstg.Clnt01Note,
                Clnt02Note = wstg.Clnt02Note,
                Clnt03Note = wstg.Clnt03Note,
                Clnt04Note = wstg.Clnt04Note,
                Clnt05Note = wstg.Clnt05Note,
                Clnt06Note = wstg.Clnt06Note,
                Clnt07Note = wstg.Clnt07Note,
                Clnt08Note = wstg.Clnt08Note,
                Clnt09Note = wstg.Clnt09Note,
                Clnt10Note = wstg.Clnt10Note,
                Clnt11Note = wstg.Clnt11Note,
                Clnt12Note = wstg.Clnt12Note,
                Clnt13Note = wstg.Clnt13Note,
                Clnt01Status = wstg.Clnt01Status,
                Clnt02Status = wstg.Clnt02Status,
                Clnt03Status = wstg.Clnt03Status,
                Clnt04Status = wstg.Clnt04Status,
                Clnt05Status = wstg.Clnt05Status,
                Clnt06Status = wstg.Clnt06Status,
                Clnt07Status = wstg.Clnt07Status,
                Clnt08Status = wstg.Clnt08Status,
                Clnt09Status = wstg.Clnt09Status,
                Clnt10Status = wstg.Clnt10Status,
                Clnt11Status = wstg.Clnt11Status,
                Clnt12Status = wstg.Clnt12Status,
                Clnt13Status = wstg.Clnt13Status
            };
            WSTGConf conf = new WSTGConf
            {
                Conf01Note = wstg.Conf01Note,
                Conf02Note = wstg.Conf02Note,
                Conf03Note = wstg.Conf03Note,
                Conf04Note = wstg.Conf04Note,
                Conf05Note = wstg.Conf05Note,
                Conf06Note = wstg.Conf06Note,
                Conf07Note = wstg.Conf07Note,
                Conf08Note = wstg.Conf08Note,
                Conf09Note = wstg.Conf09Note,
                Conf10Note = wstg.Conf10Note,
                Conf11Note = wstg.Conf11Note,
                Conf01Status = wstg.Conf01Status,
                Conf02Status = wstg.Conf02Status,
                Conf03Status = wstg.Conf03Status,
                Conf04Status = wstg.Conf04Status,
                Conf05Status = wstg.Conf05Status,
                Conf06Status = wstg.Conf06Status,
                Conf07Status = wstg.Conf07Status,
                Conf08Status = wstg.Conf08Status,
                Conf09Status = wstg.Conf09Status,
                Conf10Status = wstg.Conf10Status,
                Conf11Status = wstg.Conf11Status
            };
            WSTGCryp cryp = new WSTGCryp
            {
                Cryp01Note = wstg.Cryp01Note,
                Cryp02Note = wstg.Cryp02Note,
                Cryp03Note = wstg.Cryp03Note,
                Cryp04Note = wstg.Cryp04Note,
                Cryp01Status = wstg.Cryp01Status,
                Cryp02Status = wstg.Cryp02Status,
                Cryp03Status = wstg.Cryp03Status,
                Cryp04Status = wstg.Cryp04Status
            };
            WSTGErrh errh = new WSTGErrh
            {
                Errh01Note = wstg.Errh01Note,
                Errh02Note = wstg.Errh01Note,
                Errh01Status = wstg.Errh01Status,
                Errh02Status = wstg.Errh02Status
            };
            WSTGIdnt idnt = new WSTGIdnt
            {
                Idnt1Note = wstg.Idnt1Note,
                Idnt2Note = wstg.Idnt2Note,
                Idnt3Note = wstg.Idnt3Note,
                Idnt4Note = wstg.Idnt4Note,
                Idnt5Note = wstg.Idnt5Note,
                Idnt1Status = wstg.Idnt1Status,
                Idnt2Status = wstg.Idnt2Status,
                Idnt3Status = wstg.Idnt3Status,
                Idnt4Status = wstg.Idnt4Status,
                Idnt5Status = wstg.Idnt5Status
            };
            WSTGInpv inpv = new WSTGInpv
            {
                Inpv01Note = wstg.Inpv01Note,
                Inpv02Note = wstg.Inpv02Note,
                Inpv03Note = wstg.Inpv03Note,
                Inpv04Note = wstg.Inpv04Note,
                Inpv05Note = wstg.Inpv05Note,
                Inpv06Note = wstg.Inpv06Note,
                Inpv07Note = wstg.Inpv07Note,
                Inpv08Note = wstg.Inpv08Note,
                Inpv09Note = wstg.Inpv09Note,
                Inpv10Note = wstg.Inpv10Note,
                Inpv11Note = wstg.Inpv11Note,
                Inpv12Note = wstg.Inpv12Note,
                Inpv13Note = wstg.Inpv13Note,
                Inpv14Note = wstg.Inpv14Note,
                Inpv15Note = wstg.Inpv15Note,
                Inpv16Note = wstg.Inpv16Note,
                Inpv17Note = wstg.Inpv17Note,
                Inpv18Note = wstg.Inpv18Note,
                Inpv19Note = wstg.Inpv19Note,
                Inpv01Status = wstg.Inpv01Status,
                Inpv02Status = wstg.Inpv02Status,
                Inpv03Status = wstg.Inpv03Status,
                Inpv04Status = wstg.Inpv04Status,
                Inpv05Status = wstg.Inpv05Status,
                Inpv06Status = wstg.Inpv06Status,
                Inpv07Status = wstg.Inpv07Status,
                Inpv08Status = wstg.Inpv08Status,
                Inpv09Status = wstg.Inpv09Status,
                Inpv10Status = wstg.Inpv10Status,
                Inpv11Status = wstg.Inpv11Status,
                Inpv12Status = wstg.Inpv12Status,
                Inpv13Status = wstg.Inpv13Status,
                Inpv14Status = wstg.Inpv14Status,
                Inpv15Status = wstg.Inpv15Status,
                Inpv16Status = wstg.Inpv16Status,
                Inpv17Status = wstg.Inpv17Status,
                Inpv18Status = wstg.Inpv18Status,
                Inpv19Status = wstg.Inpv19Status
            };
            WSTGSess sess = new WSTGSess
            {
                Sess01Note = wstg.Sess01Note,
                Sess02Note = wstg.Sess02Note,
                Sess03Note = wstg.Sess03Note,
                Sess04Note = wstg.Sess04Note,
                Sess05Note = wstg.Sess05Note,
                Sess06Note = wstg.Sess06Note,
                Sess07Note = wstg.Sess07Note,
                Sess08Note = wstg.Sess08Note,
                Sess09Note = wstg.Sess09Note,
                Sess01Status = wstg.Sess01Status,
                Sess02Status = wstg.Sess02Status,
                Sess03Status = wstg.Sess03Status,
                Sess04Status = wstg.Sess04Status,
                Sess05Status = wstg.Sess05Status,
                Sess06Status = wstg.Sess06Status,
                Sess07Status = wstg.Sess07Status,
                Sess08Status = wstg.Sess08Status,
                Sess09Status = wstg.Sess09Status
            };

            WSTGViewModel model = new WSTGViewModel
            {
                Project = projectManager.GetById(project),
                TargetId = id,
                Info = info,
                Apit = apit,
                Athz = athz,
                Auth = auth,
                Busl = busl,
                Clnt = clnt,
                Conf = conf,
                Cryp = cryp,
                Errh = errh,
                Idnt = idnt,
                Inpv = inpv,
                Sess = sess
            };

            return View(model);
        }
        catch (Exception e)
        {
            TempData["error"] = "Error loading wstg!";
            _logger.LogError(e, "An error ocurred loading Checklist Workspace WSTG form.Project: {0} User: {1} Checklist: {2}", project,
                User.FindFirstValue(ClaimTypes.Name), id);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Wstg(Guid project, WSTGViewModel model)
    {
        try
        {
            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                TempData["userProject"] = "User is not in the project";
                return RedirectToAction("Index", "Workspaces",new {area =""});
            }

            var result = wstgManager.GetTargetId(project, model.TargetId);
            result.Apit01Note = model.Apit.Apit01Note;
            result.Apit01Status = model.Apit.Apit01Status;
            
            result.Athz01Note = model.Athz.Athz01Note;
            result.Athz01Status = model.Athz.Athz01Status;
            result.Athz02Note = model.Athz.Athz02Note;
            result.Athz02Status = model.Athz.Athz02Status;
            result.Athz03Note = model.Athz.Athz03Note;
            result.Athz03Status = model.Athz.Athz03Status;
            result.Athz04Note = model.Athz.Athz04Note;
            result.Athz04Status = model.Athz.Athz04Status;

            result.Athn01Note = model.Auth.Athn01Note;
            result.Athn01Status = model.Auth.Athn01Status;
            result.Athn02Note = model.Auth.Athn02Note;
            result.Athn02Status = model.Auth.Athn02Status;
            result.Athn03Note = model.Auth.Athn03Note;
            result.Athn03Status = model.Auth.Athn03Status;
            result.Athn04Note = model.Auth.Athn04Note;
            result.Athn04Status = model.Auth.Athn04Status;
            result.Athn05Note = model.Auth.Athn05Note;
            result.Athn05Status = model.Auth.Athn05Status;
            result.Athn06Note = model.Auth.Athn06Note;
            result.Athn06Status = model.Auth.Athn06Status;
            result.Athn07Note = model.Auth.Athn07Note;
            result.Athn07Status = model.Auth.Athn07Status;
            result.Athn08Note = model.Auth.Athn08Note;
            result.Athn08Status = model.Auth.Athn08Status;
            result.Athn09Note = model.Auth.Athn09Note;
            result.Athn09Status = model.Auth.Athn09Status;
            result.Athn10Note = model.Auth.Athn10Note;
            result.Athn10Status = model.Auth.Athn10Status;

            result.Busl01Note = model.Busl.Busl01Note;
            result.Busl01Status = model.Busl.Busl01Status;
            result.Busl02Note = model.Busl.Busl02Note;
            result.Busl02Status = model.Busl.Busl02Status;
            result.Busl03Note = model.Busl.Busl03Note;
            result.Busl03Status = model.Busl.Busl03Status;
            result.Busl04Note = model.Busl.Busl04Note;
            result.Busl04Status = model.Busl.Busl04Status;
            result.Busl05Note = model.Busl.Busl05Note;
            result.Busl05Status = model.Busl.Busl05Status;
            result.Busl06Note = model.Busl.Busl06Note;
            result.Busl06Status = model.Busl.Busl06Status;
            result.Busl07Note = model.Busl.Busl07Note;
            result.Busl07Status = model.Busl.Busl07Status;
            result.Busl08Note = model.Busl.Busl08Note;
            result.Busl08Status = model.Busl.Busl08Status;
            result.Busl09Note = model.Busl.Busl09Note;
            result.Busl09Status = model.Busl.Busl09Status;

            result.Clnt01Note = model.Clnt.Clnt01Note;
            result.Clnt01Status = model.Clnt.Clnt01Status;
            result.Clnt02Note = model.Clnt.Clnt02Note;
            result.Clnt02Status = model.Clnt.Clnt02Status;
            result.Clnt03Note = model.Clnt.Clnt03Note;
            result.Clnt03Status = model.Clnt.Clnt03Status;
            result.Clnt04Note = model.Clnt.Clnt04Note;
            result.Clnt04Status = model.Clnt.Clnt04Status;
            result.Clnt05Note = model.Clnt.Clnt05Note;
            result.Clnt05Status = model.Clnt.Clnt05Status;
            result.Clnt06Note = model.Clnt.Clnt06Note;
            result.Clnt06Status = model.Clnt.Clnt06Status;
            result.Clnt07Note = model.Clnt.Clnt07Note;
            result.Clnt07Status = model.Clnt.Clnt07Status;
            result.Clnt08Note = model.Clnt.Clnt08Note;
            result.Clnt08Status = model.Clnt.Clnt08Status;
            result.Clnt09Note = model.Clnt.Clnt09Note;
            result.Clnt09Status = model.Clnt.Clnt09Status;
            result.Clnt10Note = model.Clnt.Clnt10Note;
            result.Clnt10Status = model.Clnt.Clnt10Status;
            result.Clnt11Note = model.Clnt.Clnt11Note;
            result.Clnt11Status = model.Clnt.Clnt11Status;
            result.Clnt12Note = model.Clnt.Clnt12Note;
            result.Clnt12Status = model.Clnt.Clnt12Status;
            result.Clnt13Note = model.Clnt.Clnt13Note;
            result.Clnt13Status = model.Clnt.Clnt13Status;

            result.Conf01Note = model.Conf.Conf01Note;
            result.Conf01Status = model.Conf.Conf01Status;
            result.Conf02Note = model.Conf.Conf02Note;
            result.Conf02Status = model.Conf.Conf02Status;
            result.Conf03Note = model.Conf.Conf03Note;
            result.Conf03Status = model.Conf.Conf03Status;
            result.Conf04Note = model.Conf.Conf04Note;
            result.Conf04Status = model.Conf.Conf04Status;
            result.Conf05Note = model.Conf.Conf05Note;
            result.Conf05Status = model.Conf.Conf05Status;
            result.Conf06Note = model.Conf.Conf06Note;
            result.Conf06Status = model.Conf.Conf06Status;
            result.Conf07Note = model.Conf.Conf07Note;
            result.Conf07Status = model.Conf.Conf07Status;
            result.Conf08Note = model.Conf.Conf08Note;
            result.Conf08Status = model.Conf.Conf08Status;
            result.Conf09Note = model.Conf.Conf09Note;
            result.Conf09Status = model.Conf.Conf09Status;
            result.Conf10Note = model.Conf.Conf10Note;
            result.Conf10Status = model.Conf.Conf10Status;
            result.Conf11Note = model.Conf.Conf11Note;
            result.Conf11Status = model.Conf.Conf11Status;

            result.Cryp01Note = model.Cryp.Cryp01Note;
            result.Cryp01Status = model.Cryp.Cryp01Status;
            result.Cryp02Note = model.Cryp.Cryp02Note;
            result.Cryp02Status = model.Cryp.Cryp02Status;
            result.Cryp03Note = model.Cryp.Cryp03Note;
            result.Cryp03Status = model.Cryp.Cryp03Status;
            result.Cryp04Note = model.Cryp.Cryp04Note;
            result.Cryp04Status = model.Cryp.Cryp04Status;

            result.Errh01Note = model.Errh.Errh01Note;
            result.Errh01Status = model.Errh.Errh01Status;
            result.Errh02Note = model.Errh.Errh02Note;
            result.Errh02Status = model.Errh.Errh02Status;

            result.Idnt1Note = model.Idnt.Idnt1Note;
            result.Idnt1Status = model.Idnt.Idnt1Status;
            result.Idnt2Note = model.Idnt.Idnt2Note;
            result.Idnt2Status = model.Idnt.Idnt2Status;
            result.Idnt3Note = model.Idnt.Idnt3Note;
            result.Idnt3Status = model.Idnt.Idnt3Status;
            result.Idnt4Note = model.Idnt.Idnt4Note;
            result.Idnt4Status = model.Idnt.Idnt4Status;
            result.Idnt5Note = model.Idnt.Idnt5Note;
            result.Idnt5Status = model.Idnt.Idnt5Status;

            result.Info01Note = model.Info.Info01Note;
            result.Info01Status = model.Info.Info01Status;
            result.Info02Note = model.Info.Info02Note;
            result.Info02Status = model.Info.Info02Status;
            result.Info03Note = model.Info.Info03Note;
            result.Info03Status = model.Info.Info03Status;
            result.Info04Note = model.Info.Info04Note;
            result.Info04Status = model.Info.Info04Status;
            result.Info05Note = model.Info.Info05Note;
            result.Info05Status = model.Info.Info05Status;
            result.Info06Note = model.Info.Info06Note;
            result.Info06Status = model.Info.Info06Status;
            result.Info07Note = model.Info.Info07Note;
            result.Info07Status = model.Info.Info07Status;
            result.Info08Note = model.Info.Info08Note;
            result.Info08Status = model.Info.Info08Status;
            result.Info09Note = model.Info.Info09Note;
            result.Info09Status = model.Info.Info09Status;
            result.Info10Note = model.Info.Info10Note;
            result.Info10Status = model.Info.Info10Status;

            result.Inpv01Note = model.Inpv.Inpv01Note;
            result.Inpv01Status = model.Inpv.Inpv01Status;
            result.Inpv02Note = model.Inpv.Inpv02Note;
            result.Inpv02Status = model.Inpv.Inpv02Status;
            result.Inpv03Note = model.Inpv.Inpv03Note;
            result.Inpv03Status = model.Inpv.Inpv03Status;
            result.Inpv04Note = model.Inpv.Inpv04Note;
            result.Inpv04Status = model.Inpv.Inpv04Status;
            result.Inpv05Note = model.Inpv.Inpv05Note;
            result.Inpv05Status = model.Inpv.Inpv05Status;
            result.Inpv06Note = model.Inpv.Inpv06Note;
            result.Inpv06Status = model.Inpv.Inpv06Status;
            result.Inpv07Note = model.Inpv.Inpv07Note;
            result.Inpv07Status = model.Inpv.Inpv07Status;
            result.Inpv08Note = model.Inpv.Inpv08Note;
            result.Inpv08Status = model.Inpv.Inpv08Status;
            result.Inpv09Note = model.Inpv.Inpv09Note;
            result.Inpv09Status = model.Inpv.Inpv09Status;
            result.Inpv10Note = model.Inpv.Inpv10Note;
            result.Inpv10Status = model.Inpv.Inpv10Status;
            result.Inpv11Note = model.Inpv.Inpv11Note;
            result.Inpv11Status = model.Inpv.Inpv11Status;
            result.Inpv12Note = model.Inpv.Inpv12Note;
            result.Inpv12Status = model.Inpv.Inpv12Status;
            result.Inpv13Note = model.Inpv.Inpv13Note;
            result.Inpv13Status = model.Inpv.Inpv13Status;
            result.Inpv14Note = model.Inpv.Inpv14Note;
            result.Inpv14Status = model.Inpv.Inpv14Status;
            result.Inpv15Note = model.Inpv.Inpv15Note;
            result.Inpv15Status = model.Inpv.Inpv15Status;
            result.Inpv16Note = model.Inpv.Inpv16Note;
            result.Inpv16Status = model.Inpv.Inpv16Status;
            result.Inpv17Note = model.Inpv.Inpv17Note;
            result.Inpv17Status = model.Inpv.Inpv17Status;
            result.Inpv18Note = model.Inpv.Inpv18Note;
            result.Inpv18Status = model.Inpv.Inpv18Status;
            result.Inpv19Note = model.Inpv.Inpv19Note;
            result.Inpv19Status = model.Inpv.Inpv19Status;

            result.Sess01Note = model.Sess.Sess01Note;
            result.Sess01Status = model.Sess.Sess01Status;
            result.Sess02Note = model.Sess.Sess02Note;
            result.Sess02Status = model.Sess.Sess02Status;
            result.Sess03Note = model.Sess.Sess03Note;
            result.Sess03Status = model.Sess.Sess03Status;
            result.Sess04Note = model.Sess.Sess04Note;
            result.Sess04Status = model.Sess.Sess04Status;
            result.Sess05Note = model.Sess.Sess05Note;
            result.Sess05Status = model.Sess.Sess05Status;
            result.Sess06Note = model.Sess.Sess06Note;
            result.Sess06Status = model.Sess.Sess06Status;
            result.Sess07Note = model.Sess.Sess07Note;
            result.Sess07Status = model.Sess.Sess07Status;
            result.Sess08Note = model.Sess.Sess08Note;
            result.Sess08Status = model.Sess.Sess08Status;
            result.Sess09Note = model.Sess.Sess09Note;
            result.Sess09Status = model.Sess.Sess09Status;

            wstgManager.Context.SaveChanges();
            TempData["edited"] = "Checklist edited successfully!";

            _logger.LogInformation("User: {0} edited Checklist: {1} on Project: {2}", User.FindFirstValue(ClaimTypes.Name), result.Id,
                project);
            return RedirectToAction("Index");
        }
        catch (Exception e)
        {
            TempData["error"] = "Error loading wstg!";
            _logger.LogError(e, "An error ocurred loading Checlist Workspace WSTG form. Project: {0} User: {1}", project,
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction(nameof(Index));
        }
        
    }

    public IActionResult Create(Guid project)
    {
        try
        {
            
            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                TempData["userProject"] = "User is not in the project";
                return RedirectToAction("Index", "Workspaces",new {area =""});
            }

        
            var result = targetManager.GetAll().Where(x => x.ProjectId == project).Select(e => new VulnCreateViewModel
            {
                TargetId = e.Id,
                TargetName = e.Name
            }).ToList();

            var targets = new List<SelectListItem>();

            foreach (var item in result)
                targets.Add(new SelectListItem {Text = item.TargetName, Value = item.TargetId.ToString()});
            
            var model = new ChecklistCreateViewModel
            {
                TargetList = targets,
                Project = projectManager.GetById(project),
            };

            return View(model);
        }
        catch (Exception e)
        {
            TempData["error"] = "Error loading create form!";
            _logger.LogError(e, "An error ocurred loading Checklist Workspace create form.Project: {0} User: {1}", project,
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction(nameof(Index));
        }
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Guid project, ChecklistCreateViewModel model)
    {
        try
        {
            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                TempData["userProject"] = "User is not in the project";
                return RedirectToAction("Index", "Workspaces",new {area =""});
            }
            
            var result = targetManager.GetAll().Where(x => x.ProjectId == project).Select(e => new VulnCreateViewModel
            {
                TargetId = e.Id,
                TargetName = e.Name
            }).ToList();

            switch (model.Type)
            {
                case ChecklistType.OWASPWSTG:
                    WSTG wstg = new WSTG
                    {
                        TargetId = model.TargetId,
                        UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                        ProjectId = project,
                        CreatedDate = DateTime.Now.ToUniversalTime()

                    };
                    wstgManager.Add(wstg);
                    wstgManager.Context.SaveChanges();
                    TempData["addedCheck"] = "Checklist added successfully!";

                    _logger.LogInformation("User: {0} added a new checklist on Project: {1}", User.FindFirstValue(ClaimTypes.Name),
                        project);
                    return RedirectToAction(nameof(Index));
                case ChecklistType.OWASPMASVS:
                    MASTG mstg = new MASTG
                    {
                        TargetId = model.TargetId,
                        UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                        ProjectId = project,
                        CreatedDate = DateTime.Now.ToUniversalTime()

                    };
                    mastgManager.Add(mstg);
                    mastgManager.Context.SaveChanges();
                    TempData["addedCheck"] = "Checklist added successfully!";

                    _logger.LogInformation("User: {0} added a new checklist on Project: {1}", User.FindFirstValue(ClaimTypes.Name),
                        project);
                    return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {
            TempData["error"] = "Error loading create form!";
            _logger.LogError(e, "An error ocurred loading Checklist Workspace create form.Project: {0} User: {1}", project,
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction(nameof(Index));
        }
    }

    public IActionResult Delete(Guid project, Guid id, ChecklistType type)
    {
        try
        {
            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                TempData["userProject"] = "User is not in the project";
                return RedirectToAction("Index", "Workspaces",new {area =""});
            }

            switch (type)
            {
                case ChecklistType.OWASPWSTG:
                    var result = wstgManager.GetById(id);
                    if (result != null)
                    {
                        var model = new ChecklistDeleteViewModel
                        {
                            Project = projectManager.GetById(project),
                            Type = ChecklistType.OWASPWSTG,
                            Wstg = result
                        };
                        return View(model);
                    }
                    else
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    
                case ChecklistType.OWASPMASVS:
                    var result2 = mastgManager.GetById(id);
                    if (result2 != null)
                    {
                        var model = new ChecklistDeleteViewModel
                        {
                            Project = projectManager.GetById(project),
                            Type = ChecklistType.OWASPMASVS,
                            Mastg = result2
                        };
                        return View(model);
                    }
                    else
                    {
                        return RedirectToAction(nameof(Index));
                    }
            }

            return RedirectToAction(nameof(Index));
            
        }
        catch (Exception e)
        {
            TempData["error"] = "Error loading create form!";
            _logger.LogError(e, "An error ocurred loading Checklist Workspace delete form.Project: {0} User: {1}", project,
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(Guid project, ChecklistDeleteViewModel model)
    {
        try
        {
            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                TempData["userProject"] = "User is not in the project";
                return RedirectToAction("Index", "Workspaces",new {area =""});
            }

            switch (model.Type)
            {
                case ChecklistType.OWASPWSTG:
                    var result = wstgManager.GetById(model.Wstg.Id);
                    wstgManager.Remove(result);
                    wstgManager.Context.SaveChanges();
                    TempData["deleted"] = "Checklist deleted!";
                    _logger.LogInformation("A Checklist has been deleted.Project: {0} User: {1} Checklist: {2}", project,
                        User.FindFirstValue(ClaimTypes.Name), result.Id);
                    
                    return RedirectToAction(nameof(Index));
                case ChecklistType.OWASPMASVS:
                    return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
            
        }
        catch (Exception e)
        {
            TempData["error"] = "Error loading create form!";
            _logger.LogError(e, "An error ocurred loading Checklist Workspace delete form.Project: {0} User: {1}", project,
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction(nameof(Index));
        }
    }

    public IActionResult Mastg(Guid project)
    {
        try
        {
            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                TempData["userProject"] = "User is not in the project";
                return RedirectToAction("Index", "Workspaces", new {area = ""});
            }

            MASTGViewModel model = new MASTGViewModel
            {
                Project = projectManager.GetById(project),
            };
            return View(model);
        }
        catch (Exception e)
        {
            TempData["error"] = "Error loading create form!";
            _logger.LogError(e, "An error ocurred loading Checklist Workspace MASTG form.Project: {0} User: {1}", project,
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction(nameof(Index));
        }
    }
}