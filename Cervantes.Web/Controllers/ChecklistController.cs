using System.Security.Claims;
using System.Text;
using System.Web;
using Cervantes.Contracts;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.CORE.ViewModel.Mastg;
using Cervantes.CORE.ViewModel.Wstg;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Ganss.Xss;
using HandlebarsDotNet;
using HtmlToOpenXml;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MudBlazor;

namespace Cervantes.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize (Roles = "Admin,SuperUser,User")]
public class ChecklistController : ControllerBase
{
    private readonly ILogger<ChecklistController> _logger = null;
    private IProjectManager projectManager = null;
    private IClientManager clientManager = null;
    private IProjectUserManager projectUserManager = null;
    private ITargetManager targetManager = null;
    private IWSTGManager wstgManager = null;
    private IMASTGManager mastgManager = null;
    private IOrganizationManager organizationManager = null;
    private readonly IWebHostEnvironment env;
    private IHttpContextAccessor HttpContextAccessor;
    private string aspNetUserId;
    private IReportComponentsManager reportComponentsManager;
    private IReportsPartsManager reportsPartsManager;
    private IReportManager reportManager = null;
    private IReportTemplateManager reportTemplateManager = null;
    
    public ChecklistController(IProjectManager projectManager, 
        IProjectUserManager projectUserManager, ITargetManager targetManager, ILogger<ChecklistController> logger, IWSTGManager wstgManager,
        IMASTGManager mastgManager,IWebHostEnvironment env, IOrganizationManager organizationManager, IClientManager clientManager,IHttpContextAccessor HttpContextAccessor,
        IReportComponentsManager reportComponentsManager, IReportsPartsManager reportsPartsManager, IReportManager reportManager, IReportTemplateManager reportTemplateManager)
    {
        this.projectManager = projectManager;
        this.projectUserManager = projectUserManager;
        this.targetManager = targetManager;
        _logger = logger;
        this.wstgManager = wstgManager;
        this.mastgManager = mastgManager;
        this.env = env;
        this.organizationManager = organizationManager;
        this.clientManager = clientManager;
        aspNetUserId = HttpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        this.reportComponentsManager = reportComponentsManager;
        this.reportsPartsManager = reportsPartsManager;
        this.reportManager = reportManager;
        this.reportTemplateManager = reportTemplateManager;
    }
    
