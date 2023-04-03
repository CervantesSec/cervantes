using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.Web.Areas.Workspace.Models;
using Cervantes.Web.Areas.Workspace.Models.MASTG;
using Cervantes.Web.Areas.Workspace.Models.Wstg;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using HtmlToOpenXml;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Table = DocumentFormat.OpenXml.Wordprocessing.Table;

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
    private IOrganizationManager organizationManager = null;
    private readonly IHostingEnvironment _appEnvironment;
    
    public ChecklistController(IProjectManager projectManager, 
        IProjectUserManager projectUserManager, ITargetManager targetManager, ILogger<ChecklistController> logger, IWSTGManager wstgManager,
        IMASTGManager mastgManager,IHostingEnvironment _appEnvironment, IOrganizationManager organizationManager)
    {
        this.projectManager = projectManager;
        this.projectUserManager = projectUserManager;
        this.targetManager = targetManager;
        _logger = logger;
        this.wstgManager = wstgManager;
        this.mastgManager = mastgManager;
        this._appEnvironment = _appEnvironment;
        this.organizationManager = organizationManager;
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
            TempData["errorLaodingWSTG"] = "Error loading wstg!";
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
            TempData["editedWSTG"] = "Checklist edited successfully!";

            _logger.LogInformation("User: {0} edited Checklist: {1} on Project: {2}", User.FindFirstValue(ClaimTypes.Name), result.Id,
                project);
            return RedirectToAction("Wstg", new {id = model.TargetId});
        }
        catch (Exception e)
        {
            TempData["errorSaveWSTG"] = "Error loading wstg!";
            _logger.LogError(e, "An error ocurred loading Checlist Workspace WSTG form. Project: {0} User: {1}", project,
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Wstg", new {id = model.TargetId});
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
            TempData["errorCreateChecklist"] = "Error loading create form!";
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
            TempData["errorChecklist"] = "Error loading create form!";
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
                    TempData["deletedWSTG"] = "Checklist deleted!";
                    _logger.LogInformation("A Checklist has been deleted.Project: {0} User: {1} Checklist: {2}", project,
                        User.FindFirstValue(ClaimTypes.Name), result.Id);
                    
                    return RedirectToAction(nameof(Index));
                case ChecklistType.OWASPMASVS:
                    var result2 = mastgManager.GetById(model.Mastg.Id);
                    mastgManager.Remove(result2);
                    mastgManager.Context.SaveChanges();
                    TempData["deletedMASTG"] = "Checklist deleted!";
                    _logger.LogInformation("A Checklist has been deleted.Project: {0} User: {1} Checklist: {2}", project,
                        User.FindFirstValue(ClaimTypes.Name), result2.Id);
                    return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
            
        }
        catch (Exception e)
        {
            TempData["errorCheckDelete"] = "Error loading create form!";
            _logger.LogError(e, "An error ocurred loading Checklist Workspace delete form.Project: {0} User: {1}", project,
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction(nameof(Index));
        }
    }

    public IActionResult Mastg(Guid project, Guid id)
    {
        try
        {
            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                TempData["userProject"] = "User is not in the project";
                return RedirectToAction("Index", "Workspaces", new {area = ""});
            }
            var mastg = mastgManager.GetTargetId(project, id);

            MASTGArch arch = new MASTGArch
            {
                ArchNote01 = mastg.ArchNote01,
                ArchNote02 = mastg.ArchNote02,
                ArchNote03 = mastg.ArchNote03,
                ArchNote04 = mastg.ArchNote04,
                ArchNote05 = mastg.ArchNote05,
                ArchNote06 = mastg.ArchNote06,
                ArchNote07 = mastg.ArchNote07,
                ArchNote08 = mastg.ArchNote08,
                ArchNote09 = mastg.ArchNote09,
                ArchNote10 = mastg.ArchNote10,
                ArchNote11 = mastg.ArchNote11,
                ArchNote12 = mastg.ArchNote12,
                ArchStatus01 = mastg.ArchStatus01,
                ArchStatus02 = mastg.ArchStatus02,
                ArchStatus03 = mastg.ArchStatus03,
                ArchStatus04 = mastg.ArchStatus04,
                ArchStatus05 = mastg.ArchStatus05,
                ArchStatus06 = mastg.ArchStatus06,
                ArchStatus07 = mastg.ArchStatus07,
                ArchStatus08 = mastg.ArchStatus08,
                ArchStatus09 = mastg.ArchStatus09,
                ArchStatus10 = mastg.ArchStatus10,
                ArchStatus11 = mastg.ArchStatus11,
                ArchStatus12 = mastg.ArchStatus12
            };

            MASTGAuth auth = new MASTGAuth
            {
                AuthNote01 = mastg.AuthNote01,
                AuthNote02 = mastg.AuthNote02,
                AuthNote03 = mastg.AuthNote03,
                AuthNote04 = mastg.AuthNote04,
                AuthNote05 = mastg.AuthNote05,
                AuthNote06 = mastg.AuthNote06,
                AuthNote07 = mastg.AuthNote07,
                AuthNote08 = mastg.AuthNote08,
                AuthNote09 = mastg.AuthNote09,
                AuthNote10 = mastg.AuthNote10,
                AuthNote11 = mastg.AuthNote11,
                AuthNote12 = mastg.AuthNote12,
                AuthStatus01 = mastg.AuthStatus01,
                AuthStatus02 = mastg.AuthStatus02,
                AuthStatus03 = mastg.AuthStatus03,
                AuthStatus04 = mastg.AuthStatus04,
                AuthStatus05 = mastg.AuthStatus05,
                AuthStatus06 = mastg.AuthStatus06,
                AuthStatus07 = mastg.AuthStatus07,
                AuthStatus08 = mastg.AuthStatus08,
                AuthStatus09 = mastg.AuthStatus09,
                AuthStatus10 = mastg.AuthStatus10,
                AuthStatus11 = mastg.AuthStatus11,
                AuthStatus12 = mastg.AuthStatus12
            };

            MASTGCode code = new MASTGCode
            {
                CodeNote01 = mastg.CodeNote01,
                CodeNote02 = mastg.CodeNote02,
                CodeNote03 = mastg.CodeNote03,
                CodeNote04 = mastg.CodeNote04,
                CodeNote05 = mastg.CodeNote05,
                CodeNote06 = mastg.CodeNote06,
                CodeNote07 = mastg.CodeNote07,
                CodeNote08 = mastg.CodeNote08,
                CodeNote09 = mastg.CodeNote09,
                CodeStatus01 = mastg.CodeStatus01,
                CodeStatus02 = mastg.CodeStatus02,
                CodeStatus03 = mastg.CodeStatus03,
                CodeStatus04 = mastg.CodeStatus04,
                CodeStatus05 = mastg.CodeStatus05,
                CodeStatus06 = mastg.CodeStatus06,
                CodeStatus07 = mastg.CodeStatus07,
                CodeStatus08 = mastg.CodeStatus08,
                CodeStatus09 = mastg.CodeStatus09
            };

            MASTGCrypto crypto = new MASTGCrypto
            {
                CryptoNote01 = mastg.CryptoNote01,
                CryptoNote02 = mastg.CryptoNote02,
                CryptoNote03 = mastg.CryptoNote03,
                CryptoNote04 = mastg.CryptoNote04,
                CryptoNote05 = mastg.CryptoNote05,
                CryptoNote06 = mastg.CryptoNote06,
                CryptoStatus01 = mastg.CryptoStatus01,
                CryptoStatus02 = mastg.CryptoStatus02,
                CryptoStatus03 = mastg.CryptoStatus03,
                CryptoStatus04 = mastg.CryptoStatus04,
                CryptoStatus05 = mastg.CryptoStatus05,
                CryptoStatus06 = mastg.CryptoStatus06
            };

            MASTGNetwork network = new MASTGNetwork
            {
                NetworkNote01 = mastg.NetworkNote01,
                NetworkNote02 = mastg.NetworkNote02,
                NetworkNote03 = mastg.NetworkNote03,
                NetworkNote04 = mastg.NetworkNote04,
                NetworkNote05 = mastg.NetworkNote05,
                NetworkNote06 = mastg.NetworkNote06,
                NetworkStatus01 = mastg.NetworkStatus01,
                NetworkStatus02 = mastg.NetworkStatus02,
                NetworkStatus03 = mastg.NetworkStatus03,
                NetworkStatus04 = mastg.NetworkStatus04,
                NetworkStatus05 = mastg.NetworkStatus05,
                NetworkStatus06 = mastg.NetworkStatus06,
            };
            MASTGPlatform platform = new MASTGPlatform
            {
                PlatformNote01 = mastg.PlatformNote01,
                PlatformNote02 = mastg.PlatformNote02,
                PlatformNote03 = mastg.PlatformNote03,
                PlatformNote04 = mastg.PlatformNote04,
                PlatformNote05 = mastg.PlatformNote05,
                PlatformNote06 = mastg.PlatformNote06,
                PlatformNote07 = mastg.PlatformNote07,
                PlatformNote08 = mastg.PlatformNote08,
                PlatformNote09 = mastg.PlatformNote09,
                PlatformNote10 = mastg.PlatformNote10,
                PlatformNote11 = mastg.PlatformNote11,
                PlatformStatus01 = mastg.PlatformStatus01,
                PlatformStatus02 = mastg.PlatformStatus02,
                PlatformStatus03 = mastg.PlatformStatus03,
                PlatformStatus04 = mastg.PlatformStatus04,
                PlatformStatus05 = mastg.PlatformStatus05,
                PlatformStatus06 = mastg.PlatformStatus06,
                PlatformStatus07 = mastg.PlatformStatus07,
                PlatformStatus08 = mastg.PlatformStatus08,
                PlatformStatus09 = mastg.PlatformStatus09,
                PlatformStatus10 = mastg.PlatformStatus10,
                PlatformStatus11 = mastg.PlatformStatus11
            };

            MASTGResilience resilience = new MASTGResilience
            {
                ResilienceNote01 = mastg.ResilienceNote01,
                ResilienceNote02 = mastg.ResilienceNote02,
                ResilienceNote03 = mastg.ResilienceNote03,
                ResilienceNote04 = mastg.ResilienceNote04,
                ResilienceNote05 = mastg.ResilienceNote05,
                ResilienceNote06 = mastg.ResilienceNote06,
                ResilienceNote07 = mastg.ResilienceNote07,
                ResilienceNote08 = mastg.ResilienceNote08,
                ResilienceNote09 = mastg.ResilienceNote09,
                ResilienceNote10 = mastg.ResilienceNote10,
                ResilienceNote11 = mastg.ResilienceNote11,
                ResilienceNote12 = mastg.ResilienceNote12,
                ResilienceNote13 = mastg.ResilienceNote13,
                ResilienceStatus01 = mastg.ResilienceStatus01,
                ResilienceStatus02 = mastg.ResilienceStatus02,
                ResilienceStatus03 = mastg.ResilienceStatus03,
                ResilienceStatus04 = mastg.ResilienceStatus04,
                ResilienceStatus05 = mastg.ResilienceStatus05,
                ResilienceStatus06 = mastg.ResilienceStatus06,
                ResilienceStatus07 = mastg.ResilienceStatus07,
                ResilienceStatus08 = mastg.ResilienceStatus08,
                ResilienceStatus09 = mastg.ResilienceStatus09,
                ResilienceStatus10 = mastg.ResilienceStatus10,
                ResilienceStatus11 = mastg.ResilienceStatus11,
                ResilienceStatus12 = mastg.ResilienceStatus12,
                ResilienceStatus13 = mastg.ResilienceStatus13
            };

            MASTGStorage storage = new MASTGStorage
            {
                StorageNote01 = mastg.StorageNote01,
                StorageNote02 = mastg.StorageNote02,
                StorageNote03 = mastg.StorageNote03,
                StorageNote04 = mastg.StorageNote04,
                StorageNote05 = mastg.StorageNote05,
                StorageNote06 = mastg.StorageNote06,
                StorageNote07 = mastg.StorageNote07,
                StorageNote08 = mastg.StorageNote08,
                StorageNote09 = mastg.StorageNote09,
                StorageNote10 = mastg.StorageNote10,
                StorageNote11 = mastg.StorageNote11,
                StorageNote12 = mastg.StorageNote12,
                StorageNote13 = mastg.StorageNote13,
                StorageNote14 = mastg.StorageNote14,
                StorageNote15 = mastg.StorageNote15,
                StorageStatus01 = mastg.StorageStatus01,
                StorageStatus02 = mastg.StorageStatus02,
                StorageStatus03 = mastg.StorageStatus03,
                StorageStatus04 = mastg.StorageStatus04,
                StorageStatus05 = mastg.StorageStatus05,
                StorageStatus06 = mastg.StorageStatus06,
                StorageStatus07 = mastg.StorageStatus07,
                StorageStatus08 = mastg.StorageStatus08,
                StorageStatus09 = mastg.StorageStatus09,
                StorageStatus10 = mastg.StorageStatus10,
                StorageStatus11 = mastg.StorageStatus11,
                StorageStatus12 = mastg.StorageStatus12,
                StorageStatus13 = mastg.StorageStatus13,
                StorageStatus14 = mastg.StorageStatus14,
                StorageStatus15 = mastg.StorageStatus15
            };


            MASTGViewModel model = new MASTGViewModel
            {
                Project = projectManager.GetById(project),
                Arch = arch,
                Auth = auth,
                Code = code,
                Crypto = crypto,
                Network = network,
                Platform = platform,
                Resilience = resilience,
                Storage = storage,
                TargetId = id
            };
            return View(model);
        }
        catch (Exception e)
        {
            TempData["errorLaodingMASTG"] = "Error loading create form!";
            _logger.LogError(e, "An error ocurred loading Checklist Workspace MASTG form.Project: {0} User: {1}", project,
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction(nameof(Index));
        }
    }
    
     [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Mastg(Guid project, MASTGViewModel model)
    {
        try
        {
            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                TempData["userProject"] = "User is not in the project";
                return RedirectToAction("Index", "Workspaces",new {area =""});
            }

            var result = mastgManager.GetTargetId(project, model.TargetId);
            
            result.ArchNote01 = model.Arch.ArchNote01;
            result.ArchNote02= model.Arch.ArchNote02;
            result.ArchNote03= model.Arch.ArchNote03;
            result.ArchNote04= model.Arch.ArchNote04;
            result.ArchNote05= model.Arch.ArchNote05;
            result.ArchNote06= model.Arch.ArchNote06;
            result.ArchNote07= model.Arch.ArchNote07;
            result.ArchNote08= model.Arch.ArchNote08;
            result.ArchNote09= model.Arch.ArchNote09;
            result.ArchNote10= model.Arch.ArchNote10;
            result.ArchNote11= model.Arch.ArchNote11;
            result.ArchNote12= model.Arch.ArchNote12;
            result.ArchStatus01 = model.Arch.ArchStatus01;
            result.ArchStatus02= model.Arch.ArchStatus02;
            result.ArchStatus03= model.Arch.ArchStatus03;
            result.ArchStatus04= model.Arch.ArchStatus04;
            result.ArchStatus05= model.Arch.ArchStatus05;
            result.ArchStatus06= model.Arch.ArchStatus06;
            result.ArchStatus07= model.Arch.ArchStatus07;
            result.ArchStatus08= model.Arch.ArchStatus08;
            result.ArchStatus09= model.Arch.ArchStatus09;
            result.ArchStatus10= model.Arch.ArchStatus10;
            result.ArchStatus11= model.Arch.ArchStatus11;
            result.ArchStatus12= model.Arch.ArchStatus12;
            
            result.StorageNote01 = model.Storage.StorageNote01;
            result.StorageNote02= model.Storage.StorageNote02;
            result.StorageNote03= model.Storage.StorageNote03;
            result.StorageNote04= model.Storage.StorageNote04;
            result.StorageNote05= model.Storage.StorageNote05;
            result.StorageNote06= model.Storage.StorageNote06;
            result.StorageNote07= model.Storage.StorageNote07;
            result.StorageNote08= model.Storage.StorageNote08;
            result.StorageNote09= model.Storage.StorageNote09;
            result.StorageNote10= model.Storage.StorageNote10;
            result.StorageNote11= model.Storage.StorageNote11;
            result.StorageNote12= model.Storage.StorageNote12;
            result.StorageNote13= model.Storage.StorageNote13;
            result.StorageNote14= model.Storage.StorageNote14;
            result.StorageNote15= model.Storage.StorageNote15;
            result.StorageStatus01 = model.Storage.StorageStatus01;
            result.StorageStatus02= model.Storage.StorageStatus02;
            result.StorageStatus03= model.Storage.StorageStatus03;
            result.StorageStatus04= model.Storage.StorageStatus04;
            result.StorageStatus05= model.Storage.StorageStatus05;
            result.StorageStatus06= model.Storage.StorageStatus06;
            result.StorageStatus07= model.Storage.StorageStatus07;
            result.StorageStatus08= model.Storage.StorageStatus08;
            result.StorageStatus09= model.Storage.StorageStatus09;
            result.StorageStatus10= model.Storage.StorageStatus10;
            result.StorageStatus11= model.Storage.StorageStatus11;
            result.StorageStatus12= model.Storage.StorageStatus12;
            result.StorageStatus13= model.Storage.StorageStatus13;
            result.StorageStatus14= model.Storage.StorageStatus14;
            result.StorageStatus15= model.Storage.StorageStatus15;
            
            result.CryptoNote01 = model.Crypto.CryptoNote01;
            result.CryptoNote02= model.Crypto.CryptoNote02;
            result.CryptoNote03= model.Crypto.CryptoNote03;
            result.CryptoNote04= model.Crypto.CryptoNote04;
            result.CryptoNote05= model.Crypto.CryptoNote05;
            result.CryptoNote06= model.Crypto.CryptoNote06;
            result.CryptoStatus01 = model.Crypto.CryptoStatus01;
            result.CryptoStatus02= model.Crypto.CryptoStatus02;
            result.CryptoStatus03= model.Crypto.CryptoStatus03;
            result.CryptoStatus04= model.Crypto.CryptoStatus04;
            result.CryptoStatus05= model.Crypto.CryptoStatus05;
            result.CryptoStatus06= model.Crypto.CryptoStatus06;
            
            result.AuthNote01 = model.Auth.AuthNote01;
            result.AuthNote02= model.Auth.AuthNote02;
            result.AuthNote03= model.Auth.AuthNote03;
            result.AuthNote04= model.Auth.AuthNote04;
            result.AuthNote05= model.Auth.AuthNote05;
            result.AuthNote06= model.Auth.AuthNote06;
            result.AuthNote07= model.Auth.AuthNote07;
            result.AuthNote08= model.Auth.AuthNote08;
            result.AuthNote09= model.Auth.AuthNote09;
            result.AuthNote10= model.Auth.AuthNote10;
            result.AuthNote11= model.Auth.AuthNote11;
            result.AuthNote12= model.Auth.AuthNote12;
            result.AuthStatus01 = model.Auth.AuthStatus01;
            result.AuthStatus02= model.Auth.AuthStatus02;
            result.AuthStatus03= model.Auth.AuthStatus03;
            result.AuthStatus04= model.Auth.AuthStatus04;
            result.AuthStatus05= model.Auth.AuthStatus05;
            result.AuthStatus06= model.Auth.AuthStatus06;
            result.AuthStatus07= model.Auth.AuthStatus07;
            result.AuthStatus08= model.Auth.AuthStatus08;
            result.AuthStatus09= model.Auth.AuthStatus09;
            result.AuthStatus10= model.Auth.AuthStatus10;
            result.AuthStatus11= model.Auth.AuthStatus11;
            result.AuthStatus12= model.Auth.AuthStatus12;
            
            result.NetworkNote01 = model.Network.NetworkNote01;
            result.NetworkNote02= model.Network.NetworkNote02;
            result.NetworkNote03= model.Network.NetworkNote03;
            result.NetworkNote04= model.Network.NetworkNote04;
            result.NetworkNote05= model.Network.NetworkNote05;
            result.NetworkNote06= model.Network.NetworkNote06;
            result.NetworkStatus01 = model.Network.NetworkStatus01;
            result.NetworkStatus02= model.Network.NetworkStatus02;
            result.NetworkStatus03= model.Network.NetworkStatus03;
            result.NetworkStatus04= model.Network.NetworkStatus04;
            result.NetworkStatus05= model.Network.NetworkStatus05;
            result.NetworkStatus06= model.Network.NetworkStatus06;
            
            result.CodeNote01 = model.Code.CodeNote01;
            result.CodeNote02= model.Code.CodeNote02;
            result.CodeNote03= model.Code.CodeNote03;
            result.CodeNote04= model.Code.CodeNote04;
            result.CodeNote05= model.Code.CodeNote05;
            result.CodeNote06= model.Code.CodeNote06;
            result.CodeNote07= model.Code.CodeNote07;
            result.CodeNote08= model.Code.CodeNote08;
            result.CodeNote09= model.Code.CodeNote09;
            result.CodeStatus01 = model.Code.CodeStatus01;
            result.CodeStatus02= model.Code.CodeStatus02;
            result.CodeStatus03= model.Code.CodeStatus03;
            result.CodeStatus04= model.Code.CodeStatus04;
            result.CodeStatus05= model.Code.CodeStatus05;
            result.CodeStatus06= model.Code.CodeStatus06;
            result.CodeStatus07= model.Code.CodeStatus07;
            result.CodeStatus08= model.Code.CodeStatus08;
            result.CodeStatus09= model.Code.CodeStatus09;
            
            result.ResilienceNote01 = model.Resilience.ResilienceNote01;
            result.ResilienceNote02= model.Resilience.ResilienceNote02;
            result.ResilienceNote03= model.Resilience.ResilienceNote03;
            result.ResilienceNote04= model.Resilience.ResilienceNote04;
            result.ResilienceNote05= model.Resilience.ResilienceNote05;
            result.ResilienceNote06= model.Resilience.ResilienceNote06;
            result.ResilienceNote07= model.Resilience.ResilienceNote07;
            result.ResilienceNote08= model.Resilience.ResilienceNote08;
            result.ResilienceNote09= model.Resilience.ResilienceNote09;
            result.ResilienceNote10= model.Resilience.ResilienceNote10;
            result.ResilienceNote11= model.Resilience.ResilienceNote11;
            result.ResilienceNote12= model.Resilience.ResilienceNote12;
            result.ResilienceNote13= model.Resilience.ResilienceNote13;
            result.ResilienceStatus01 = model.Resilience.ResilienceStatus01;
            result.ResilienceStatus02= model.Resilience.ResilienceStatus02;
            result.ResilienceStatus03= model.Resilience.ResilienceStatus03;
            result.ResilienceStatus04= model.Resilience.ResilienceStatus04;
            result.ResilienceStatus05= model.Resilience.ResilienceStatus05;
            result.ResilienceStatus06= model.Resilience.ResilienceStatus06;
            result.ResilienceStatus07= model.Resilience.ResilienceStatus07;
            result.ResilienceStatus08= model.Resilience.ResilienceStatus08;
            result.ResilienceStatus09= model.Resilience.ResilienceStatus09;
            result.ResilienceStatus10= model.Resilience.ResilienceStatus10;
            result.ResilienceStatus11= model.Resilience.ResilienceStatus11;
            result.ResilienceStatus12= model.Resilience.ResilienceStatus12;
            result.ResilienceStatus13= model.Resilience.ResilienceStatus13;
            
            result.PlatformNote01 = model.Platform.PlatformNote01;
            result.PlatformNote02= model.Platform.PlatformNote02;
            result.PlatformNote03= model.Platform.PlatformNote03;
            result.PlatformNote04= model.Platform.PlatformNote04;
            result.PlatformNote05= model.Platform.PlatformNote05;
            result.PlatformNote06= model.Platform.PlatformNote06;
            result.PlatformNote07= model.Platform.PlatformNote07;
            result.PlatformNote08= model.Platform.PlatformNote08;
            result.PlatformNote09= model.Platform.PlatformNote09;
            result.PlatformNote10= model.Platform.PlatformNote10;
            result.PlatformNote11= model.Platform.PlatformNote11;
            result.PlatformStatus01 = model.Platform.PlatformStatus01;
            result.PlatformStatus02= model.Platform.PlatformStatus02;
            result.PlatformStatus03= model.Platform.PlatformStatus03;
            result.PlatformStatus04= model.Platform.PlatformStatus04;
            result.PlatformStatus05= model.Platform.PlatformStatus05;
            result.PlatformStatus06= model.Platform.PlatformStatus06;
            result.PlatformStatus07= model.Platform.PlatformStatus07;
            result.PlatformStatus08= model.Platform.PlatformStatus08;
            result.PlatformStatus09= model.Platform.PlatformStatus09;
            result.PlatformStatus10= model.Platform.PlatformStatus10;
            result.PlatformStatus11= model.Platform.PlatformStatus11;
            
            mastgManager.Context.SaveChanges();
            TempData["editedMASTG"] = "Checklist edited successfully!";

            _logger.LogInformation("User: {0} edited Checklist: {1} on Project: {2}", User.FindFirstValue(ClaimTypes.Name), result.Id,
                project);
            return RedirectToAction("Mastg", new {id = model.TargetId});
        }
        catch (Exception e)
        {
            TempData["errorSaveMastg"] = "Error loading wstg!";
            _logger.LogError(e, "An error ocurred loading Checlist Workspace WSTG form. Project: {0} User: {1}", project,
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Mastg", new {id = model.TargetId});
        }
        
    }

    public IActionResult GenerateReportWSTG(IFormCollection form)
    {
        try
        {
            var pro = projectManager.GetById(Guid.Parse(form["project"]));

            var target = targetManager.GetById(Guid.Parse(form["target"]));

            var org = organizationManager.GetAll().First();

            var wstg = wstgManager.GetTargetId(Guid.Parse(form["project"]), Guid.Parse(form["target"]));

            var templatePath = _appEnvironment.WebRootPath + "/Attachments/Templates/templateWSTG.dotx";
            string resultPath = _appEnvironment.WebRootPath + "/Attachments/Templates/" + Guid.NewGuid().ToString() +
                                ".docx";


            using (WordprocessingDocument document = WordprocessingDocument.CreateFromTemplate(templatePath, true))
            {
                MainDocumentPart mainPart = document.MainDocumentPart;
                HtmlConverter converter = new HtmlConverter(mainPart);

                var body = document.MainDocumentPart.Document.Body;
                var paragraphs = body.Elements<Paragraph>();

                var tables = mainPart.Document.Descendants<Table>().ToList();

                var w = from c in tables
                    where c.InnerText.Contains("ClientName") ||
                          c.InnerText.Contains("OrganizationVersion") || c.InnerText.Contains("DocumentDescription")
                    select c;

                Table orgTable = w.First();
                
                var row = orgTable.Descendants<Text>();

                foreach (Text textDoc in row)
                {
                    switch (textDoc.Text)
                    {
                        case "ClientName":
                            textDoc.Text = pro.Client.Name;
                            break;
                        case "ClientContactEmail":
                            textDoc.Text = pro.Client.ContactEmail;
                            break;
                        case "ClientContactPhone":
                            textDoc.Text = pro.Client.ContactPhone;
                            break;
                        case "OrganizationName":
                            textDoc.Text = org.Name;
                            break;
                        case "OrganizationContactEmail":
                            textDoc.Text = org.ContactEmail;
                            break;
                        case "OrganizationContactPhone":
                            textDoc.Text = org.ContactPhone;
                            break;
                    }
                }

                var q = from c in tables
                    where c.InnerText.Contains("TargetUrl") ||
                          c.InnerText.Contains("DateTime") || c.InnerText.Contains("TargetDescription")
                    select c;

                Table targetTable = q.First();
                var rowTarget = targetTable.Descendants<Text>();
                foreach (Text textDoc in rowTarget)
                {
                    switch (textDoc.Text)
                    {
                        case "TargetUrl":
                            textDoc.Text = target.Name;
                            break;
                        case "DateTime":
                            textDoc.Text = DateTime.Now.ToUniversalTime().ToShortDateString();
                            break;
                    }
                }

                var r = from c in tables
                    where c.InnerText.Contains("Info01Note") ||
                          c.InnerText.Contains("Info01Status") || c.InnerText.Contains("Conf01Note") ||
                          c.InnerText.Contains("Conf01Status")
                          || c.InnerText.Contains("Idnt1Status") || c.InnerText.Contains("Idnt1Note")
                    select c;

                Table resultTable = r.First();
                var rowResult = resultTable.Descendants<Text>();
                foreach (Text textDoc in rowResult)
                {
                    switch (textDoc.Text)
                    {
                        case "Info01Note":
                            textDoc.Text = wstg.Info01Note;
                            break;
                        case "Info01Status":
                            textDoc.Text = wstg.Info01Status.ToString();
                            break;
                        case "Info02Note":
                            textDoc.Text = wstg.Info02Note;
                            break;
                        case "Info02Status":
                            textDoc.Text = wstg.Info02Status.ToString();
                            break;
                        case "Info03Note":
                            textDoc.Text = wstg.Info03Note;
                            break;
                        case "Info03Status":
                            textDoc.Text = wstg.Info03Status.ToString();
                            break;
                        case "Info04Note":
                            textDoc.Text = wstg.Info04Note;
                            break;
                        case "Info04Status":
                            textDoc.Text = wstg.Info04Status.ToString();
                            break;
                        case "Info05Note":
                            textDoc.Text = wstg.Info05Note;
                            break;
                        case "Info05Status":
                            textDoc.Text = wstg.Info05Status.ToString();
                            break;
                        case "Info06Note":
                            textDoc.Text = wstg.Info06Note;
                            break;
                        case "Info06Status":
                            textDoc.Text = wstg.Info06Status.ToString();
                            break;
                        case "Info07Note":
                            textDoc.Text = wstg.Info07Note;
                            break;
                        case "Info07Status":
                            textDoc.Text = wstg.Info07Status.ToString();
                            break;
                        case "Info08Note":
                            textDoc.Text = wstg.Info08Note;
                            break;
                        case "Info08Status":
                            textDoc.Text = wstg.Info08Status.ToString();
                            break;
                        case "Info09Note":
                            textDoc.Text = wstg.Info09Note;
                            break;
                        case "Info09Status":
                            textDoc.Text = wstg.Info09Status.ToString();
                            break;
                        case "Info10Note":
                            textDoc.Text = wstg.Info10Note;
                            break;
                        case "Info10Status":
                            textDoc.Text = wstg.Info10Status.ToString();
                            break;
                        case "Conf01Note":
                            textDoc.Text = wstg.Conf01Note;
                            break;
                        case "Conf01Status":
                            textDoc.Text = wstg.Conf01Status.ToString();
                            break;
                        case "Conf02Note":
                            textDoc.Text = wstg.Conf02Note;
                            break;
                        case "Conf02Status":
                            textDoc.Text = wstg.Conf02Status.ToString();
                            break;
                        case "Conf03Note":
                            textDoc.Text = wstg.Conf03Note;
                            break;
                        case "Conf03Status":
                            textDoc.Text = wstg.Conf03Status.ToString();
                            break;
                        case "Conf04Note":
                            textDoc.Text = wstg.Conf04Note;
                            break;
                        case "Conf04Status":
                            textDoc.Text = wstg.Conf04Status.ToString();
                            break;
                        case "Conf05Note":
                            textDoc.Text = wstg.Conf05Note;
                            break;
                        case "Conf05Status":
                            textDoc.Text = wstg.Conf05Status.ToString();
                            break;
                        case "Conf06Note":
                            textDoc.Text = wstg.Conf06Note;
                            break;
                        case "Conf06Status":
                            textDoc.Text = wstg.Conf06Status.ToString();
                            break;
                        case "Conf07Note":
                            textDoc.Text = wstg.Conf07Note;
                            break;
                        case "Conf07Status":
                            textDoc.Text = wstg.Conf07Status.ToString();
                            break;
                        case "Conf08Note":
                            textDoc.Text = wstg.Conf08Note;
                            break;
                        case "Conf08Status":
                            textDoc.Text = wstg.Conf08Status.ToString();
                            break;
                        case "Conf09Note":
                            textDoc.Text = wstg.Conf09Note;
                            break;
                        case "Conf09Status":
                            textDoc.Text = wstg.Conf09Status.ToString();
                            break;
                        case "Conf10Note":
                            textDoc.Text = wstg.Conf10Note;
                            break;
                        case "Conf10Status":
                            textDoc.Text = wstg.Conf10Status.ToString();
                            break;
                        case "Conf11Note":
                            textDoc.Text = wstg.Conf11Note;
                            break;
                        case "Conf11Status":
                            textDoc.Text = wstg.Conf11Status.ToString();
                            break;
                        case "Idnt1Note":
                            textDoc.Text = wstg.Idnt1Note;
                            break;
                        case "Idnt1Status":
                            textDoc.Text = wstg.Idnt1Status.ToString();
                            break;
                        case "Idnt2Note":
                            textDoc.Text = wstg.Idnt2Note;
                            break;
                        case "Idnt2Status":
                            textDoc.Text = wstg.Idnt2Status.ToString();
                            break;
                        case "Idnt3Note":
                            textDoc.Text = wstg.Idnt3Note;
                            break;
                        case "Idnt3Status":
                            textDoc.Text = wstg.Idnt3Status.ToString();
                            break;
                        case "Idnt4Note":
                            textDoc.Text = wstg.Idnt4Note;
                            break;
                        case "Idnt4Status":
                            textDoc.Text = wstg.Idnt4Status.ToString();
                            break;
                        case "Idnt5Note":
                            textDoc.Text = wstg.Idnt5Note;
                            break;
                        case "Idnt5Status":
                            textDoc.Text = wstg.Idnt5Status.ToString();
                            break;
                        case "Athn01Note":
                            textDoc.Text = wstg.Athn01Note;
                            break;
                        case "Athn01Status":
                            textDoc.Text = wstg.Athn01Status.ToString();
                            break;
                        case "Athn02Note":
                            textDoc.Text = wstg.Athn02Note;
                            break;
                        case "Athn02Status":
                            textDoc.Text = wstg.Athn02Status.ToString();
                            break;
                        case "Athn03Note":
                            textDoc.Text = wstg.Athn03Note;
                            break;
                        case "Athn03Status":
                            textDoc.Text = wstg.Athn03Status.ToString();
                            break;
                        case "Athn04Note":
                            textDoc.Text = wstg.Athn04Note;
                            break;
                        case "Athn04Status":
                            textDoc.Text = wstg.Athn04Status.ToString();
                            break;
                        case "Athn05Note":
                            textDoc.Text = wstg.Athn05Note;
                            break;
                        case "Athn05Status":
                            textDoc.Text = wstg.Athn05Status.ToString();
                            break;
                        case "Athn06Note":
                            textDoc.Text = wstg.Athn06Note;
                            break;
                        case "Athn06Status":
                            textDoc.Text = wstg.Athn06Status.ToString();
                            break;
                        case "Athn07Note":
                            textDoc.Text = wstg.Athn07Note;
                            break;
                        case "Athn07Status":
                            textDoc.Text = wstg.Athn07Status.ToString();
                            break;
                        case "Athn08Note":
                            textDoc.Text = wstg.Athn08Note;
                            break;
                        case "Athn08Status":
                            textDoc.Text = wstg.Athn08Status.ToString();
                            break;
                        case "Athn09Note":
                            textDoc.Text = wstg.Athn09Note;
                            break;
                        case "Athn09Status":
                            textDoc.Text = wstg.Athn09Status.ToString();
                            break;
                        case "Athn10Note":
                            textDoc.Text = wstg.Athn10Note;
                            break;
                        case "Athn10Status":
                            textDoc.Text = wstg.Athn10Status.ToString();
                            break;
                        case "Athz01Note":
                            textDoc.Text = wstg.Athz01Note;
                            break;
                        case "Athz01Status":
                            textDoc.Text = wstg.Athz01Status.ToString();
                            break;
                        case "Athz02Note":
                            textDoc.Text = wstg.Athz02Note;
                            break;
                        case "Athz02Status":
                            textDoc.Text = wstg.Athz02Status.ToString();
                            break;
                        case "Athz03Note":
                            textDoc.Text = wstg.Athz03Note;
                            break;
                        case "Athz03Status":
                            textDoc.Text = wstg.Athz03Status.ToString();
                            break;
                        case "Athz04Note":
                            textDoc.Text = wstg.Athz04Note;
                            break;
                        case "Athz04Status":
                            textDoc.Text = wstg.Athz04Status.ToString();
                            break;
                        case "Sess01Note":
                            textDoc.Text = wstg.Sess01Note;
                            break;
                        case "Sess01Status":
                            textDoc.Text = wstg.Sess01Status.ToString();
                            break;
                        case "Sess02Note":
                            textDoc.Text = wstg.Sess02Note;
                            break;
                        case "Sess02Status":
                            textDoc.Text = wstg.Sess02Status.ToString();
                            break;
                        case "Sess03Note":
                            textDoc.Text = wstg.Sess03Note;
                            break;
                        case "Sess03Status":
                            textDoc.Text = wstg.Sess03Status.ToString();
                            break;
                        case "Sess04Note":
                            textDoc.Text = wstg.Sess04Note;
                            break;
                        case "Sess04Status":
                            textDoc.Text = wstg.Sess04Status.ToString();
                            break;
                        case "Sess05Note":
                            textDoc.Text = wstg.Sess05Note;
                            break;
                        case "Sess05Status":
                            textDoc.Text = wstg.Sess05Status.ToString();
                            break;
                        case "Sess06Note":
                            textDoc.Text = wstg.Sess06Note;
                            break;
                        case "Sess06Status":
                            textDoc.Text = wstg.Sess06Status.ToString();
                            break;
                        case "Sess07Note":
                            textDoc.Text = wstg.Sess07Note;
                            break;
                        case "Sess07Status":
                            textDoc.Text = wstg.Sess07Status.ToString();
                            break;
                        case "Sess08Note":
                            textDoc.Text = wstg.Sess08Note;
                            break;
                        case "Sess08Status":
                            textDoc.Text = wstg.Sess08Status.ToString();
                            break;
                        case "Sess09Note":
                            textDoc.Text = wstg.Sess09Note;
                            break;
                        case "Sess09Status":
                            textDoc.Text = wstg.Sess09Status.ToString();
                            break;
                        case "Inpv01Note":
                            textDoc.Text = wstg.Inpv01Note;
                            break;
                        case "Inpv01Status":
                            textDoc.Text = wstg.Inpv01Status.ToString();
                            break;
                        case "Inpv02Note":
                            textDoc.Text = wstg.Inpv02Note;
                            break;
                        case "Inpv02Status":
                            textDoc.Text = wstg.Inpv02Status.ToString();
                            break;
                        case "Inpv03Note":
                            textDoc.Text = wstg.Inpv03Note;
                            break;
                        case "Inpv03Status":
                            textDoc.Text = wstg.Inpv03Status.ToString();
                            break;
                        case "Inpv04Note":
                            textDoc.Text = wstg.Inpv04Note;
                            break;
                        case "Inpv04Status":
                            textDoc.Text = wstg.Inpv04Status.ToString();
                            break;
                        case "Inpv05Note":
                            textDoc.Text = wstg.Inpv05Note;
                            break;
                        case "Inpv05Status":
                            textDoc.Text = wstg.Inpv05Status.ToString();
                            break;
                        case "Inpv06Note":
                            textDoc.Text = wstg.Inpv06Note;
                            break;
                        case "Inpv06Status":
                            textDoc.Text = wstg.Inpv06Status.ToString();
                            break;
                        case "Inpv07Note":
                            textDoc.Text = wstg.Inpv07Note;
                            break;
                        case "Inpv07Status":
                            textDoc.Text = wstg.Inpv07Status.ToString();
                            break;
                        case "Inpv08Note":
                            textDoc.Text = wstg.Inpv08Note;
                            break;
                        case "Inpv08Status":
                            textDoc.Text = wstg.Inpv08Status.ToString();
                            break;
                        case "Inpv09Note":
                            textDoc.Text = wstg.Inpv09Note;
                            break;
                        case "Inpv09Status":
                            textDoc.Text = wstg.Inpv09Status.ToString();
                            break;
                        case "Inpv10Note":
                            textDoc.Text = wstg.Inpv10Note;
                            break;
                        case "Inpv10Status":
                            textDoc.Text = wstg.Inpv10Status.ToString();
                            break;
                        case "Inpv11Note":
                            textDoc.Text = wstg.Inpv11Note;
                            break;
                        case "Inpv11Status":
                            textDoc.Text = wstg.Inpv11Status.ToString();
                            break;
                        case "Inpv12Note":
                            textDoc.Text = wstg.Inpv12Note;
                            break;
                        case "Inpv12Status":
                            textDoc.Text = wstg.Inpv12Status.ToString();
                            break;
                        case "Inpv13Note":
                            textDoc.Text = wstg.Inpv13Note;
                            break;
                        case "Inpv13Status":
                            textDoc.Text = wstg.Inpv13Status.ToString();
                            break;
                        case "Inpv14Note":
                            textDoc.Text = wstg.Inpv14Note;
                            break;
                        case "Inpv14Status":
                            textDoc.Text = wstg.Inpv14Status.ToString();
                            break;
                        case "Inpv15Note":
                            textDoc.Text = wstg.Inpv15Note;
                            break;
                        case "Inpv15Status":
                            textDoc.Text = wstg.Inpv15Status.ToString();
                            break;
                        case "Inpv16Note":
                            textDoc.Text = wstg.Inpv16Note;
                            break;
                        case "Inpv16Status":
                            textDoc.Text = wstg.Inpv16Status.ToString();
                            break;
                        case "Inpv17Note":
                            textDoc.Text = wstg.Inpv17Note;
                            break;
                        case "Inpv17Status":
                            textDoc.Text = wstg.Inpv17Status.ToString();
                            break;
                        case "Inpv18Note":
                            textDoc.Text = wstg.Inpv18Note;
                            break;
                        case "Inpv18Status":
                            textDoc.Text = wstg.Inpv18Status.ToString();
                            break;
                        case "Inpv19Note":
                            textDoc.Text = wstg.Inpv19Note;
                            break;
                        case "Inpv19Status":
                            textDoc.Text = wstg.Inpv19Status.ToString();
                            break;
                        case "Errh01Note":
                            textDoc.Text = wstg.Errh01Note;
                            break;
                        case "Errh01Status":
                            textDoc.Text = wstg.Errh01Status.ToString();
                            break;
                        case "Cryp01Note":
                            textDoc.Text = wstg.Cryp01Note;
                            break;
                        case "Cryp01Status":
                            textDoc.Text = wstg.Cryp01Status.ToString();
                            break;
                        case "Cryp02Note":
                            textDoc.Text = wstg.Cryp02Note;
                            break;
                        case "Cryp02Status":
                            textDoc.Text = wstg.Cryp02Status.ToString();
                            break;
                        case "Cryp03Note":
                            textDoc.Text = wstg.Cryp03Note;
                            break;
                        case "Cryp03Status":
                            textDoc.Text = wstg.Cryp03Status.ToString();
                            break;
                        case "Cryp04Note":
                            textDoc.Text = wstg.Cryp04Note;
                            break;
                        case "Cryp04Status":
                            textDoc.Text = wstg.Cryp04Status.ToString();
                            break;
                        case "Busl01Note":
                            textDoc.Text = wstg.Busl01Note;
                            break;
                        case "Busl01Status":
                            textDoc.Text = wstg.Busl01Status.ToString();
                            break;
                        case "Busl02Note":
                            textDoc.Text = wstg.Busl02Note;
                            break;
                        case "Busl02Status":
                            textDoc.Text = wstg.Busl02Status.ToString();
                            break;
                        case "Busl03Note":
                            textDoc.Text = wstg.Busl03Note;
                            break;
                        case "Busl03Status":
                            textDoc.Text = wstg.Busl03Status.ToString();
                            break;
                        case "Busl04Note":
                            textDoc.Text = wstg.Busl04Note;
                            break;
                        case "Busl04Status":
                            textDoc.Text = wstg.Busl04Status.ToString();
                            break;
                        case "Busl05Note":
                            textDoc.Text = wstg.Busl05Note;
                            break;
                        case "Busl05Status":
                            textDoc.Text = wstg.Busl05Status.ToString();
                            break;
                        case "Busl06Note":
                            textDoc.Text = wstg.Busl06Note;
                            break;
                        case "Busl06Status":
                            textDoc.Text = wstg.Busl06Status.ToString();
                            break;
                        case "Busl07Note":
                            textDoc.Text = wstg.Busl07Note;
                            break;
                        case "Busl07Status":
                            textDoc.Text = wstg.Busl07Status.ToString();
                            break;
                        case "Busl08Note":
                            textDoc.Text = wstg.Busl08Note;
                            break;
                        case "Busl08Status":
                            textDoc.Text = wstg.Busl08Status.ToString();
                            break;
                        case "Busl09Note":
                            textDoc.Text = wstg.Busl09Note;
                            break;
                        case "Busl09Status":
                            textDoc.Text = wstg.Busl09Status.ToString();
                            break;
                        case "Clnt01Note":
                            textDoc.Text = wstg.Clnt01Note;
                            break;
                        case "Clnt01Status":
                            textDoc.Text = wstg.Clnt01Status.ToString();
                            break;
                        case "Clnt02Note":
                            textDoc.Text = wstg.Clnt02Note;
                            break;
                        case "Clnt02Status":
                            textDoc.Text = wstg.Clnt02Status.ToString();
                            break;
                        case "Clnt03Note":
                            textDoc.Text = wstg.Clnt03Note;
                            break;
                        case "Clnt03Status":
                            textDoc.Text = wstg.Clnt03Status.ToString();
                            break;
                        case "Clnt04Note":
                            textDoc.Text = wstg.Clnt04Note;
                            break;
                        case "Clnt04Status":
                            textDoc.Text = wstg.Clnt04Status.ToString();
                            break;
                        case "Clnt05Note":
                            textDoc.Text = wstg.Clnt05Note;
                            break;
                        case "Clnt05Status":
                            textDoc.Text = wstg.Clnt05Status.ToString();
                            break;
                        case "Clnt06Note":
                            textDoc.Text = wstg.Clnt06Note;
                            break;
                        case "Clnt06Status":
                            textDoc.Text = wstg.Clnt06Status.ToString();
                            break;
                        case "Clnt07Note":
                            textDoc.Text = wstg.Clnt07Note;
                            break;
                        case "Clnt07Status":
                            textDoc.Text = wstg.Clnt07Status.ToString();
                            break;
                        case "Clnt08Note":
                            textDoc.Text = wstg.Clnt08Note;
                            break;
                        case "Clnt08Status":
                            textDoc.Text = wstg.Clnt08Status.ToString();
                            break;
                        case "Clnt09Note":
                            textDoc.Text = wstg.Clnt09Note;
                            break;
                        case "Clnt09Status":
                            textDoc.Text = wstg.Clnt09Status.ToString();
                            break;
                        case "Clnt10Note":
                            textDoc.Text = wstg.Clnt10Note;
                            break;
                        case "Clnt10Status":
                            textDoc.Text = wstg.Clnt10Status.ToString();
                            break;
                        case "Clnt11Note":
                            textDoc.Text = wstg.Clnt11Note;
                            break;
                        case "Clnt11Status":
                            textDoc.Text = wstg.Clnt11Status.ToString();
                            break;
                        case "Clnt12Note":
                            textDoc.Text = wstg.Clnt12Note;
                            break;
                        case "Clnt12Status":
                            textDoc.Text = wstg.Clnt12Status.ToString();
                            break;
                        case "Clnt13Note":
                            textDoc.Text = wstg.Clnt13Note;
                            break;
                        case "Clnt13Status":
                            textDoc.Text = wstg.Clnt13Status.ToString();
                            break;
                        case "Apit01Note":
                            textDoc.Text = wstg.Clnt10Note;
                            break;
                        case "Apit01Status":
                            textDoc.Text = wstg.Clnt10Status.ToString();
                            break;
                    }
                }

                var result = document.SaveAs(resultPath);
                result.Close();

            }

            var fileBytes = System.IO.File.ReadAllBytes(resultPath);
            System.IO.File.Delete(resultPath);

            return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.template",
                "OWASPWSTG_Report_" + target.Name + ".docx");
        }
        catch (Exception e)
        {
            TempData["errorReportWSTG"] = "Error generating report!";
            _logger.LogError(e, "An error ocurred generating Checlist Workspace WSTG report. User: {1}", 
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction(nameof(Index));
        }
    }
    public IActionResult GenerateReportMASTG(IFormCollection form)
    {
        try
        {
            var pro = projectManager.GetById(Guid.Parse(form["project"]));

            var target = targetManager.GetById(Guid.Parse(form["target"]));

            var org = organizationManager.GetAll().First();

            var mastg = mastgManager.GetTargetId(Guid.Parse(form["project"]), Guid.Parse(form["target"]));

            var templatePath = _appEnvironment.WebRootPath + "/Attachments/Templates/templateMASTG.dotx";
            string resultPath = _appEnvironment.WebRootPath + "/Attachments/Templates/" + Guid.NewGuid().ToString() +
                                ".docx";


            using (WordprocessingDocument document = WordprocessingDocument.CreateFromTemplate(templatePath, true))
            {
                MainDocumentPart mainPart = document.MainDocumentPart;
                HtmlConverter converter = new HtmlConverter(mainPart);

                var body = document.MainDocumentPart.Document.Body;
                var paragraphs = body.Elements<Paragraph>();

                var tables = mainPart.Document.Descendants<Table>().ToList();

                var w = from c in tables
                    where c.InnerText.Contains("ClientName") ||
                          c.InnerText.Contains("OrganizationVersion") || c.InnerText.Contains("DocumentDescription")
                    select c;

                Table orgTable = w.First();
                
                var row = orgTable.Descendants<Text>();

                foreach (Text textDoc in row)
                {
                    switch (textDoc.Text)
                    {
                        case "ClientName":
                            textDoc.Text = pro.Client.Name;
                            break;
                        case "ClientContactEmail":
                            textDoc.Text = pro.Client.ContactEmail;
                            break;
                        case "ClientContactPhone":
                            textDoc.Text = pro.Client.ContactPhone;
                            break;
                        case "OrganizationName":
                            textDoc.Text = org.Name;
                            break;
                        case "OrganizationContactEmail":
                            textDoc.Text = org.ContactEmail;
                            break;
                        case "OrganizationContactPhone":
                            textDoc.Text = org.ContactPhone;
                            break;
                    }
                }

                var q = from c in tables
                    where c.InnerText.Contains("TargetUrl") ||
                          c.InnerText.Contains("DateTime") || c.InnerText.Contains("TargetDescription")
                    select c;

                Table targetTable = q.First();
                var rowTarget = targetTable.Descendants<Text>();
                foreach (Text textDoc in rowTarget)
                {
                    switch (textDoc.Text)
                    {
                        case "TargetUrl":
                            textDoc.Text = target.Name;
                            break;
                        case "DateTime":
                            textDoc.Text = DateTime.Now.ToUniversalTime().ToShortDateString();
                            break;
                    }
                }
                
                var r = from c in tables
                    where c.InnerText.Contains("ArchNote01") ||
                          c.InnerText.Contains("ArchStatus01") || c.InnerText.Contains("CodeNote01")
                          || c.InnerText.Contains("CodeStatus01") || c.InnerText.Contains("ResilienceNote01")
                    select c;
                
                Table resultsTable = r.First();
                var rowResults = resultsTable.Descendants<Text>();
                foreach (Text textDoc in rowResults)
                {
                    switch (textDoc.Text)
                    {
                        case "ArchNote01":
                            textDoc.Text = mastg.ArchNote01;
                            break;
                        case "ArchStatus01":
                            textDoc.Text = mastg.ArchStatus01.ToString();
                            break;
                        case "ArchNote02":
                            textDoc.Text = mastg.ArchNote02;
                            break;
                        case "ArchStatus02":
                            textDoc.Text = mastg.ArchStatus02.ToString();
                            break;
                        case "ArchNote03":
                            textDoc.Text = mastg.ArchNote03;
                            break;
                        case "ArchStatus03":
                            textDoc.Text = mastg.ArchStatus03.ToString();
                            break;
                        case "ArchNote04":
                            textDoc.Text = mastg.ArchNote04;
                            break;
                        case "ArchStatus04":
                            textDoc.Text = mastg.ArchStatus04.ToString();
                            break;
                        case "ArchNote05":
                            textDoc.Text = mastg.ArchNote05;
                            break;
                        case "ArchStatus05":
                            textDoc.Text = mastg.ArchStatus05.ToString();
                            break;
                        case "ArchNote06":
                            textDoc.Text = mastg.ArchNote06;
                            break;
                        case "ArchStatus06":
                            textDoc.Text = mastg.ArchStatus06.ToString();
                            break;
                        case "ArchNote07":
                            textDoc.Text = mastg.ArchNote07;
                            break;
                        case "ArchStatus07":
                            textDoc.Text = mastg.ArchStatus07.ToString();
                            break;
                        case "ArchNote08":
                            textDoc.Text = mastg.ArchNote08;
                            break;
                        case "ArchStatus08":
                            textDoc.Text = mastg.ArchStatus08.ToString();
                            break;
                        case "ArchNote09":
                            textDoc.Text = mastg.ArchNote09;
                            break;
                        case "ArchStatus09":
                            textDoc.Text = mastg.ArchStatus09.ToString();
                            break;
                        case "ArchNote10":
                            textDoc.Text = mastg.ArchNote10;
                            break;
                        case "ArchStatus10":
                            textDoc.Text = mastg.ArchStatus10.ToString();
                            break;
                        case "ArchNote11":
                            textDoc.Text = mastg.ArchNote11;
                            break;
                        case "ArchStatus11":
                            textDoc.Text = mastg.ArchStatus11.ToString();
                            break;
                        case "ArchNote12":
                            textDoc.Text = mastg.ArchNote12;
                            break;
                        case "ArchStatus12":
                            textDoc.Text = mastg.ArchStatus12.ToString();
                            break;
                        case "StorageNote01":
                            textDoc.Text = mastg.StorageNote01;
                            break;
                        case "StorageStatus01":
                            textDoc.Text = mastg.StorageStatus01.ToString();
                            break;
                        case "StorageNote02":
                            textDoc.Text = mastg.StorageNote02;
                            break;
                        case "StorageStatus02":
                            textDoc.Text = mastg.StorageStatus02.ToString();
                            break;
                        case "StorageNote03":
                            textDoc.Text = mastg.StorageNote03;
                            break;
                        case "StorageStatus03":
                            textDoc.Text = mastg.StorageStatus03.ToString();
                            break;
                        case "StorageNote04":
                            textDoc.Text = mastg.StorageNote04;
                            break;
                        case "StorageStatus04":
                            textDoc.Text = mastg.StorageStatus04.ToString();
                            break;
                        case "StorageNote05":
                            textDoc.Text = mastg.StorageNote05;
                            break;
                        case "StorageStatus05":
                            textDoc.Text = mastg.StorageStatus05.ToString();
                            break;
                        case "StorageNote06":
                            textDoc.Text = mastg.StorageNote06;
                            break;
                        case "StorageStatus06":
                            textDoc.Text = mastg.StorageStatus06.ToString();
                            break;
                        case "StorageNote07":
                            textDoc.Text = mastg.StorageNote07;
                            break;
                        case "StorageStatus07":
                            textDoc.Text = mastg.StorageStatus07.ToString();
                            break;
                        case "StorageNote08":
                            textDoc.Text = mastg.StorageNote08;
                            break;
                        case "StorageStatus08":
                            textDoc.Text = mastg.StorageStatus08.ToString();
                            break;
                        case "StorageNote09":
                            textDoc.Text = mastg.StorageNote09;
                            break;
                        case "StorageStatus09":
                            textDoc.Text = mastg.StorageStatus09.ToString();
                            break;
                        case "StorageNote10":
                            textDoc.Text = mastg.StorageNote10;
                            break;
                        case "StorageStatus10":
                            textDoc.Text = mastg.StorageStatus10.ToString();
                            break;
                        case "StorageNote11":
                            textDoc.Text = mastg.StorageNote11;
                            break;
                        case "StorageStatus11":
                            textDoc.Text = mastg.StorageStatus11.ToString();
                            break;
                        case "StorageNote12":
                            textDoc.Text = mastg.StorageNote12;
                            break;
                        case "StorageStatus12":
                            textDoc.Text = mastg.StorageStatus12.ToString();
                            break;
                        case "StorageNote13":
                            textDoc.Text = mastg.StorageNote13;
                            break;
                        case "StorageStatus13":
                            textDoc.Text = mastg.StorageStatus13.ToString();
                            break;
                        case "StorageNote14":
                            textDoc.Text = mastg.StorageNote14;
                            break;
                        case "StorageStatus14":
                            textDoc.Text = mastg.StorageStatus14.ToString();
                            break;
                        case "StorageNote15":
                            textDoc.Text = mastg.StorageNote15;
                            break;
                        case "StorageStatus15":
                            textDoc.Text = mastg.StorageStatus15.ToString();
                            break;
                        case "CryptoNote01":
                            textDoc.Text = mastg.CryptoNote01;
                            break;
                        case "CryptoStatus01":
                            textDoc.Text = mastg.CryptoStatus01.ToString();
                            break;
                        case "CryptoNote02":
                            textDoc.Text = mastg.CryptoNote02;
                            break;
                        case "CryptoStatus02":
                            textDoc.Text = mastg.CryptoStatus02.ToString();
                            break;
                        case "CryptoNote03":
                            textDoc.Text = mastg.CryptoNote03;
                            break;
                        case "CryptoStatus03":
                            textDoc.Text = mastg.CryptoStatus03.ToString();
                            break;
                        case "CryptoNote04":
                            textDoc.Text = mastg.CryptoNote04;
                            break;
                        case "CryptoStatus04":
                            textDoc.Text = mastg.CryptoStatus04.ToString();
                            break;
                        case "CryptoNote05":
                            textDoc.Text = mastg.CryptoNote05;
                            break;
                        case "CryptoStatus05":
                            textDoc.Text = mastg.CryptoStatus05.ToString();
                            break;
                        case "CryptoNote06":
                            textDoc.Text = mastg.CryptoNote06;
                            break;
                        case "CryptoStatus06":
                            textDoc.Text = mastg.CryptoStatus06.ToString();
                            break;
                        case "AuthNote01":
                            textDoc.Text = mastg.AuthNote01;
                            break;
                        case "AuthStatus01":
                            textDoc.Text = mastg.AuthStatus01.ToString();
                            break;
                        case "AuthNote02":
                            textDoc.Text = mastg.AuthNote02;
                            break;
                        case "AuthStatus02":
                            textDoc.Text = mastg.AuthStatus02.ToString();
                            break;
                        case "AuthNote03":
                            textDoc.Text = mastg.AuthNote03;
                            break;
                        case "AuthStatus03":
                            textDoc.Text = mastg.AuthStatus03.ToString();
                            break;
                        case "AuthNote04":
                            textDoc.Text = mastg.AuthNote04;
                            break;
                        case "AuthStatus04":
                            textDoc.Text = mastg.AuthStatus04.ToString();
                            break;
                        case "AuthNote05":
                            textDoc.Text = mastg.AuthNote05;
                            break;
                        case "AuthStatus05":
                            textDoc.Text = mastg.AuthStatus05.ToString();
                            break;
                        case "AuthNote06":
                            textDoc.Text = mastg.AuthNote06;
                            break;
                        case "AuthStatus06":
                            textDoc.Text = mastg.AuthStatus06.ToString();
                            break;
                        case "AuthNote07":
                            textDoc.Text = mastg.AuthNote07;
                            break;
                        case "AuthStatus07":
                            textDoc.Text = mastg.AuthStatus07.ToString();
                            break;
                        case "AuthNote08":
                            textDoc.Text = mastg.AuthNote08;
                            break;
                        case "AuthStatus08":
                            textDoc.Text = mastg.AuthStatus08.ToString();
                            break;
                        case "AuthNote09":
                            textDoc.Text = mastg.AuthNote09;
                            break;
                        case "AuthStatus09":
                            textDoc.Text = mastg.AuthStatus09.ToString();
                            break;
                        case "AuthNote10":
                            textDoc.Text = mastg.AuthNote10;
                            break;
                        case "AuthStatus10":
                            textDoc.Text = mastg.AuthStatus10.ToString();
                            break;
                        case "AuthNote11":
                            textDoc.Text = mastg.AuthNote11;
                            break;
                        case "AuthStatus11":
                            textDoc.Text = mastg.AuthStatus11.ToString();
                            break;
                        case "AuthNote12":
                            textDoc.Text = mastg.AuthNote12;
                            break;
                        case "AuthStatus12":
                            textDoc.Text = mastg.AuthStatus12.ToString();
                            break;
                        case "NetworkNote01":
                            textDoc.Text = mastg.NetworkNote01;
                            break;
                        case "NetworkStatus01":
                            textDoc.Text = mastg.NetworkStatus01.ToString();
                            break;
                        case "NetworkNote02":
                            textDoc.Text = mastg.NetworkNote02;
                            break;
                        case "NetworkStatus02":
                            textDoc.Text = mastg.NetworkStatus02.ToString();
                            break;
                        case "NetworkNote03":
                            textDoc.Text = mastg.NetworkNote03;
                            break;
                        case "NetworkStatus03":
                            textDoc.Text = mastg.NetworkStatus03.ToString();
                            break;
                        case "NetworkNote04":
                            textDoc.Text = mastg.NetworkNote04;
                            break;
                        case "NetworkStatus04":
                            textDoc.Text = mastg.NetworkStatus04.ToString();
                            break;
                        case "NetworkNote05":
                            textDoc.Text = mastg.NetworkNote05;
                            break;
                        case "NetworkStatus05":
                            textDoc.Text = mastg.NetworkStatus05.ToString();
                            break;
                        case "NetworkNote06":
                            textDoc.Text = mastg.NetworkNote06;
                            break;
                        case "NetworkStatus06":
                            textDoc.Text = mastg.NetworkStatus06.ToString();
                            break;
                        case "PlatformNote01":
                            textDoc.Text = mastg.PlatformNote01;
                            break;
                        case "PlatformStatus01":
                            textDoc.Text = mastg.PlatformStatus01.ToString();
                            break;
                        case "PlatformNote02":
                            textDoc.Text = mastg.PlatformNote02;
                            break;
                        case "PlatformStatus02":
                            textDoc.Text = mastg.PlatformStatus02.ToString();
                            break;
                        case "PlatformNote03":
                            textDoc.Text = mastg.PlatformNote03;
                            break;
                        case "PlatformStatus03":
                            textDoc.Text = mastg.PlatformStatus03.ToString();
                            break;
                        case "PlatformNote04":
                            textDoc.Text = mastg.PlatformNote04;
                            break;
                        case "PlatformStatus04":
                            textDoc.Text = mastg.PlatformStatus04.ToString();
                            break;
                        case "PlatformNote05":
                            textDoc.Text = mastg.PlatformNote05;
                            break;
                        case "PlatformStatus05":
                            textDoc.Text = mastg.PlatformStatus05.ToString();
                            break;
                        case "PlatformNote06":
                            textDoc.Text = mastg.PlatformNote06;
                            break;
                        case "PlatformStatus06":
                            textDoc.Text = mastg.PlatformStatus06.ToString();
                            break;
                        case "PlatformNote07":
                            textDoc.Text = mastg.PlatformNote07;
                            break;
                        case "PlatformStatus07":
                            textDoc.Text = mastg.PlatformStatus07.ToString();
                            break;
                        case "PlatformNote08":
                            textDoc.Text = mastg.PlatformNote08;
                            break;
                        case "PlatformStatus08":
                            textDoc.Text = mastg.PlatformStatus08.ToString();
                            break;
                        case "PlatformNote09":
                            textDoc.Text = mastg.PlatformNote09;
                            break;
                        case "PlatformStatus09":
                            textDoc.Text = mastg.PlatformStatus09.ToString();
                            break;
                        case "PlatformNote10":
                            textDoc.Text = mastg.PlatformNote10;
                            break;
                        case "PlatformStatus10":
                            textDoc.Text = mastg.PlatformStatus10.ToString();
                            break;
                        case "PlatformNote11":
                            textDoc.Text = mastg.PlatformNote11;
                            break;
                        case "PlatformStatus11":
                            textDoc.Text = mastg.PlatformStatus11.ToString();
                            break;
                        case "CodeNote01":
                            textDoc.Text = mastg.CodeNote01;
                            break;
                        case "CodeStatus01":
                            textDoc.Text = mastg.CodeStatus01.ToString();
                            break;
                        case "CodeNote02":
                            textDoc.Text = mastg.CodeNote02;
                            break;
                        case "CodeStatus02":
                            textDoc.Text = mastg.CodeStatus02.ToString();
                            break;
                        case "CodeNote03":
                            textDoc.Text = mastg.CodeNote03;
                            break;
                        case "CodeStatus03":
                            textDoc.Text = mastg.CodeStatus03.ToString();
                            break;
                        case "CodeNote04":
                            textDoc.Text = mastg.CodeNote04;
                            break;
                        case "CodeStatus04":
                            textDoc.Text = mastg.CodeStatus04.ToString();
                            break;
                        case "CodeNote05":
                            textDoc.Text = mastg.CodeNote05;
                            break;
                        case "CodeStatus05":
                            textDoc.Text = mastg.CodeStatus05.ToString();
                            break;
                        case "CodeNote06":
                            textDoc.Text = mastg.CodeNote06;
                            break;
                        case "CodeStatus06":
                            textDoc.Text = mastg.CodeStatus06.ToString();
                            break;
                        case "CodeNote07":
                            textDoc.Text = mastg.CodeNote07;
                            break;
                        case "CodeStatus07":
                            textDoc.Text = mastg.CodeStatus07.ToString();
                            break;
                        case "CodeNote08":
                            textDoc.Text = mastg.CodeNote08;
                            break;
                        case "CodeStatus08":
                            textDoc.Text = mastg.CodeStatus08.ToString();
                            break;
                        case "CodeNote09":
                            textDoc.Text = mastg.CodeNote09;
                            break;
                        case "CodeStatus09":
                            textDoc.Text = mastg.CodeStatus09.ToString();
                            break;
                        case "ResilienceNote01":
                            textDoc.Text = mastg.ResilienceNote01;
                            break;
                        case "ResilienceStatus01":
                            textDoc.Text = mastg.ResilienceStatus01.ToString();
                            break;
                        case "ResilienceNote02":
                            textDoc.Text = mastg.ResilienceNote02;
                            break;
                        case "ResilienceStatus02":
                            textDoc.Text = mastg.ResilienceStatus02.ToString();
                            break;
                        case "ResilienceNote03":
                            textDoc.Text = mastg.ResilienceNote03;
                            break;
                        case "ResilienceStatus03":
                            textDoc.Text = mastg.ResilienceStatus03.ToString();
                            break;
                        case "ResilienceNote04":
                            textDoc.Text = mastg.ResilienceNote04;
                            break;
                        case "ResilienceStatus04":
                            textDoc.Text = mastg.ResilienceStatus04.ToString();
                            break;
                        case "ResilienceNote05":
                            textDoc.Text = mastg.ResilienceNote05;
                            break;
                        case "ResilienceStatus05":
                            textDoc.Text = mastg.ResilienceStatus05.ToString();
                            break;
                        case "ResilienceNote06":
                            textDoc.Text = mastg.ResilienceNote06;
                            break;
                        case "ResilienceStatus06":
                            textDoc.Text = mastg.ResilienceStatus06.ToString();
                            break;
                        case "ResilienceNote07":
                            textDoc.Text = mastg.ResilienceNote07;
                            break;
                        case "ResilienceStatus07":
                            textDoc.Text = mastg.ResilienceStatus07.ToString();
                            break;
                        case "ResilienceNote08":
                            textDoc.Text = mastg.ResilienceNote08;
                            break;
                        case "ResilienceStatus08":
                            textDoc.Text = mastg.ResilienceStatus08.ToString();
                            break;
                        case "ResilienceNote09":
                            textDoc.Text = mastg.ResilienceNote09;
                            break;
                        case "ResilienceStatus09":
                            textDoc.Text = mastg.ResilienceStatus09.ToString();
                            break;
                        case "ResilienceNote10":
                            textDoc.Text = mastg.ResilienceNote10;
                            break;
                        case "ResilienceStatus10":
                            textDoc.Text = mastg.ResilienceStatus10.ToString();
                            break;
                        case "ResilienceNote11":
                            textDoc.Text = mastg.ResilienceNote11;
                            break;
                        case "ResilienceStatus11":
                            textDoc.Text = mastg.ResilienceStatus11.ToString();
                            break;
                        case "ResilienceNote12":
                            textDoc.Text = mastg.ResilienceNote12;
                            break;
                        case "ResilienceStatus12":
                            textDoc.Text = mastg.ResilienceStatus12.ToString();
                            break;
                        case "ResilienceNote13":
                            textDoc.Text = mastg.ResilienceNote13;
                            break;
                        case "ResilienceStatus13":
                            textDoc.Text = mastg.ResilienceStatus13.ToString();
                            break;
                    }
                }
                
                
                var result = document.SaveAs(resultPath);
                result.Close();
                
            }
            var fileBytes = System.IO.File.ReadAllBytes(resultPath);
            System.IO.File.Delete(resultPath);

            return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.template",
                "OWASPMASTG_Report_" + target.Name + ".docx");
        }
        catch (Exception e)
        {
            TempData["errorReportMASTG"] = "Error generating report!";
            _logger.LogError(e, "An error ocurred generating Checlist Workspace WSTG report. User: {1}",
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction(nameof(Index));
        }
    }
}