    [HttpGet]
    [Route("WSTG/Project/{projectId}")]
    public IEnumerable<CORE.Entities.WSTG> GetWSTG(Guid projectId)
    {
        try
        {
            var user = projectUserManager.VerifyUser(projectId, aspNetUserId);
            if (user == null)
            {
                return null;
            }
            IEnumerable<CORE.Entities.WSTG> model = wstgManager.GetAll().Where(x => x.ProjectId == projectId).Include(x => x.User).Include(x => x.Target).ToArray();
          
            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred getting the WSTG information. User: {0}",
                aspNetUserId);
            throw;
        }
        
    }
    
    [HttpGet]
    [Route("WSTG/{checklistId}")]
    public CORE.Entities.WSTG GetWSTGById(Guid checklistId)
    {
        try
        {
            
            CORE.Entities.WSTG model = wstgManager.GetById(checklistId);
            var user = projectUserManager.VerifyUser(model.ProjectId, aspNetUserId);
            if (user == null)
            {
                return null;
            }
            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred getting the WSTG information. User: {0}",
                aspNetUserId);
            throw;
        }
        
    }
    
    [HttpGet]
    [Route("MASTG/Project/{projectId}")]
    public IEnumerable<CORE.Entities.MASTG> GetMSTG(Guid projectId)
    {
        try
        {
            
            IEnumerable<CORE.Entities.MASTG> model = mastgManager.GetAll().Where(x => x.ProjectId == projectId).Include(x => x.User).Include(x => x.Target).ToArray();
            if (model != null)
            {
                var user = projectUserManager.VerifyUser(projectId, aspNetUserId);
                if (user == null)
                {
                    return null;
                }
            }
            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred getting the MASTG information. User: {0}",
                aspNetUserId);
            throw;
        }
        
    }
    [HttpGet]
    [Route("MASTG/{checklistId}")]
    public CORE.Entities.MASTG GetMASTGById(Guid checklistId)
    {
        try
        {
            
            CORE.Entities.MASTG model = mastgManager.GetById(checklistId);
            if (model != null)
            {
                var user = projectUserManager.VerifyUser(model.ProjectId, aspNetUserId);
                if (user == null)
                {
                    return null;
                }
                return model;
            }

            return null;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred getting the MASTG information. User: {0}",
                aspNetUserId);
            throw;
        }
        
    }
    
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] ChecklistCreateViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {

                var user = projectUserManager.VerifyUser(model.ProjectId, aspNetUserId);
                if (user == null)
                {
                    return BadRequest();
                }
                
                switch (model.Type)
                {
                    case ChecklistType.OWASPWSTG:
                        WSTG wstg = new WSTG
                        {
                            TargetId = model.TargetId,
                            UserId = aspNetUserId,
                            ProjectId = model.ProjectId,
                            CreatedDate = DateTime.Now.ToUniversalTime()

                        };
                        await wstgManager.AddAsync(wstg);
                        await wstgManager.Context.SaveChangesAsync();
                        _logger.LogInformation("Checklist added successfully. User: {0}",
                            aspNetUserId);
                        return Ok();
                    case ChecklistType.OWASPMASVS:
                        MASTG mstg = new MASTG
                        {
                            TargetId = model.TargetId,
                            UserId = aspNetUserId,
                            ProjectId = model.ProjectId,
                            CreatedDate = DateTime.Now.ToUniversalTime(),
                            MobilePlatform = model.MobileAppPlatform

                        };
                        await mastgManager.AddAsync(mstg);
                        await mastgManager.Context.SaveChangesAsync();
                        _logger.LogInformation("Checklist added successfully. User: {0}",
                            aspNetUserId);
                        return Ok();
                }
            
                
            }
            _logger.LogError("An error occurred adding a Checklist. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred adding a Checklist. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
       
    }

    [HttpPut]
    [Route("WSTG")]
    public async Task<IActionResult> EditWstg([FromBody] WSTGViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                
                var sanitizer = new HtmlSanitizer();
                sanitizer.AllowedSchemes.Add("data");
                
               var result = wstgManager.GetById(model.Id);
              
               if (result.Id != Guid.Empty)
               {
                   var user = projectUserManager.VerifyUser(result.ProjectId, aspNetUserId);
                   if (user == null)
                   {
                       return BadRequest();
                   }
                   
                   result.Apit01Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Apit.Apit01Note));
                   result.Apit01Status = model.Apit.Apit01Status;

                   result.Athz01Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Athz.Athz01Note));
                   result.Athz01Status = model.Athz.Athz01Status;
                   result.Athz02Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Athz.Athz02Note));
                   result.Athz02Status = model.Athz.Athz02Status;
                   result.Athz03Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Athz.Athz03Note));
                   result.Athz03Status = model.Athz.Athz03Status;
                   result.Athz04Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Athz.Athz04Note));
                   result.Athz04Status = model.Athz.Athz04Status;

                   result.Athn01Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Auth.Athn01Note));
                   result.Athn01Status = model.Auth.Athn01Status;
                   result.Athn02Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Auth.Athn02Note));
                   result.Athn02Status = model.Auth.Athn02Status;
                   result.Athn03Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Auth.Athn03Note));
                   result.Athn03Status = model.Auth.Athn03Status;
                   result.Athn04Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Auth.Athn04Note));
                   result.Athn04Status = model.Auth.Athn04Status;
                   result.Athn05Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Auth.Athn05Note));
                   result.Athn05Status = model.Auth.Athn05Status;
                   result.Athn06Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Auth.Athn06Note));
                   result.Athn06Status = model.Auth.Athn06Status;
                   result.Athn07Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Auth.Athn07Note));
                   result.Athn07Status = model.Auth.Athn07Status;
                   result.Athn08Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Auth.Athn08Note));
                   result.Athn08Status = model.Auth.Athn08Status;
                   result.Athn09Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Auth.Athn09Note));
                   result.Athn09Status = model.Auth.Athn09Status;
                   result.Athn10Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Auth.Athn10Note));
                   result.Athn10Status = model.Auth.Athn10Status;

                   result.Busl01Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Busl.Busl01Note));
                   result.Busl01Status = model.Busl.Busl01Status;
                   result.Busl02Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Busl.Busl02Note));
                   result.Busl02Status = model.Busl.Busl02Status;
                   result.Busl03Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Busl.Busl03Note));
                   result.Busl03Status = model.Busl.Busl03Status;
                   result.Busl04Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Busl.Busl04Note));
                   result.Busl04Status = model.Busl.Busl04Status;
                   result.Busl05Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Busl.Busl05Note));
                   result.Busl05Status = model.Busl.Busl05Status;
                   result.Busl06Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Busl.Busl06Note));
                   result.Busl06Status = model.Busl.Busl06Status;
                   result.Busl07Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Busl.Busl07Note));
                   result.Busl07Status = model.Busl.Busl07Status;
                   result.Busl08Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Busl.Busl08Note));
                   result.Busl08Status = model.Busl.Busl08Status;
                   result.Busl09Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Busl.Busl09Note));
                   result.Busl09Status = model.Busl.Busl09Status;

                   result.Clnt01Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Clnt.Clnt01Note));
                   result.Clnt01Status = model.Clnt.Clnt01Status;
                   result.Clnt02Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Clnt.Clnt02Note));
                   result.Clnt02Status = model.Clnt.Clnt02Status;
                   result.Clnt03Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Clnt.Clnt03Note));
                   result.Clnt03Status = model.Clnt.Clnt03Status;
                   result.Clnt04Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Clnt.Clnt04Note));
                   result.Clnt04Status = model.Clnt.Clnt04Status;
                   result.Clnt05Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Clnt.Clnt05Note));
                   result.Clnt05Status = model.Clnt.Clnt05Status;
                   result.Clnt06Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Clnt.Clnt06Note));
                   result.Clnt06Status = model.Clnt.Clnt06Status;
                   result.Clnt07Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Clnt.Clnt07Note));
                   result.Clnt07Status = model.Clnt.Clnt07Status;
                   result.Clnt08Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Clnt.Clnt08Note));
                   result.Clnt08Status = model.Clnt.Clnt08Status;
                   result.Clnt09Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Clnt.Clnt09Note));
                   result.Clnt09Status = model.Clnt.Clnt09Status;
                   result.Clnt10Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Clnt.Clnt10Note));
                   result.Clnt10Status = model.Clnt.Clnt10Status;
                   result.Clnt11Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Clnt.Clnt11Note));
                   result.Clnt11Status = model.Clnt.Clnt11Status;
                   result.Clnt12Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Clnt.Clnt12Note));
                   result.Clnt12Status = model.Clnt.Clnt12Status;
                   result.Clnt13Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Clnt.Clnt13Note));
                   result.Clnt13Status = model.Clnt.Clnt13Status;

                   result.Conf01Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Conf.Conf01Note));
                   result.Conf01Status = model.Conf.Conf01Status;
                   result.Conf02Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Conf.Conf02Note));
                   result.Conf02Status = model.Conf.Conf02Status;
                   result.Conf03Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Conf.Conf03Note));
                   result.Conf03Status = model.Conf.Conf03Status;
                   result.Conf04Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Conf.Conf04Note));
                   result.Conf04Status = model.Conf.Conf04Status;
                   result.Conf05Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Conf.Conf05Note));
                   result.Conf05Status = model.Conf.Conf05Status;
                   result.Conf06Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Conf.Conf06Note));
                   result.Conf06Status = model.Conf.Conf06Status;
                   result.Conf07Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Conf.Conf07Note));
                   result.Conf07Status = model.Conf.Conf07Status;
                   result.Conf08Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Conf.Conf08Note));
                   result.Conf08Status = model.Conf.Conf08Status;
                   result.Conf09Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Conf.Conf09Note));
                   result.Conf09Status = model.Conf.Conf09Status;
                   result.Conf10Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Conf.Conf10Note));
                   result.Conf10Status = model.Conf.Conf10Status;
                   result.Conf11Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Conf.Conf11Note));
                   result.Conf11Status = model.Conf.Conf11Status;

                   result.Cryp01Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Cryp.Cryp01Note));
                   result.Cryp01Status = model.Cryp.Cryp01Status;
                   result.Cryp02Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Cryp.Cryp02Note));
                   result.Cryp02Status = model.Cryp.Cryp02Status;
                   result.Cryp03Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Cryp.Cryp03Note));
                   result.Cryp03Status = model.Cryp.Cryp03Status;
                   result.Cryp04Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Cryp.Cryp04Note));
                   result.Cryp04Status = model.Cryp.Cryp04Status;

                   result.Errh01Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Errh.Errh01Note));
                   result.Errh01Status = model.Errh.Errh01Status;
                   result.Errh02Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Errh.Errh02Note));
                   result.Errh02Status = model.Errh.Errh02Status;

                   result.Idnt1Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Idnt.Idnt1Note));
                   result.Idnt1Status = model.Idnt.Idnt1Status;
                   result.Idnt2Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Idnt.Idnt2Note));
                   result.Idnt2Status = model.Idnt.Idnt2Status;
                   result.Idnt3Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Idnt.Idnt3Note));
                   result.Idnt3Status = model.Idnt.Idnt3Status;
                   result.Idnt4Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Idnt.Idnt4Note));
                   result.Idnt4Status = model.Idnt.Idnt4Status;
                   result.Idnt5Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Idnt.Idnt5Note));
                   result.Idnt5Status = model.Idnt.Idnt5Status;

                   result.Info01Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Info.Info01Note));
                   result.Info01Status = model.Info.Info01Status;
                   result.Info02Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Info.Info02Note));
                   result.Info02Status = model.Info.Info02Status;
                   result.Info03Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Info.Info03Note));
                   result.Info03Status = model.Info.Info03Status;
                   result.Info04Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Info.Info04Note));
                   result.Info04Status = model.Info.Info04Status;
                   result.Info05Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Info.Info05Note));
                   result.Info05Status = model.Info.Info05Status;
                   result.Info06Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Info.Info06Note));
                   result.Info06Status = model.Info.Info06Status;
                   result.Info07Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Info.Info07Note));
                   result.Info07Status = model.Info.Info07Status;
                   result.Info08Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Info.Info08Note));
                   result.Info08Status = model.Info.Info08Status;
                   result.Info09Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Info.Info09Note));
                   result.Info09Status = model.Info.Info09Status;
                   result.Info10Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Info.Info10Note));
                   result.Info10Status = model.Info.Info10Status;

                   result.Inpv01Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Inpv.Inpv01Note));
                   result.Inpv01Status = model.Inpv.Inpv01Status;
                   result.Inpv02Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Inpv.Inpv02Note));
                   result.Inpv02Status = model.Inpv.Inpv02Status;
                   result.Inpv03Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Inpv.Inpv03Note));
                   result.Inpv03Status = model.Inpv.Inpv03Status;
                   result.Inpv04Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Inpv.Inpv04Note));
                   result.Inpv04Status = model.Inpv.Inpv04Status;
                   result.Inpv05Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Inpv.Inpv05Note));
                   result.Inpv05Status = model.Inpv.Inpv05Status;
                   result.Inpv06Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Inpv.Inpv06Note));
                   result.Inpv06Status = model.Inpv.Inpv06Status;
                   result.Inpv07Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Inpv.Inpv07Note));
                   result.Inpv07Status = model.Inpv.Inpv07Status;
                   result.Inpv08Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Inpv.Inpv08Note));
                   result.Inpv08Status = model.Inpv.Inpv08Status;
                   result.Inpv09Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Inpv.Inpv09Note));
                   result.Inpv09Status = model.Inpv.Inpv09Status;
                   result.Inpv10Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Inpv.Inpv10Note));
                   result.Inpv10Status = model.Inpv.Inpv10Status;
                   result.Inpv11Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Inpv.Inpv11Note));
                   result.Inpv11Status = model.Inpv.Inpv11Status;
                   result.Inpv12Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Inpv.Inpv12Note));
                   result.Inpv12Status = model.Inpv.Inpv12Status;
                   result.Inpv13Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Inpv.Inpv13Note));
                   result.Inpv13Status = model.Inpv.Inpv13Status;
                   result.Inpv14Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Inpv.Inpv14Note));
                   result.Inpv14Status = model.Inpv.Inpv14Status;
                   result.Inpv15Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Inpv.Inpv15Note));
                   result.Inpv15Status = model.Inpv.Inpv15Status;
                   result.Inpv16Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Inpv.Inpv16Note));
                   result.Inpv16Status = model.Inpv.Inpv16Status;
                   result.Inpv17Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Inpv.Inpv17Note));
                   result.Inpv17Status = model.Inpv.Inpv17Status;
                   result.Inpv18Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Inpv.Inpv18Note));
                   result.Inpv18Status = model.Inpv.Inpv18Status;
                   result.Inpv19Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Inpv.Inpv19Note));
                   result.Inpv19Status = model.Inpv.Inpv19Status;

                   result.Sess01Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Sess.Sess01Note));
                   result.Sess01Status = model.Sess.Sess01Status;
                   result.Sess02Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Sess.Sess02Note));
                   result.Sess02Status = model.Sess.Sess02Status;
                   result.Sess03Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Sess.Sess03Note));
                   result.Sess03Status = model.Sess.Sess03Status;
                   result.Sess04Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Sess.Sess04Note));
                   result.Sess04Status = model.Sess.Sess04Status;
                   result.Sess05Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Sess.Sess05Note));
                   result.Sess05Status = model.Sess.Sess05Status;
                   result.Sess06Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Sess.Sess06Note));
                   result.Sess06Status = model.Sess.Sess06Status;
                   result.Sess07Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Sess.Sess07Note));
                   result.Sess07Status = model.Sess.Sess07Status;
                   result.Sess08Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Sess.Sess08Note));
                   result.Sess08Status = model.Sess.Sess08Status;
                   result.Sess09Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Sess.Sess09Note));
                   result.Sess09Status = model.Sess.Sess09Status;

                   await wstgManager.Context.SaveChangesAsync();
                   _logger.LogInformation("User: {0} edited Checklist: {1}", aspNetUserId, result.Id);
                   return Ok();
               }

               _logger.LogError("An error occurred editing a Checklist. User: {0}",
                   aspNetUserId);
               return BadRequest();
            }

            _logger.LogError("An error occurred editing a Checklist. User: {0}",
                aspNetUserId);;
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e,"An error occurred editing a Checklist. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
    }
    
    
    [HttpDelete]
    [Route("WSTG/{checklistId}")]
    public async Task<IActionResult> DeleteWstg(Guid checklistId)
    {
        try
        {
            var result = wstgManager.GetById(checklistId);
            if (result.Id != Guid.Empty)
            {
                var user = projectUserManager.VerifyUser(result.ProjectId, aspNetUserId);
                if (user == null)
                {
                    return BadRequest();
                }
                
                wstgManager.Remove(result);
                await wstgManager.Context.SaveChangesAsync();
                _logger.LogInformation("WSTG Checklist deleted successfully. User: {0}",
                    aspNetUserId);
                return Ok();
            }
            _logger.LogError("An error occurred deleting a WSTG Checklist. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error deleting adding a WSTG Checklist. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
       
    }
    
    [HttpDelete]
    [Route("MASTG/{checklistId}")]
    public async Task<IActionResult> DeleteMastg(Guid checklistId)
    {
        try
        {
            var result = mastgManager.GetById(checklistId);
            if (result.Id != Guid.Empty)
            {
                var user = projectUserManager.VerifyUser(result.ProjectId, aspNetUserId);
                if (user == null)
                {
                    return BadRequest();
                }
                
                mastgManager.Remove(result);
                await mastgManager.Context.SaveChangesAsync();
                _logger.LogInformation("MASTG Checklist deleted successfully. User: {0}",
                    aspNetUserId);
                return Ok();
            }
            _logger.LogError("An error occurred deleting a MASTG Checklist. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error deleting adding a MASTG Checklist. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
       
    }
    
    
    [HttpPut]
    [Route("MASTG/")]
    public async Task<IActionResult> EditMastg([FromBody] MastgViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {

               var result = mastgManager.GetById(model.Id);
               if (result.Id != Guid.Empty)
               {
                   var user = projectUserManager.VerifyUser(result.ProjectId, aspNetUserId);
                   if (user == null)
                   {
                       return BadRequest();
                   }
                   
                   var sanitizer = new HtmlSanitizer();
                   sanitizer.AllowedSchemes.Add("data");
                   
                   result.Storage1Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Storage.Storage1Note));
                     result.Storage1Status = model.Storage.Storage1Status;
                     result.Storage1Note1 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Storage.Storage1Note1));
                        result.Storage1Status1 = model.Storage.Storage1Status1;
                        result.Storage1Note2 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Storage.Storage1Note2));
                        result.Storage1Status2 = model.Storage.Storage1Status2;
                        result.Storage1Note3 =sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Storage.Storage1Note3));
                        result.Storage1Status3 = model.Storage.Storage1Status3;
                        result.Storage2Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Storage.Storage2Note));
                        result.Storage2Status = model.Storage.Storage2Status;
                        result.Storage2Note1 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Storage.Storage2Note1));
                        result.Storage2Status1 = model.Storage.Storage2Status1;
                        result.Storage2Note2 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Storage.Storage2Note2));
                        result.Storage2Status2 = model.Storage.Storage2Status2;
                        result.Storage2Note3 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Storage.Storage2Note3));
                        result.Storage2Status3 = model.Storage.Storage2Status3;
                        result.Storage2Note4 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Storage.Storage2Note4));
                        result.Storage2Status4 = model.Storage.Storage2Status4;
                        result.Storage2Note5 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Storage.Storage2Note5));
                        result.Storage2Status5 = model.Storage.Storage2Status5;
                        result.Storage2Note6 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Storage.Storage2Note6));
                        result.Storage2Status6 = model.Storage.Storage2Status6;
                        result.Storage2Note7 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Storage.Storage2Note7));
                        result.Storage2Status7 = model.Storage.Storage2Status7;
                        result.Storage2Note8 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Storage.Storage2Note8));
                        result.Storage2Status8 = model.Storage.Storage2Status8;
                        result.Storage2Note9 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Storage.Storage2Note9));
                        result.Storage2Status9 = model.Storage.Storage2Status9;
                        result.Storage2Note10 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Storage.Storage2Note10));
                        result.Storage2Status10 = model.Storage.Storage2Status10;
                        result.Storage2Note11 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Storage.Storage2Note11));
                        result.Storage2Status11 = model.Storage.Storage2Status11;
                        
                        result.Crypto1Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Crypto.Crypto1Note));
                        result.Crypto1Status = model.Crypto.Crypto1Status;
                        result.Crypto1Note1 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Crypto.Crypto1Note1));
                        result.Crypto1Status1 = model.Crypto.Crypto1Status1;
                        result.Crypto1Note2 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Crypto.Crypto1Note2));
                        result.Crypto1Status2 = model.Crypto.Crypto1Status2;
                        result.Crypto1Note3 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Crypto.Crypto1Note3));
                        result.Crypto1Status3 = model.Crypto.Crypto1Status3;
                        result.Crypto1Note4 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Crypto.Crypto1Note4));
                        result.Crypto1Status4 = model.Crypto.Crypto1Status4;
                        result.Crypto1Note5 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Crypto.Crypto1Note5));
                        result.Crypto1Status5 = model.Crypto.Crypto1Status5;
                        result.Crypto2Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Crypto.Crypto2Note));
                        result.Crypto2Status = model.Crypto.Crypto2Status;
                        result.Crypto2Note1 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Crypto.Crypto2Note1));
                        result.Crypto2Status1 = model.Crypto.Crypto2Status1;
                        result.Crypto2Note2 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Crypto.Crypto2Note2));
                        result.Crypto2Status2 = model.Crypto.Crypto2Status2;
                        
                        result.Auth1Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Auth.Auth1Note));
                        result.Auth1Status = model.Auth.Auth1Status;
                        result.Auth2Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Auth.Auth2Note));
                        result.Auth2Status = model.Auth.Auth2Status;
                        result.Auth2Note1 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Auth.Auth2Note1));
                        result.Auth2Status1 = model.Auth.Auth2Status1;
                        result.Auth2Note2 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Auth.Auth2Note2));
                        result.Auth2Status2 = model.Auth.Auth2Status2;
                        result.Auth2Note3 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Auth.Auth2Note3));
                        result.Auth2Status3 = model.Auth.Auth2Status3;
                        result.Auth3Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Auth.Auth3Note));
                        result.Auth3Status = model.Auth.Auth3Status;
                        
                        result.Network1Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Network.Network1Note));
                        result.Network1Status = model.Network.Network1Status;
                        result.Network1Note1 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Network.Network1Note1));
                        result.Network1Status1 = model.Network.Network1Status1;
                        result.Network1Note2 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Network.Network1Note2));
                        result.Network1Status2 = model.Network.Network1Status2;
                        result.Network1Note3 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Network.Network1Note3));
                        result.Network1Status3 = model.Network.Network1Status3;
                        result.Network1Note4 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Network.Network1Note4));
                        result.Network1Status4 = model.Network.Network1Status4;
                        result.Network1Note5 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Network.Network1Note5));
                        result.Network1Status5 = model.Network.Network1Status5;
                        result.Network1Note6 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Network.Network1Note6));
                        result.Network1Status6 = model.Network.Network1Status6;
                        result.Network1Note7 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Network.Network1Note7));
                        result.Network1Status7 = model.Network.Network1Status7;
                        result.Network2Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Network.Network2Note));
                        result.Network2Status = model.Network.Network2Status;
                        result.Network2Note1 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Network.Network2Note1));
                        result.Network2Status1 = model.Network.Network2Status1;
                        result.Network2Note2 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Network.Network2Note2));
                        result.Network2Status2 = model.Network.Network2Status2;
                        
                        result.Platform1Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Platform.Platform1Note));
                        result.Platform1Status = model.Platform.Platform1Status;
                        result.Platform1Note1 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Platform.Platform1Note1));
                        result.Platform1Status1 = model.Platform.Platform1Status1;
                        result.Platform1Note2 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Platform.Platform1Note2));
                        result.Platform1Status2 = model.Platform.Platform1Status2;
                        result.Platform1Note3 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Platform.Platform1Note3));
                        result.Platform1Status3 = model.Platform.Platform1Status3;
                        result.Platform1Note4 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Platform.Platform1Note4));
                        result.Platform1Status4 = model.Platform.Platform1Status4;
                        result.Platform1Note5 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Platform.Platform1Note5));
                        result.Platform1Status5 = model.Platform.Platform1Status5;
                        result.Platform1Note6 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Platform.Platform1Note6));
                        result.Platform1Status6 = model.Platform.Platform1Status6;
                        result.Platform1Note7 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Platform.Platform1Note7));
                        result.Platform1Status7 = model.Platform.Platform1Status7;
                        result.Platform1Note8 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Platform.Platform1Note8));
                        result.Platform1Status8 = model.Platform.Platform1Status8;
                        result.Platform1Note9 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Platform.Platform1Note9));
                        result.Platform1Status9 = model.Platform.Platform1Status9;
                        result.Platform1Note10 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Platform.Platform1Note10));
                        result.Platform1Status10 = model.Platform.Platform1Status10;
                        result.Platform1Note11 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Platform.Platform1Note11));
                        result.Platform1Status11 = model.Platform.Platform1Status11;
                        result.Platform1Note12 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Platform.Platform1Note12));
                        result.Platform1Status12 = model.Platform.Platform1Status12;
                        result.Platform1Note13 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Platform.Platform1Note13));
                        result.Platform1Status13 = model.Platform.Platform1Status13;
                        result.Platform2Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Platform.Platform2Note));
                        result.Platform2Status = model.Platform.Platform2Status;
                        result.Platform2Note1 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Platform.Platform2Note1));
                        result.Platform2Status1 = model.Platform.Platform2Status1;
                        result.Platform2Note2 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Platform.Platform2Note2));
                        result.Platform2Status2 = model.Platform.Platform2Status2;
                        result.Platform2Note3 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Platform.Platform2Note3));
                        result.Platform2Status3 = model.Platform.Platform2Status3;
                        result.Platform2Note4 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Platform.Platform2Note4));
                        result.Platform2Status4 = model.Platform.Platform2Status4;
                        result.Platform2Note5 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Platform.Platform2Note5));
                        result.Platform2Status5 = model.Platform.Platform2Status5;
                        result.Platform2Note6 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Platform.Platform2Note6));
                        result.Platform2Status6 = model.Platform.Platform2Status6;
                        result.Platform2Note7 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Platform.Platform2Note7));
                        result.Platform2Status7 = model.Platform.Platform2Status7;
                        result.Platform3Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Platform.Platform3Note));
                        result.Platform3Status = model.Platform.Platform3Status;
                        result.Platform3Note1 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Platform.Platform3Note1));
                        result.Platform3Status1 = model.Platform.Platform3Status1;
                        result.Platform3Note2 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Platform.Platform3Note2));
                        result.Platform3Status2 = model.Platform.Platform3Status2;
                        result.Platform3Note3 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Platform.Platform3Note3));
                        result.Platform3Status3 = model.Platform.Platform3Status3;
                        result.Platform3Note4 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Platform.Platform3Note4));
                        result.Platform3Status4 = model.Platform.Platform3Status4;
                        result.Platform3Note5 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Platform.Platform3Note5));
                        result.Platform3Status5 = model.Platform.Platform3Status5;

                        result.Code1Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Code.Code1Note));
                        result.Code1Status = model.Code.Code1Status;
                        result.Code2Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Code.Code2Note));
                        result.Code2Status = model.Code.Code2Status;
                        result.Code2Note1 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Code.Code2Note1));
                        result.Code2Status1 = model.Code.Code2Status1;
                        result.Code2Note2 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Code.Code2Note2));
                        result.Code2Status2 = model.Code.Code2Status2;
                        result.Code3Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Code.Code3Note));
                        result.Code3Status = model.Code.Code3Status;
                        result.Code3Note1 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Code.Code3Note1));
                        result.Code3Status1 = model.Code.Code3Status1;
                        result.Code3Note2 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Code.Code3Note2));
                        result.Code3Status2 = model.Code.Code3Status2;
                        result.Code4Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Code.Code4Note));
                        result.Code4Status = model.Code.Code4Status;
                        result.Code4Note1 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Code.Code4Note1));
                        result.Code4Status1 = model.Code.Code4Status1;
                        result.Code4Note2 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Code.Code4Note2));
                        result.Code4Status2 = model.Code.Code4Status2;
                        result.Code4Note3 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Code.Code4Note3));
                        result.Code4Status3 = model.Code.Code4Status3;
                        result.Code4Note4 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Code.Code4Note4));
                        result.Code4Status4 = model.Code.Code4Status4;
                        result.Code4Note5 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Code.Code4Note5));
                        result.Code4Status5 = model.Code.Code4Status5;
                        result.Code4Note6 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Code.Code4Note6));
                        result.Code4Status6 = model.Code.Code4Status6;
                        result.Code4Note7 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Code.Code4Note7));
                        result.Code4Status7 = model.Code.Code4Status7;
                        result.Code4Note8 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Code.Code4Note8));
                        result.Code4Status8 = model.Code.Code4Status8;
                        result.Code4Note9 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Code.Code4Note9));
                        result.Code4Status9 = model.Code.Code4Status9;
                        result.Code4Note10 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Code.Code4Note10));
                        result.Code4Status10 = model.Code.Code4Status10;

                        result.Resilience1Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Resilience.Resilience1Note));
                        result.Resilience1Status = model.Resilience.Resilience1Status;
                        result.Resilience1Note1 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Resilience.Resilience1Note1));
                        result.Resilience1Status1 = model.Resilience.Resilience1Status1;
                        result.Resilience1Note2 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Resilience.Resilience1Note2));
                        result.Resilience1Status2 = model.Resilience.Resilience1Status2;
                        result.Resilience1Note3 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Resilience.Resilience1Note3));
                        result.Resilience1Status3 = model.Resilience.Resilience1Status3;
                        result.Resilience1Note4 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Resilience.Resilience1Note4));
                        result.Resilience1Status4 = model.Resilience.Resilience1Status4;
                        result.Resilience2Status = model.Resilience.Resilience2Status;
                        result.Resilience2Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Resilience.Resilience2Note));
                        result.Resilience2Status1 = model.Resilience.Resilience2Status1;
                        result.Resilience2Note1 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Resilience.Resilience2Note1));
                        result.Resilience2Status2 = model.Resilience.Resilience2Status2;
                        result.Resilience2Note2 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Resilience.Resilience2Note2));
                        result.Resilience2Status3 = model.Resilience.Resilience2Status3;
                        result.Resilience2Note3 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Resilience.Resilience2Note3));
                        result.Resilience2Status4 = model.Resilience.Resilience2Status4;
                        result.Resilience2Note4 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Resilience.Resilience2Note4));
                        result.Resilience2Status5 = model.Resilience.Resilience2Status5;
                        result.Resilience2Note5 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Resilience.Resilience2Note5));
                        result.Resilience3Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Resilience.Resilience3Note));
                        result.Resilience3Status = model.Resilience.Resilience3Status;
                        result.Resilience3Note1 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Resilience.Resilience3Note1));
                        result.Resilience3Status1 = model.Resilience.Resilience3Status1;
                        result.Resilience3Note2 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Resilience.Resilience3Note2));
                        result.Resilience3Status2 = model.Resilience.Resilience3Status2;
                        result.Resilience3Note3 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Resilience.Resilience3Note3));
                        result.Resilience3Status3 = model.Resilience.Resilience3Status3;
                        result.Resilience3Note4 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Resilience.Resilience3Note4));
                        result.Resilience3Status4 = model.Resilience.Resilience3Status4;
                        result.Resilience3Note5 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Resilience.Resilience3Note5));
                        result.Resilience3Status5 = model.Resilience.Resilience3Status5;
                        result.Resilience3Note6 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Resilience.Resilience3Note6));
                        result.Resilience3Status6 = model.Resilience.Resilience3Status6;
                        result.Resilience4Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Resilience.Resilience4Note));
                        result.Resilience4Status = model.Resilience.Resilience4Status;
                        result.Resilience4Status1 = model.Resilience.Resilience4Status1;
                        result.Resilience4Note1 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Resilience.Resilience4Note1));
                        result.Resilience4Status2 = model.Resilience.Resilience4Status2;
                        result.Resilience4Note2 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Resilience.Resilience4Note2));
                        result.Resilience4Status3 = model.Resilience.Resilience4Status3;
                        result.Resilience4Note3 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Resilience.Resilience4Note3));
                        result.Resilience4Status4 = model.Resilience.Resilience4Status4;
                        result.Resilience4Note4 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Resilience.Resilience4Note4));
                        result.Resilience4Status5 = model.Resilience.Resilience4Status5;
                        result.Resilience4Note5 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Resilience.Resilience4Note5));
                        result.Resilience4Status6 = model.Resilience.Resilience4Status6;
                        result.Resilience4Note6 = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Resilience.Resilience4Note6));

                        await mastgManager.Context.SaveChangesAsync();
                   _logger.LogInformation("User: {0} edited MASTG Checklist: {1}", aspNetUserId, result.Id);
                   return Ok();
               }

               _logger.LogError("An error occurred editing a MASTG Checklist. User: {0}",
                   aspNetUserId);
               return BadRequest();
            }

            _logger.LogError("An error occurred editing a MASTG Checklist. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e,"An error occurred editing a MASTG Checklist. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
    }
    
    [HttpPost]
    [Route("MASTG/Report")]
    public async Task<IActionResult> GenerateMastgReport(ReportChecklistCreateModel model)
    {
       try
       {

           var mastg = mastgManager.GetById(model.ChecklistId);
           if (mastg.Id != Guid.Empty)
           {

               var user = projectUserManager.VerifyUser(mastg.ProjectId, aspNetUserId);
               if (user == null)
               {
                   return BadRequest();
               }
               var pro = projectManager.GetById(mastg.ProjectId);

               var target = targetManager.GetById(mastg.TargetId);

               var org = organizationManager.GetAll().First();

               var client = clientManager.GetById(pro.ClientId);

               var sanitizer = new HtmlSanitizer();
               sanitizer.AllowedSchemes.Add("data");
               Report rep = new Report
               {
                   Id = Guid.NewGuid(),
                   Name = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Name)),
                   ProjectId = model.ProjectId,
                   UserId = aspNetUserId,
                   CreatedDate = DateTime.Now.ToUniversalTime(),
                   Description = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Description)),
                   Version = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Version)),
                   Language = pro.Language,
                   ReportType = ReportType.MASTG
               };
              
               var reportParts = reportsPartsManager.GetAll().Where(x => x.TemplateId == model.ReportTemplateId)
                   .OrderBy(x => x.Order).Include(x => x.Component).ToList();
               
               string source = @"<!DOCTYPE html>
                <html>
                <head>
                    <meta charset=""utf-8"">
                    <title></title>
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
                </head>
                <body>
                    <header>
                        {{HeaderComponents}}
                    </header>
                    <cover>
                        {{CoverComponents}}
                    </cover>
                    
                    {{BodyComponents}}

                    <footer> 
                        {{FooterComponents}}
                    </footer>
                </body>
                </html>";

                StringBuilder sbHeader = new StringBuilder();
                foreach (var part in reportParts.Where(x => x.Component.ComponentType == ReportPartType.Header)
                             .OrderBy(x => x.Order))
                {
                    sbHeader.Append(part.Component.Content);
                }

                source = source.Replace("{{HeaderComponents}}", sbHeader.ToString());


                StringBuilder sbCover = new StringBuilder();
                foreach (var part in reportParts.Where(x => x.Component.ComponentType == ReportPartType.Cover)
                             .OrderBy(x => x.Order))
                {
                    sbCover.Append(part.Component.Content);
                }

                source = source.Replace("{{CoverComponents}}", sbCover.ToString());

                StringBuilder sbBody = new StringBuilder();
                foreach (var part in reportParts.Where(x => x.Component.ComponentType == ReportPartType.Body)
                             .OrderBy(x => x.Order))
                {
                    sbBody.Append(part.Component.Content);
                }

                source = source.Replace("{{BodyComponents}}", sbBody.ToString());


                StringBuilder sbFooter = new StringBuilder();
                foreach (var part in reportParts.Where(x => x.Component.ComponentType == ReportPartType.Footer)
                             .OrderBy(x => x.Order))
                {
                    sbFooter.Append(part.Component.Content);
                }

                source = source.Replace("{{FooterComponents}}", sbFooter.ToString());

                var handlebars = Handlebars.Create();
                //handlebars.Configuration.UseNewtonsoftJson();
                //handlebars.RegisterHelper("lookup", (output, context, arguments) => { output.WriteSafeString(arguments[0]); }); 
                handlebars.RegisterHelper("lookup", (output, context, arguments) =>
                   {
                       var targetObject = arguments[0] as IDictionary<string, object>;
                       var propertyName = arguments[1].ToString();
                   
                       if (targetObject != null && targetObject.ContainsKey(propertyName))
                       {
                           output.WriteSafeString(targetObject[propertyName].ToString());
                       }
                   });
                
                
                var templateHtml = handlebars.Compile(source);
               
                 var data = new
                {
                    OrganizationName = org.Name,
                    OrganizationEmail = org.ContactEmail,
                    OrganizationPhone = org.ContactPhone,
                    OrganizationDescription = org.Description,
                    OrganizationContactName = org.ContactName,
                    OrganizationUrl = org.Url,
                    ClientName = client.Name,
                    ClientDescription = client.Description,
                    ClientUrl = client.Url,
                    ClientContactName = client.ContactName,
                    ClientEmail = client.ContactEmail,
                    ClientPhone = client.ContactPhone,
                    Year = DateTime.Now.Year.ToString(),
                    CreatedDate = DateTime.Now.ToUniversalTime().ToShortDateString(),
                    TargetName = target.Name,
                    TargetDescription = target.Description,
                    TargetType = target.Type.ToString(),
                    PageBreak = @"<span style=""page-break-after: always;""></span>",
                    Today = DateTime.Now.ToShortDateString(),
                    Storage1Note = mastg.Storage1Note,
                    Storage1Status = mastg.Storage1Status.ToString(),
                    Storage1Note1 = mastg.Storage1Note1,
                    Storage1Status1 = mastg.Storage1Status1.ToString(),
                    Storage1Note2 = mastg.Storage1Note2,
                    Storage1Status2 = mastg.Storage1Status2.ToString(),
                    Storage1Note3 = mastg.Storage1Note3,
                    Storage1Status3 = mastg.Storage1Status3.ToString(),
                    Storage2Note = mastg.Storage2Note,
                    Storage2Status = mastg.Storage2Status.ToString(),
                    Storage2Note1 = mastg.Storage2Note1,
                    Storage2Status1 = mastg.Storage2Status1.ToString(),
                    Storage2Note2 = mastg.Storage2Note2,
                    Storage2Status2 = mastg.Storage2Status2.ToString(),
                    Storage2Note3 = mastg.Storage2Note3,
                    Storage2Status3 = mastg.Storage2Status3.ToString(),
                    Storage2Note4 = mastg.Storage2Note4,
                    Storage2Status4 = mastg.Storage2Status4.ToString(),
                    Storage2Note5 = mastg.Storage2Note5,
                    Storage2Status5 = mastg.Storage2Status5.ToString(),
                    Storage2Note6 = mastg.Storage2Note6,
                    Storage2Status6 = mastg.Storage2Status6.ToString(),
                    Storage2Note7 = mastg.Storage2Note7,
                    Storage2Status7 = mastg.Storage2Status7.ToString(),
                    Storage2Note8 = mastg.Storage2Note8,
                    Storage2Status8 = mastg.Storage2Status8.ToString(),
                    Storage2Note9 = mastg.Storage2Note9,
                    Storage2Status9 = mastg.Storage2Status9.ToString(),
                    Storage2Note10 = mastg.Storage2Note10,
                    Storage2Status10 = mastg.Storage2Status10.ToString(),
                    Storage2Note11 = mastg.Storage2Note11,
                    Storage2Status11 = mastg.Storage2Status11.ToString(),
                    Crypto1Note = mastg.Crypto1Note,
                    Crypto1Status = mastg.Crypto1Status.ToString(),
                    Crypto1Note1 = mastg.Crypto1Note1,
                    Crypto1Status1 = mastg.Crypto1Status1.ToString(),
                    Crypto1Note2 = mastg.Crypto1Note2,
                    Crypto1Status2 = mastg.Crypto1Status2.ToString(),
                    Crypto1Note3 = mastg.Crypto1Note3,
                    Crypto1Status3 = mastg.Crypto1Status3.ToString(),
                    Crypto1Note4 = mastg.Crypto1Note4,
                    Crypto1Status4 = mastg.Crypto1Status4.ToString(),
                    Crypto1Note5 = mastg.Crypto1Note5,
                    Crypto1Status5 = mastg.Crypto1Status5.ToString(),
                    Crypto2Note = mastg.Crypto2Note,
                    Crypto2Status = mastg.Crypto2Status.ToString(),
                    Crypto2Note1 = mastg.Crypto2Note1,
                    Crypto2Status1 = mastg.Crypto2Status1.ToString(),
                    Crypto2Note2 = mastg.Crypto2Note2,
                    Crypto2Status2 = mastg.Crypto2Status2.ToString(),
                    Auth1Note = mastg.Auth1Note,
                    Auth1Status = mastg.Auth1Status.ToString(),
                    Auth2Note = mastg.Auth2Note,
                    Auth2Status = mastg.Auth2Status.ToString(),
                    Auth2Note1 = mastg.Auth2Note1,
                    Auth2Status1 = mastg.Auth2Status1.ToString(),
                    Auth2Note2 = mastg.Auth2Note2,
                    Auth2Status2 = mastg.Auth2Status2.ToString(),
                    Auth2Note3 = mastg.Auth2Note3,
                    Auth2Status3 = mastg.Auth2Status3.ToString(),
                    Auth3Note = mastg.Auth3Note,
                    Auth3Status = mastg.Auth3Status.ToString(),
                    Network1Note = mastg.Network1Note,
                    Network1Status = mastg.Network1Status.ToString(),
                    Network1Note1 = mastg.Network1Note1,
                    Network1Status1 = mastg.Network1Status1.ToString(),
                    Network1Note2 = mastg.Network1Note2,
                    Network1Status2 = mastg.Network1Status2.ToString(),
                    Network1Note3 = mastg.Network1Note3,
                    Network1Status3 = mastg.Network1Status3.ToString(),
                    Network1Note4 = mastg.Network1Note4,
                    Network1Status4 = mastg.Network1Status4.ToString(),
                    Network1Note5 = mastg.Network1Note5,
                    Network1Status5 = mastg.Network1Status5.ToString(),
                    Network1Note6 = mastg.Network1Note6,
                    Network1Status6 = mastg.Network1Status6.ToString(),
                    Network1Note7 = mastg.Network1Note7,
                    Network1Status7 = mastg.Network1Status7.ToString(),
                    Network2Note = mastg.Network2Note,
                    Network2Status = mastg.Network2Status.ToString(),
                    Network2Note1 = mastg.Network2Note1,
                    Network2Status1 = mastg.Network2Status1.ToString(),
                    Network2Note2 = mastg.Network2Note2,
                    Network2Status2 = mastg.Network2Status2.ToString(),
                    Platform1Note = mastg.Platform1Note,
                    Platform1Status = mastg.Platform1Status.ToString(),
                    Platform1Note1 = mastg.Platform1Note1,
                    Platform1Status1 = mastg.Platform1Status1.ToString(),
                    Platform1Note2 = mastg.Platform1Note2,
                    Platform1Status2 = mastg.Platform1Status2.ToString(),
                    Platform1Note3 = mastg.Platform1Note3,
                    Platform1Status3 = mastg.Platform1Status3.ToString(),
                    Platform1Note4 = mastg.Platform1Note4,
                    Platform1Status4 = mastg.Platform1Status4.ToString(),
                    Platform1Note5 = mastg.Platform1Note5,
                    Platform1Status5 = mastg.Platform1Status5.ToString(),
                    Platform1Note6 = mastg.Platform1Note6,
                    Platform1Status6 = mastg.Platform1Status6.ToString(),
                    Platform1Note7 = mastg.Platform1Note7,
                    Platform1Status7 = mastg.Platform1Status7.ToString(),
                    Platform1Note8 = mastg.Platform1Note8,
                    Platform1Status8 = mastg.Platform1Status8.ToString(),
                    Platform1Note9 = mastg.Platform1Note9,
                    Platform1Status9 = mastg.Platform1Status9.ToString(),
                    Platform1Note10 = mastg.Platform1Note10,
                    Platform1Status10 = mastg.Platform1Status10.ToString(),
                    Platform1Note11 = mastg.Platform1Note11,
                    Platform1Status11 = mastg.Platform1Status11.ToString(),
                    Platform1Note12 = mastg.Platform1Note12,
                    Platform1Status12 = mastg.Platform1Status12.ToString(),
                    Platform1Note13 = mastg.Platform1Note13,
                    Platform1Status13 = mastg.Platform1Status13.ToString(),
                    Platform2Note = mastg.Platform2Note,
                    Platform2Status = mastg.Platform2Status.ToString(),
                    Platform2Note1 = mastg.Platform2Note1,
                    Platform2Status1 = mastg.Platform2Status1.ToString(),
                    Platform2Note2 = mastg.Platform2Note2,
                    Platform2Status2 = mastg.Platform2Status2.ToString(),
                    Platform2Note3 = mastg.Platform2Note3,
                    Platform2Status3 = mastg.Platform2Status3.ToString(),
                    Platform2Note4 = mastg.Platform2Note4,
                    Platform2Status4 = mastg.Platform2Status4.ToString(),
                    Platform2Note5 = mastg.Platform2Note5,
                    Platform2Status5 = mastg.Platform2Status5.ToString(),
                    Platform2Note6 = mastg.Platform2Note6,
                    Platform2Status6 = mastg.Platform2Status6.ToString(),
                    Platform2Note7 = mastg.Platform2Note7,
                    Platform2Status7 = mastg.Platform2Status7.ToString(),
                    Platform3Note = mastg.Platform3Note,
                    Platform3Status = mastg.Platform3Status.ToString(),
                    Platform3Note1 = mastg.Platform3Note1,
                    Platform3Status1 = mastg.Platform3Status1.ToString(),
                    Platform3Note2 = mastg.Platform3Note2,
                    Platform3Status2 = mastg.Platform3Status2.ToString(),
                    Platform3Note3 = mastg.Platform3Note3,
                    Platform3Status3 = mastg.Platform3Status3.ToString(),
                    Platform3Note4 = mastg.Platform3Note4,
                    Platform3Status4 = mastg.Platform3Status4.ToString(),
                    Platform3Note5 = mastg.Platform3Note5,
                    Platform3Status5 = mastg.Platform3Status5.ToString(),
                    Code1Note = mastg.Code1Note,
                    Code1Status = mastg.Code1Status.ToString(),
                    Code2Note = mastg.Code2Note,
                    Code2Status = mastg.Code2Status.ToString(),
                    Code2Note1 = mastg.Code2Note1,
                    Code2Status1 = mastg.Code2Status1.ToString(),
                    Code2Note2 = mastg.Code2Note2,
                    Code2Status2 = mastg.Code2Status2.ToString(),
                    Code3Note = mastg.Code3Note,
                    Code3Status = mastg.Code3Status.ToString(),
                    Code3Note1 = mastg.Code3Note1,
                    Code3Status1 = mastg.Code3Status1.ToString(),
                    Code3Note2 = mastg.Code3Note2,
                    Code3Status2 = mastg.Code3Status2.ToString(),
                    Code4Note = mastg.Code4Note,
                    Code4Status = mastg.Code4Status.ToString(),
                    Code4Note1 = mastg.Code4Note1,
                    Code4Status1 = mastg.Code4Status1.ToString(),
                    Code4Note2 = mastg.Code4Note2,
                    Code4Status2 = mastg.Code4Status2.ToString(),
                    Code4Note3 = mastg.Code4Note3,
                    Code4Status3 = mastg.Code4Status3.ToString(),
                    Code4Note4 = mastg.Code4Note4,
                    Code4Status4 = mastg.Code4Status4.ToString(),
                    Code4Note5 = mastg.Code4Note5,
                    Code4Status5 = mastg.Code4Status5.ToString(),
                    Code4Note6 = mastg.Code4Note6,
                    Code4Status6 = mastg.Code4Status6.ToString(),
                    Code4Note7 = mastg.Code4Note7,
                    Code4Status7 = mastg.Code4Status7.ToString(),
                    Code4Note8 = mastg.Code4Note8,
                    Code4Status8 = mastg.Code4Status8.ToString(),
                    Code4Note9 = mastg.Code4Note9,
                    Code4Status9 = mastg.Code4Status9.ToString(),
                    Code4Note10 = mastg.Code4Note10,
                    Code4Status10 = mastg.Code4Status10.ToString(),
                    Resilience1Note = mastg.Resilience1Note,
                    Resilience1Status = mastg.Resilience1Status.ToString(),
                    Resilience1Note1 = mastg.Resilience1Note1,
                    Resilience1Status1 = mastg.Resilience1Status1.ToString(),
                    Resilience1Note2 = mastg.Resilience1Note2,
                    Resilience1Status2 = mastg.Resilience1Status2.ToString(),
                    Resilience1Note3 = mastg.Resilience1Note3,
                    Resilience1Status3 = mastg.Resilience1Status3.ToString(),
                    Resilience1Note4 = mastg.Resilience1Note4,
                    Resilience1Status4 = mastg.Resilience1Status4.ToString(),
                    Resilience2Status = mastg.Resilience2Status.ToString(),
                    Resilience2Note = mastg.Resilience2Note,
                    Resilience2Status1 = mastg.Resilience2Status1.ToString(),
                    Resilience2Note1 = mastg.Resilience2Note1,
                    Resilience2Status2 = mastg.Resilience2Status2.ToString(),
                    Resilience2Note2 = mastg.Resilience2Note2,
                    Resilience2Status3 = mastg.Resilience2Status3.ToString(),
                    Resilience2Note3 = mastg.Resilience2Note3,
                    Resilience2Status4 = mastg.Resilience2Status4.ToString(),
                    Resilience2Note4 = mastg.Resilience2Note4,
                    Resilience2Status5 = mastg.Resilience2Status5.ToString(),
                    Resilience2Note5 = mastg.Resilience2Note5,
                    Resilience3Note = mastg.Resilience3Note,
                    Resilience3Status = mastg.Resilience3Status.ToString(),
                    Resilience3Note1 = mastg.Resilience3Note1,
                    Resilience3Status1 = mastg.Resilience3Status1.ToString(),
                    Resilience3Note2 = mastg.Resilience3Note2,
                    Resilience3Status2 = mastg.Resilience3Status2.ToString(),
                    Resilience3Note3 = mastg.Resilience3Note3,
                    Resilience3Status3 = mastg.Resilience3Status3.ToString(),
                    Resilience3Note4 = mastg.Resilience3Note4,
                    Resilience3Status4 = mastg.Resilience3Status4.ToString(),
                    Resilience3Note5 = mastg.Resilience3Note5,
                    Resilience3Status5 = mastg.Resilience3Status5.ToString(),
                    Resilience3Note6 = mastg.Resilience3Note6,
                    Resilience3Status6 = mastg.Resilience3Status6.ToString(),
                    Resilience4Note = mastg.Resilience4Note,
                    Resilience4Status = mastg.Resilience4Status.ToString(),
                    Resilience4Status1 = mastg.Resilience4Status1.ToString(),
                    Resilience4Note1 = mastg.Resilience4Note1,
                    Resilience4Status2 = mastg.Resilience4Status2.ToString(),
                    Resilience4Note2 = mastg.Resilience4Note2,
                    Resilience4Status3 = mastg.Resilience4Status3.ToString(),
                    Resilience4Note3 = mastg.Resilience4Note3,
                    Resilience4Status4 = mastg.Resilience4Status4.ToString(),
                    Resilience4Note4 = mastg.Resilience4Note4,
                    Resilience4Status5 = mastg.Resilience4Status5.ToString(),
                    Resilience4Note5 = mastg.Resilience4Note5,
                    Resilience4Status6 = mastg.Resilience4Status6.ToString(),
                    Resilience4Note6 = mastg.Resilience4Note6,
                };

               
                var resultHtml = templateHtml(data);

                rep.HtmlCode = resultHtml;
                reportManager.Add(rep);
                reportManager.Context.SaveChanges();
                _logger.LogInformation("Report MASVS generated successfully. User: {0}",
                    aspNetUserId);
                return Ok();
               
           }
           
           return BadRequest();

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred making backup data. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
    }
    
      [HttpPost]
      [Route("WSTG/GenerateReport")]
    public async Task<IActionResult> GenerateWstgReport([FromBody] ReportChecklistCreateModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var wstg = wstgManager.GetById(model.ChecklistId);

                if (wstg != null)
                {
                    var user = projectUserManager.VerifyUser(wstg.ProjectId, aspNetUserId);
                    if (user == null)
                    {
                        return BadRequest();
                    }
                    
                    var pro = projectManager.GetById(model.ProjectId);
                var template = reportTemplateManager.GetById(model.ReportTemplateId);
                
                var sanitizer = new HtmlSanitizer();
                sanitizer.AllowedSchemes.Add("data");

                Report rep = new Report
                {
                    Id = Guid.NewGuid(),
                    Name = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Name)),
                    ProjectId = model.ProjectId,
                    UserId = aspNetUserId,
                    CreatedDate = DateTime.Now.ToUniversalTime(),
                    Description = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Description)),
                    Version = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Version)),
                    Language = pro.Language,
                    ReportType = ReportType.WSTG
                };


                var Organization = organizationManager.GetAll().First();
                var Client = clientManager.GetById(pro.ClientId);
                

                var reportParts = reportsPartsManager.GetAll().Where(x => x.TemplateId == model.ReportTemplateId)
                    .OrderBy(x => x.Order).Include(x => x.Component).ToList();
                string source = @"<!DOCTYPE html>
                <html>
                <head>
                    <meta charset=""utf-8"">
                    <title></title>
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
                </head>
                <body>
                    <header>
                        {{HeaderComponents}}
                    </header>
                    <cover>
                        {{CoverComponents}}
                    </cover>
                    
                    {{BodyComponents}}

                    <footer> 
                        {{FooterComponents}}
                    </footer>
                </body>
                </html>";

                StringBuilder sbHeader = new StringBuilder();
                foreach (var part in reportParts.Where(x => x.Component.ComponentType == ReportPartType.Header)
                             .OrderBy(x => x.Order))
                {
                    sbHeader.Append(part.Component.Content);
                }

                source = source.Replace("{{HeaderComponents}}", sbHeader.ToString());


                StringBuilder sbCover = new StringBuilder();
                foreach (var part in reportParts.Where(x => x.Component.ComponentType == ReportPartType.Cover)
                             .OrderBy(x => x.Order))
                {
                    sbCover.Append(part.Component.Content);
                }

                source = source.Replace("{{CoverComponents}}", sbCover.ToString());

                StringBuilder sbBody = new StringBuilder();
                foreach (var part in reportParts.Where(x => x.Component.ComponentType == ReportPartType.Body)
                             .OrderBy(x => x.Order))
                {
                    sbBody.Append(part.Component.Content);
                }

                source = source.Replace("{{BodyComponents}}", sbBody.ToString());


                StringBuilder sbFooter = new StringBuilder();
                foreach (var part in reportParts.Where(x => x.Component.ComponentType == ReportPartType.Footer)
                             .OrderBy(x => x.Order))
                {
                    sbBody.Append(part.Component.Content);
                }

                source = source.Replace("{{FooterComponents}}", sbFooter.ToString());


                var templateHtml = Handlebars.Compile(source);


                var data = new
                {
                    OrganizationName = Organization.Name,
                    ClientName = Client.Name,
                    OrganizationEmail = Organization.ContactEmail,
                    OrganizationPhone = Organization.ContactPhone,
                    ClientEmail = Client.ContactEmail,
                    ClientPhone = Client.ContactPhone,
                    ProjectName = pro.Name,
                    ProjectDescription = pro.Description,
                    CreatedDate = DateTime.Now.ToUniversalTime().ToShortDateString(),
                    TargetUrl = wstg.Target.Name,
                    TargetDescription = wstg.Target.Description,
                    Info01Status = wstg.Info01Status.ToString(),
                    Info01Note = wstg.Info01Note,
                    Info02Status = wstg.Info02Status.ToString(),
                    Info02Note = wstg.Info02Note,
                    Info03Status = wstg.Info03Status.ToString(),
                    Info03Note = wstg.Info03Note,
                    Info04Status = wstg.Info04Status.ToString(),
                    Info04Note = wstg.Info04Note,
                    Info05Status = wstg.Info05Status.ToString(),
                    Info05Note = wstg.Info05Note,
                    Info06Status = wstg.Info06Status.ToString(),
                    Info06Note = wstg.Info06Note,
                    Info07Status = wstg.Info07Status.ToString(),
                    Info07Note = wstg.Info07Note,
                    Info08Status = wstg.Info08Status.ToString(),
                    Info08Note = wstg.Info08Note,
                    Info09Status = wstg.Info09Status.ToString(),
                    Info09Note = wstg.Info09Note,
                    Info10Status = wstg.Info10Status.ToString(),
                    Info10Note = wstg.Info10Note,
                    
                    Conf01Status = wstg.Conf01Status.ToString(),
                    Conf01Note = wstg.Conf01Note,
                    Conf02Status = wstg.Conf02Status.ToString(),
                    Conf02Note = wstg.Conf02Note,
                    Conf03Status = wstg.Conf03Status.ToString(),
                    Conf03Note = wstg.Conf03Note,
                    Conf04Status = wstg.Conf04Status.ToString(),
                    Conf04Note = wstg.Conf04Note,
                    Conf05Status = wstg.Conf05Status.ToString(),
                    Conf05Note = wstg.Conf05Note,
                    Conf06Status = wstg.Conf06Status.ToString(),
                    Conf06Note = wstg.Conf06Note,
                    Conf07Status = wstg.Conf07Status.ToString(),
                    Conf07Note = wstg.Conf07Note,
                    Conf08Status = wstg.Conf08Status.ToString(),
                    Conf08Note = wstg.Conf08Note,
                    Conf09Status = wstg.Conf09Status.ToString(),
                    Conf09Note = wstg.Conf09Note,
                    Conf10Status = wstg.Conf10Status.ToString(),
                    Conf10Note = wstg.Conf10Note,
                    Conf11Status = wstg.Conf11Status.ToString(),
                    Conf11Note = wstg.Conf11Note,
                    
                    Idnt01Status = wstg.Idnt1Status.ToString(),
                    Idnt01Note = wstg.Idnt1Note,
                    Idnt02Status = wstg.Idnt2Status.ToString(),
                    Idnt02Note = wstg.Idnt2Note,
                    Idnt03Status = wstg.Idnt3Status.ToString(),
                    Idnt03Note = wstg.Idnt3Note,
                    Idnt04Status = wstg.Idnt4Status.ToString(),
                    Idnt04Note = wstg.Idnt4Note,
                    Idnt05Status = wstg.Idnt5Status.ToString(),
                    
                    Athn01Status = wstg.Athn01Status.ToString(),
                    Athn01Note = wstg.Athn01Note,
                    Athn02Status = wstg.Athn02Status.ToString(),
                    Athn02Note = wstg.Athn02Note,
                    Athn03Status = wstg.Athn03Status.ToString(),
                    Athn03Note = wstg.Athn03Note,
                    Athn04Status = wstg.Athn04Status.ToString(),
                    Athn04Note = wstg.Athn04Note,
                    Athn05Status = wstg.Athn05Status.ToString(),
                    Athn05Note = wstg.Athn05Note,
                    Athn06Status = wstg.Athn06Status.ToString(),
                    Athn06Note = wstg.Athn06Note,
                    Athn07Status = wstg.Athn07Status.ToString(),
                    Athn07Note = wstg.Athn07Note,
                    Athn08Status = wstg.Athn08Status.ToString(),
                    Athn08Note = wstg.Athn08Note,
                    Athn09Status = wstg.Athn09Status.ToString(),
                    Athn09Note = wstg.Athn09Note,
                    Athn10Status = wstg.Athn10Status.ToString(),
                    
                    Sess01Status = wstg.Sess01Status.ToString(),
                    Sess01Note = wstg.Sess01Note,
                    Sess02Status = wstg.Sess02Status.ToString(),
                    Sess02Note = wstg.Sess02Note,
                    Sess03Status = wstg.Sess03Status.ToString(),
                    Sess03Note = wstg.Sess03Note,
                    Sess04Status = wstg.Sess04Status.ToString(),
                    Sess04Note = wstg.Sess04Note,
                    Sess05Status = wstg.Sess05Status.ToString(),
                    Sess05Note = wstg.Sess05Note,
                    Sess06Status = wstg.Sess06Status.ToString(),
                    Sess06Note = wstg.Sess06Note,
                    Sess07Status = wstg.Sess07Status.ToString(),
                    Sess07Note = wstg.Sess07Note,
                    Sess08Status = wstg.Sess08Status.ToString(),
                    Sess08Note = wstg.Sess08Note,
                    Sess09Status = wstg.Sess09Status.ToString(),
                    Sess09Note = wstg.Sess09Note,
                    
                    Inpv01Status = wstg.Inpv01Status.ToString(),
                    Inpv01Note = wstg.Inpv01Note,
                    Inpv02Status = wstg.Inpv02Status.ToString(),
                    Inpv02Note = wstg.Inpv02Note,
                    Inpv03Status = wstg.Inpv03Status.ToString(),
                    Inpv03Note = wstg.Inpv03Note,
                    Inpv04Status = wstg.Inpv04Status.ToString(),
                    Inpv04Note = wstg.Inpv04Note,
                    Inpv05Status = wstg.Inpv05Status.ToString(),
                    Inpv05Note = wstg.Inpv05Note,
                    Inpv06Status = wstg.Inpv06Status.ToString(),
                    Inpv06Note = wstg.Inpv06Note,
                    Inpv07Status = wstg.Inpv07Status.ToString(),
                    Inpv07Note = wstg.Inpv07Note,
                    Inpv08Status = wstg.Inpv08Status.ToString(),
                    Inpv08Note = wstg.Inpv08Note,
                    Inpv09Status = wstg.Inpv09Status.ToString(),
                    Inpv09Note = wstg.Inpv09Note,
                    Inpv10Status = wstg.Inpv10Status.ToString(),
                    Inpv10Note = wstg.Inpv10Note,
                    Inpv11Status = wstg.Inpv11Status.ToString(),
                    Inpv11Note = wstg.Inpv11Note,
                    Inpv12Status = wstg.Inpv12Status.ToString(),
                    Inpv12Note = wstg.Inpv12Note,
                    Inpv13Status = wstg.Inpv13Status.ToString(),
                    Inpv13Note = wstg.Inpv13Note,
                    Inpv14Status = wstg.Inpv14Status.ToString(),
                    Inpv14Note = wstg.Inpv14Note,
                    Inpv15Status = wstg.Inpv15Status.ToString(),
                    Inpv15Note = wstg.Inpv15Note,
                    Inpv16Status = wstg.Inpv16Status.ToString(),
                    Inpv16Note = wstg.Inpv16Note,
                    Inpv17Status = wstg.Inpv17Status.ToString(),
                    Inpv17Note = wstg.Inpv17Note,   
                    Inpv18Status = wstg.Inpv18Status.ToString(),
                    Inpv18Note = wstg.Inpv18Note,
                    Inpv19Status = wstg.Inpv19Status.ToString(),
                    Inpv19Note = wstg.Inpv19Note,
                    
                    Errh01Status = wstg.Errh01Status.ToString(),
                    Errh01Note = wstg.Errh01Note,
                    Errh02Status = wstg.Errh02Status.ToString(),
                    Errh02Note = wstg.Errh02Note,
                    
                    Cryp01Status = wstg.Cryp01Status.ToString(),
                    Cryp01Note = wstg.Cryp01Note,
                    Cryp02Status = wstg.Cryp02Status.ToString(),
                    Cryp02Note = wstg.Cryp02Note,
                    Cryp03Status = wstg.Cryp03Status.ToString(),
                    Cryp03Note = wstg.Cryp03Note,
                    Cryp04Status = wstg.Cryp04Status.ToString(),
                    Cryp04Note = wstg.Cryp04Note,

                    Busl01Status = wstg.Busl01Status.ToString(),
                    Busl01Note = wstg.Busl01Note,
                    Busl02Status = wstg.Busl02Status.ToString(),
                    Busl02Note = wstg.Busl02Note,
                    Busl03Status = wstg.Busl03Status.ToString(),
                    Busl03Note = wstg.Busl03Note,
                    Busl04Status = wstg.Busl04Status.ToString(),    
                    Busl04Note = wstg.Busl04Note,
                    Busl05Status = wstg.Busl05Status.ToString(),
                    Busl05Note = wstg.Busl05Note,
                    Busl06Status = wstg.Busl06Status.ToString(),
                    Busl06Note = wstg.Busl06Note,
                    Busl07Status = wstg.Busl07Status.ToString(),
                    Busl07Note = wstg.Busl07Note,
                    Busl08Status = wstg.Busl08Status.ToString(),
                    Busl08Note = wstg.Busl08Note,
                    Busl09Status = wstg.Busl09Status.ToString(),
                    Busl09Note = wstg.Busl09Note,

                    Clnt01Status = wstg.Clnt01Status.ToString(),
                    Clnt01Note = wstg.Clnt01Note,
                    Clnt02Status = wstg.Clnt02Status.ToString(),
                    Clnt02Note = wstg.Clnt02Note,
                    Clnt03Status = wstg.Clnt03Status.ToString(),
                    Clnt03Note = wstg.Clnt03Note,
                    Clnt04Status = wstg.Clnt04Status.ToString(),
                    Clnt04Note = wstg.Clnt04Note,
                    Clnt05Status = wstg.Clnt05Status.ToString(),
                    Clnt05Note = wstg.Clnt05Note,
                    Clnt06Status = wstg.Clnt06Status.ToString(),
                    Clnt06Note = wstg.Clnt06Note,
                    Clnt07Status = wstg.Clnt07Status.ToString(),
                    Clnt07Note = wstg.Clnt07Note,
                    Clnt08Status = wstg.Clnt08Status.ToString(),
                    Clnt08Note = wstg.Clnt08Note,
                    Clnt09Status = wstg.Clnt09Status.ToString(),
                    Clnt09Note = wstg.Clnt09Note,
                    Clnt10Status = wstg.Clnt10Status.ToString(),
                    Clnt10Note = wstg.Clnt10Note,
                    Clnt11Status = wstg.Clnt11Status.ToString(),
                    Clnt11Note = wstg.Clnt11Note,
                    Clnt12Status = wstg.Clnt12Status.ToString(),
                    Clnt12Note = wstg.Clnt12Note,
                    Clnt13Status = wstg.Clnt13Status.ToString(),
                    Clnt13Note = wstg.Clnt13Note,
                    
                    Apit01Status = wstg.Apit01Status.ToString(),
                    Apit01Note = wstg.Apit01Note,
                };

                var resultHtml = templateHtml(data);

                rep.HtmlCode = resultHtml;
                reportManager.Add(rep);
                reportManager.Context.SaveChanges();

                _logger.LogInformation("Report generated successfully. User: {0}",
                    aspNetUserId);
                return Ok();
                }
                
                _logger.LogError("An error ocurred generating report. User: {0}",
                    aspNetUserId);
                return BadRequest();
            }

            _logger.LogError("An error ocurred generating report. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred generating report. User: {0}",
                aspNetUserId);
            throw;
        }
    }
}