using System.Security.Claims;
using System.Text;
using System.Web;
using AuthPermissions.AspNetCore;
using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.CORE.ViewModel.Mastg;
using Cervantes.CORE.ViewModel.Wstg;
using Cervantes.Web.Helpers;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Ganss.Xss;
using HtmlToOpenXml;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Scriban;
using Scriban.Runtime;

namespace Cervantes.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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
    private Sanitizer Sanitizer;
    
    public ChecklistController(IProjectManager projectManager, 
        IProjectUserManager projectUserManager, ITargetManager targetManager, ILogger<ChecklistController> logger, IWSTGManager wstgManager,
        IMASTGManager mastgManager,IWebHostEnvironment env, IOrganizationManager organizationManager, IClientManager clientManager,IHttpContextAccessor HttpContextAccessor,
        IReportComponentsManager reportComponentsManager, IReportsPartsManager reportsPartsManager, IReportManager 
            reportManager, IReportTemplateManager reportTemplateManager,Sanitizer Sanitizer)
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
        this.Sanitizer = Sanitizer;
    }
    
    [HttpGet]
    [Route("WSTG/Project/{projectId}")]
    [HasPermission(Permissions.ChecklistsRead)]
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
    [HasPermission(Permissions.ChecklistsRead)]
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
    [HasPermission(Permissions.ChecklistsRead)]
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
    [HasPermission(Permissions.ChecklistsRead)]
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
    [HasPermission(Permissions.ChecklistsAdd)]
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
                        return CreatedAtAction(nameof(GetWSTGById), new { checklistId = wstg.Id }, wstg);
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
                        return CreatedAtAction(nameof(GetMASTGById), new { checklistId = mstg.Id }, mstg);
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
    [HasPermission(Permissions.ChecklistsEdit)]
    public async Task<IActionResult> EditWstg([FromBody] WSTGViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                
               var result = wstgManager.GetById(model.Id);
              
               if (result.Id != Guid.Empty)
               {
                   var user = projectUserManager.VerifyUser(result.ProjectId, aspNetUserId);
                   if (user == null)
                   {
                       return BadRequest();
                   }
                   
                   result.Apit01Note = Sanitizer.Sanitize(model.Apit.Apit01Note);
                   result.Apit01Status = model.Apit.Apit01Status;

                   result.Athz01Note = Sanitizer.Sanitize(model.Athz.Athz01Note);
                   result.Athz01Status = model.Athz.Athz01Status;
                   result.Athz02Note = Sanitizer.Sanitize(model.Athz.Athz02Note);
                   result.Athz02Status = model.Athz.Athz02Status;
                   result.Athz03Note = Sanitizer.Sanitize(model.Athz.Athz03Note);
                   result.Athz03Status = model.Athz.Athz03Status;
                   result.Athz04Note = Sanitizer.Sanitize(model.Athz.Athz04Note);
                   result.Athz04Status = model.Athz.Athz04Status;

                   result.Athn01Note = Sanitizer.Sanitize(model.Auth.Athn01Note);
                   result.Athn01Status = model.Auth.Athn01Status;
                   result.Athn02Note = Sanitizer.Sanitize(model.Auth.Athn02Note);
                   result.Athn02Status = model.Auth.Athn02Status;
                   result.Athn03Note = Sanitizer.Sanitize(model.Auth.Athn03Note);
                   result.Athn03Status = model.Auth.Athn03Status;
                   result.Athn04Note = Sanitizer.Sanitize(model.Auth.Athn04Note);
                   result.Athn04Status = model.Auth.Athn04Status;
                   result.Athn05Note = Sanitizer.Sanitize(model.Auth.Athn05Note);
                   result.Athn05Status = model.Auth.Athn05Status;
                   result.Athn06Note = Sanitizer.Sanitize(model.Auth.Athn06Note);
                   result.Athn06Status = model.Auth.Athn06Status;
                   result.Athn07Note = Sanitizer.Sanitize(model.Auth.Athn07Note);
                   result.Athn07Status = model.Auth.Athn07Status;
                   result.Athn08Note = Sanitizer.Sanitize(model.Auth.Athn08Note);
                   result.Athn08Status = model.Auth.Athn08Status;
                   result.Athn09Note = Sanitizer.Sanitize(model.Auth.Athn09Note);
                   result.Athn09Status = model.Auth.Athn09Status;
                   result.Athn10Note = Sanitizer.Sanitize(model.Auth.Athn10Note);
                   result.Athn10Status = model.Auth.Athn10Status;

                   result.Busl01Note = Sanitizer.Sanitize(model.Busl.Busl01Note);
                   result.Busl01Status = model.Busl.Busl01Status;
                   result.Busl02Note = Sanitizer.Sanitize(model.Busl.Busl02Note);
                   result.Busl02Status = model.Busl.Busl02Status;
                   result.Busl03Note = Sanitizer.Sanitize(model.Busl.Busl03Note);
                   result.Busl03Status = model.Busl.Busl03Status;
                   result.Busl04Note = Sanitizer.Sanitize(model.Busl.Busl04Note);
                   result.Busl04Status = model.Busl.Busl04Status;
                   result.Busl05Note = Sanitizer.Sanitize(model.Busl.Busl05Note);
                   result.Busl05Status = model.Busl.Busl05Status;
                   result.Busl06Note = Sanitizer.Sanitize(model.Busl.Busl06Note);
                   result.Busl06Status = model.Busl.Busl06Status;
                   result.Busl07Note = Sanitizer.Sanitize(model.Busl.Busl07Note);
                   result.Busl07Status = model.Busl.Busl07Status;
                   result.Busl08Note = Sanitizer.Sanitize(model.Busl.Busl08Note);
                   result.Busl08Status = model.Busl.Busl08Status;
                   result.Busl09Note = Sanitizer.Sanitize(model.Busl.Busl09Note);
                   result.Busl09Status = model.Busl.Busl09Status;

                   result.Clnt01Note = Sanitizer.Sanitize(model.Clnt.Clnt01Note);
                   result.Clnt01Status = model.Clnt.Clnt01Status;
                   result.Clnt02Note = Sanitizer.Sanitize(model.Clnt.Clnt02Note);
                   result.Clnt02Status = model.Clnt.Clnt02Status;
                   result.Clnt03Note = Sanitizer.Sanitize(model.Clnt.Clnt03Note);
                   result.Clnt03Status = model.Clnt.Clnt03Status;
                   result.Clnt04Note = Sanitizer.Sanitize(model.Clnt.Clnt04Note);
                   result.Clnt04Status = model.Clnt.Clnt04Status;
                   result.Clnt05Note = Sanitizer.Sanitize(model.Clnt.Clnt05Note);
                   result.Clnt05Status = model.Clnt.Clnt05Status;
                   result.Clnt06Note = Sanitizer.Sanitize(model.Clnt.Clnt06Note);
                   result.Clnt06Status = model.Clnt.Clnt06Status;
                   result.Clnt07Note = Sanitizer.Sanitize(model.Clnt.Clnt07Note);
                   result.Clnt07Status = model.Clnt.Clnt07Status;
                   result.Clnt08Note = Sanitizer.Sanitize(model.Clnt.Clnt08Note);
                   result.Clnt08Status = model.Clnt.Clnt08Status;
                   result.Clnt09Note = Sanitizer.Sanitize(model.Clnt.Clnt09Note);
                   result.Clnt09Status = model.Clnt.Clnt09Status;
                   result.Clnt10Note = Sanitizer.Sanitize(model.Clnt.Clnt10Note);
                   result.Clnt10Status = model.Clnt.Clnt10Status;
                   result.Clnt11Note = Sanitizer.Sanitize(model.Clnt.Clnt11Note);
                   result.Clnt11Status = model.Clnt.Clnt11Status;
                   result.Clnt12Note = Sanitizer.Sanitize(model.Clnt.Clnt12Note);
                   result.Clnt12Status = model.Clnt.Clnt12Status;
                   result.Clnt13Note = Sanitizer.Sanitize(model.Clnt.Clnt13Note);
                   result.Clnt13Status = model.Clnt.Clnt13Status;

                   result.Conf01Note = Sanitizer.Sanitize(model.Conf.Conf01Note);
                   result.Conf01Status = model.Conf.Conf01Status;
                   result.Conf02Note = Sanitizer.Sanitize(model.Conf.Conf02Note);
                   result.Conf02Status = model.Conf.Conf02Status;
                   result.Conf03Note = Sanitizer.Sanitize(model.Conf.Conf03Note);
                   result.Conf03Status = model.Conf.Conf03Status;
                   result.Conf04Note = Sanitizer.Sanitize(model.Conf.Conf04Note);
                   result.Conf04Status = model.Conf.Conf04Status;
                   result.Conf05Note = Sanitizer.Sanitize(model.Conf.Conf05Note);
                   result.Conf05Status = model.Conf.Conf05Status;
                   result.Conf06Note = Sanitizer.Sanitize(model.Conf.Conf06Note);
                   result.Conf06Status = model.Conf.Conf06Status;
                   result.Conf07Note = Sanitizer.Sanitize(model.Conf.Conf07Note);
                   result.Conf07Status = model.Conf.Conf07Status;
                   result.Conf08Note = Sanitizer.Sanitize(model.Conf.Conf08Note);
                   result.Conf08Status = model.Conf.Conf08Status;
                   result.Conf09Note = Sanitizer.Sanitize(model.Conf.Conf09Note);
                   result.Conf09Status = model.Conf.Conf09Status;
                   result.Conf10Note = Sanitizer.Sanitize(model.Conf.Conf10Note);
                   result.Conf10Status = model.Conf.Conf10Status;
                   result.Conf11Note = Sanitizer.Sanitize(model.Conf.Conf11Note);
                   result.Conf11Status = model.Conf.Conf11Status;

                   result.Cryp01Note = Sanitizer.Sanitize(model.Cryp.Cryp01Note);
                   result.Cryp01Status = model.Cryp.Cryp01Status;
                   result.Cryp02Note = Sanitizer.Sanitize(model.Cryp.Cryp02Note);
                   result.Cryp02Status = model.Cryp.Cryp02Status;
                   result.Cryp03Note = Sanitizer.Sanitize(model.Cryp.Cryp03Note);
                   result.Cryp03Status = model.Cryp.Cryp03Status;
                   result.Cryp04Note = Sanitizer.Sanitize(model.Cryp.Cryp04Note);
                   result.Cryp04Status = model.Cryp.Cryp04Status;

                   result.Errh01Note = Sanitizer.Sanitize(model.Errh.Errh01Note);
                   result.Errh01Status = model.Errh.Errh01Status;
                   result.Errh02Note = Sanitizer.Sanitize(model.Errh.Errh02Note);
                   result.Errh02Status = model.Errh.Errh02Status;

                   result.Idnt01Note = Sanitizer.Sanitize(model.Idnt.Idnt01Note);
                   result.Idnt01Status = model.Idnt.Idnt01Status;
                   result.Idnt02Note = Sanitizer.Sanitize(model.Idnt.Idnt02Note);
                   result.Idnt02Status = model.Idnt.Idnt02Status;
                   result.Idnt03Note = Sanitizer.Sanitize(model.Idnt.Idnt03Note);
                   result.Idnt03Status = model.Idnt.Idnt03Status;
                   result.Idnt04Note = Sanitizer.Sanitize(model.Idnt.Idnt04Note);
                   result.Idnt04Status = model.Idnt.Idnt04Status;
                   result.Idnt05Note = Sanitizer.Sanitize(model.Idnt.Idnt05Note);
                   result.Idnt05Status = model.Idnt.Idnt05Status;

                   result.Info01Note = Sanitizer.Sanitize(model.Info.Info01Note);
                   result.Info01Status = model.Info.Info01Status;
                   result.Info02Note = Sanitizer.Sanitize(model.Info.Info02Note);
                   result.Info02Status = model.Info.Info02Status;
                   result.Info03Note = Sanitizer.Sanitize(model.Info.Info03Note);
                   result.Info03Status = model.Info.Info03Status;
                   result.Info04Note = Sanitizer.Sanitize(model.Info.Info04Note);
                   result.Info04Status = model.Info.Info04Status;
                   result.Info05Note = Sanitizer.Sanitize(model.Info.Info05Note);
                   result.Info05Status = model.Info.Info05Status;
                   result.Info06Note = Sanitizer.Sanitize(model.Info.Info06Note);
                   result.Info06Status = model.Info.Info06Status;
                   result.Info07Note = Sanitizer.Sanitize(model.Info.Info07Note);
                   result.Info07Status = model.Info.Info07Status;
                   result.Info08Note = Sanitizer.Sanitize(model.Info.Info08Note);
                   result.Info08Status = model.Info.Info08Status;
                   result.Info09Note = Sanitizer.Sanitize(model.Info.Info09Note);
                   result.Info09Status = model.Info.Info09Status;
                   result.Info10Note = Sanitizer.Sanitize(model.Info.Info10Note);
                   result.Info10Status = model.Info.Info10Status;

                   result.Inpv01Note = Sanitizer.Sanitize(model.Inpv.Inpv01Note);
                   result.Inpv01Status = model.Inpv.Inpv01Status;
                   result.Inpv02Note = Sanitizer.Sanitize(model.Inpv.Inpv02Note);
                   result.Inpv02Status = model.Inpv.Inpv02Status;
                   result.Inpv03Note = Sanitizer.Sanitize(model.Inpv.Inpv03Note);
                   result.Inpv03Status = model.Inpv.Inpv03Status;
                   result.Inpv04Note = Sanitizer.Sanitize(model.Inpv.Inpv04Note);
                   result.Inpv04Status = model.Inpv.Inpv04Status;
                   result.Inpv05Note = Sanitizer.Sanitize(model.Inpv.Inpv05Note);
                   result.Inpv05Status = model.Inpv.Inpv05Status;
                   result.Inpv06Note = Sanitizer.Sanitize(model.Inpv.Inpv06Note);
                   result.Inpv06Status = model.Inpv.Inpv06Status;
                   result.Inpv07Note = Sanitizer.Sanitize(model.Inpv.Inpv07Note);
                   result.Inpv07Status = model.Inpv.Inpv07Status;
                   result.Inpv08Note = Sanitizer.Sanitize(model.Inpv.Inpv08Note);
                   result.Inpv08Status = model.Inpv.Inpv08Status;
                   result.Inpv09Note = Sanitizer.Sanitize(model.Inpv.Inpv09Note);
                   result.Inpv09Status = model.Inpv.Inpv09Status;
                   result.Inpv10Note = Sanitizer.Sanitize(model.Inpv.Inpv10Note);
                   result.Inpv10Status = model.Inpv.Inpv10Status;
                   result.Inpv11Note = Sanitizer.Sanitize(model.Inpv.Inpv11Note);
                   result.Inpv11Status = model.Inpv.Inpv11Status;
                   result.Inpv12Note = Sanitizer.Sanitize(model.Inpv.Inpv12Note);
                   result.Inpv12Status = model.Inpv.Inpv12Status;
                   result.Inpv13Note = Sanitizer.Sanitize(model.Inpv.Inpv13Note);
                   result.Inpv13Status = model.Inpv.Inpv13Status;
                   result.Inpv14Note = Sanitizer.Sanitize(model.Inpv.Inpv14Note);
                   result.Inpv14Status = model.Inpv.Inpv14Status;
                   result.Inpv15Note = Sanitizer.Sanitize(model.Inpv.Inpv15Note);
                   result.Inpv15Status = model.Inpv.Inpv15Status;
                   result.Inpv16Note = Sanitizer.Sanitize(model.Inpv.Inpv16Note);
                   result.Inpv16Status = model.Inpv.Inpv16Status;
                   result.Inpv17Note = Sanitizer.Sanitize(model.Inpv.Inpv17Note);
                   result.Inpv17Status = model.Inpv.Inpv17Status;
                   result.Inpv18Note = Sanitizer.Sanitize(model.Inpv.Inpv18Note);
                   result.Inpv18Status = model.Inpv.Inpv18Status;
                   result.Inpv19Note = Sanitizer.Sanitize(model.Inpv.Inpv19Note);
                   result.Inpv19Status = model.Inpv.Inpv19Status;

                   result.Sess01Note = Sanitizer.Sanitize(model.Sess.Sess01Note);
                   result.Sess01Status = model.Sess.Sess01Status;
                   result.Sess02Note = Sanitizer.Sanitize(model.Sess.Sess02Note);
                   result.Sess02Status = model.Sess.Sess02Status;
                   result.Sess03Note = Sanitizer.Sanitize(model.Sess.Sess03Note);
                   result.Sess03Status = model.Sess.Sess03Status;
                   result.Sess04Note = Sanitizer.Sanitize(model.Sess.Sess04Note);
                   result.Sess04Status = model.Sess.Sess04Status;
                   result.Sess05Note = Sanitizer.Sanitize(model.Sess.Sess05Note);
                   result.Sess05Status = model.Sess.Sess05Status;
                   result.Sess06Note = Sanitizer.Sanitize(model.Sess.Sess06Note);
                   result.Sess06Status = model.Sess.Sess06Status;
                   result.Sess07Note = Sanitizer.Sanitize(model.Sess.Sess07Note);
                   result.Sess07Status = model.Sess.Sess07Status;
                   result.Sess08Note = Sanitizer.Sanitize(model.Sess.Sess08Note);
                   result.Sess08Status = model.Sess.Sess08Status;
                   result.Sess09Note = Sanitizer.Sanitize(model.Sess.Sess09Note);
                   result.Sess09Status = model.Sess.Sess09Status;

                   await wstgManager.Context.SaveChangesAsync();
                   _logger.LogInformation("User: {0} edited Checklist: {1}", aspNetUserId, result.Id);
                   return NoContent();
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
    [HasPermission(Permissions.ChecklistsDelete)]
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
                return NoContent();
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
    [HasPermission(Permissions.ChecklistsDelete)]
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
                return NoContent();
            }
            _logger.LogError("An error occurred deleting a MASTG Checklist. User: {0}",
                aspNetUserId);
            return NoContent();
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
    [HasPermission(Permissions.ChecklistsEdit)]
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
                   
                   result.Storage1Note = Sanitizer.Sanitize(model.Storage.Storage1Note);
                     result.Storage1Status = model.Storage.Storage1Status;
                     result.Storage1Note1 = Sanitizer.Sanitize(model.Storage.Storage1Note1);
                        result.Storage1Status1 = model.Storage.Storage1Status1;
                        result.Storage1Note2 = Sanitizer.Sanitize(model.Storage.Storage1Note2);
                        result.Storage1Status2 = model.Storage.Storage1Status2;
                        result.Storage1Note3 =Sanitizer.Sanitize(model.Storage.Storage1Note3);
                        result.Storage1Status3 = model.Storage.Storage1Status3;
                        result.Storage2Note = Sanitizer.Sanitize(model.Storage.Storage2Note);
                        result.Storage2Status = model.Storage.Storage2Status;
                        result.Storage2Note1 = Sanitizer.Sanitize(model.Storage.Storage2Note1);
                        result.Storage2Status1 = model.Storage.Storage2Status1;
                        result.Storage2Note2 = Sanitizer.Sanitize(model.Storage.Storage2Note2);
                        result.Storage2Status2 = model.Storage.Storage2Status2;
                        result.Storage2Note3 = Sanitizer.Sanitize(model.Storage.Storage2Note3);
                        result.Storage2Status3 = model.Storage.Storage2Status3;
                        result.Storage2Note4 = Sanitizer.Sanitize(model.Storage.Storage2Note4);
                        result.Storage2Status4 = model.Storage.Storage2Status4;
                        result.Storage2Note5 = Sanitizer.Sanitize(model.Storage.Storage2Note5);
                        result.Storage2Status5 = model.Storage.Storage2Status5;
                        result.Storage2Note6 = Sanitizer.Sanitize(model.Storage.Storage2Note6);
                        result.Storage2Status6 = model.Storage.Storage2Status6;
                        result.Storage2Note7 = Sanitizer.Sanitize(model.Storage.Storage2Note7);
                        result.Storage2Status7 = model.Storage.Storage2Status7;
                        result.Storage2Note8 = Sanitizer.Sanitize(model.Storage.Storage2Note8);
                        result.Storage2Status8 = model.Storage.Storage2Status8;
                        result.Storage2Note9 = Sanitizer.Sanitize(model.Storage.Storage2Note9);
                        result.Storage2Status9 = model.Storage.Storage2Status9;
                        result.Storage2Note10 = Sanitizer.Sanitize(model.Storage.Storage2Note10);
                        result.Storage2Status10 = model.Storage.Storage2Status10;
                        result.Storage2Note11 = Sanitizer.Sanitize(model.Storage.Storage2Note11);
                        result.Storage2Status11 = model.Storage.Storage2Status11;
                        
                        result.Crypto1Note = Sanitizer.Sanitize(model.Crypto.Crypto1Note);
                        result.Crypto1Status = model.Crypto.Crypto1Status;
                        result.Crypto1Note1 = Sanitizer.Sanitize(model.Crypto.Crypto1Note1);
                        result.Crypto1Status1 = model.Crypto.Crypto1Status1;
                        result.Crypto1Note2 = Sanitizer.Sanitize(model.Crypto.Crypto1Note2);
                        result.Crypto1Status2 = model.Crypto.Crypto1Status2;
                        result.Crypto1Note3 = Sanitizer.Sanitize(model.Crypto.Crypto1Note3);
                        result.Crypto1Status3 = model.Crypto.Crypto1Status3;
                        result.Crypto1Note4 = Sanitizer.Sanitize(model.Crypto.Crypto1Note4);
                        result.Crypto1Status4 = model.Crypto.Crypto1Status4;
                        result.Crypto1Note5 = Sanitizer.Sanitize(model.Crypto.Crypto1Note5);
                        result.Crypto1Status5 = model.Crypto.Crypto1Status5;
                        result.Crypto2Note = Sanitizer.Sanitize(model.Crypto.Crypto2Note);
                        result.Crypto2Status = model.Crypto.Crypto2Status;
                        result.Crypto2Note1 = Sanitizer.Sanitize(model.Crypto.Crypto2Note1);
                        result.Crypto2Status1 = model.Crypto.Crypto2Status1;
                        result.Crypto2Note2 = Sanitizer.Sanitize(model.Crypto.Crypto2Note2);
                        result.Crypto2Status2 = model.Crypto.Crypto2Status2;
                        
                        result.Auth1Note = Sanitizer.Sanitize(model.Auth.Auth1Note);
                        result.Auth1Status = model.Auth.Auth1Status;
                        result.Auth2Note = Sanitizer.Sanitize(model.Auth.Auth2Note);
                        result.Auth2Status = model.Auth.Auth2Status;
                        result.Auth2Note1 = Sanitizer.Sanitize(model.Auth.Auth2Note1);
                        result.Auth2Status1 = model.Auth.Auth2Status1;
                        result.Auth2Note2 = Sanitizer.Sanitize(model.Auth.Auth2Note2);
                        result.Auth2Status2 = model.Auth.Auth2Status2;
                        result.Auth2Note3 = Sanitizer.Sanitize(model.Auth.Auth2Note3);
                        result.Auth2Status3 = model.Auth.Auth2Status3;
                        result.Auth3Note = Sanitizer.Sanitize(model.Auth.Auth3Note);
                        result.Auth3Status = model.Auth.Auth3Status;
                        
                        result.Network1Note = Sanitizer.Sanitize(model.Network.Network1Note);
                        result.Network1Status = model.Network.Network1Status;
                        result.Network1Note1 = Sanitizer.Sanitize(model.Network.Network1Note1);
                        result.Network1Status1 = model.Network.Network1Status1;
                        result.Network1Note2 = Sanitizer.Sanitize(model.Network.Network1Note2);
                        result.Network1Status2 = model.Network.Network1Status2;
                        result.Network1Note3 = Sanitizer.Sanitize(model.Network.Network1Note3);
                        result.Network1Status3 = model.Network.Network1Status3;
                        result.Network1Note4 = Sanitizer.Sanitize(model.Network.Network1Note4);
                        result.Network1Status4 = model.Network.Network1Status4;
                        result.Network1Note5 = Sanitizer.Sanitize(model.Network.Network1Note5);
                        result.Network1Status5 = model.Network.Network1Status5;
                        result.Network1Note6 = Sanitizer.Sanitize(model.Network.Network1Note6);
                        result.Network1Status6 = model.Network.Network1Status6;
                        result.Network1Note7 = Sanitizer.Sanitize(model.Network.Network1Note7);
                        result.Network1Status7 = model.Network.Network1Status7;
                        result.Network2Note = Sanitizer.Sanitize(model.Network.Network2Note);
                        result.Network2Status = model.Network.Network2Status;
                        result.Network2Note1 = Sanitizer.Sanitize(model.Network.Network2Note1);
                        result.Network2Status1 = model.Network.Network2Status1;
                        result.Network2Note2 = Sanitizer.Sanitize(model.Network.Network2Note2);
                        result.Network2Status2 = model.Network.Network2Status2;
                        
                        result.Platform1Note = Sanitizer.Sanitize(model.Platform.Platform1Note);
                        result.Platform1Status = model.Platform.Platform1Status;
                        result.Platform1Note1 = Sanitizer.Sanitize(model.Platform.Platform1Note1);
                        result.Platform1Status1 = model.Platform.Platform1Status1;
                        result.Platform1Note2 = Sanitizer.Sanitize(model.Platform.Platform1Note2);
                        result.Platform1Status2 = model.Platform.Platform1Status2;
                        result.Platform1Note3 = Sanitizer.Sanitize(model.Platform.Platform1Note3);
                        result.Platform1Status3 = model.Platform.Platform1Status3;
                        result.Platform1Note4 = Sanitizer.Sanitize(model.Platform.Platform1Note4);
                        result.Platform1Status4 = model.Platform.Platform1Status4;
                        result.Platform1Note5 = Sanitizer.Sanitize(model.Platform.Platform1Note5);
                        result.Platform1Status5 = model.Platform.Platform1Status5;
                        result.Platform1Note6 = Sanitizer.Sanitize(model.Platform.Platform1Note6);
                        result.Platform1Status6 = model.Platform.Platform1Status6;
                        result.Platform1Note7 = Sanitizer.Sanitize(model.Platform.Platform1Note7);
                        result.Platform1Status7 = model.Platform.Platform1Status7;
                        result.Platform1Note8 = Sanitizer.Sanitize(model.Platform.Platform1Note8);
                        result.Platform1Status8 = model.Platform.Platform1Status8;
                        result.Platform1Note9 = Sanitizer.Sanitize(model.Platform.Platform1Note9);
                        result.Platform1Status9 = model.Platform.Platform1Status9;
                        result.Platform1Note10 = Sanitizer.Sanitize(model.Platform.Platform1Note10);
                        result.Platform1Status10 = model.Platform.Platform1Status10;
                        result.Platform1Note11 = Sanitizer.Sanitize(model.Platform.Platform1Note11);
                        result.Platform1Status11 = model.Platform.Platform1Status11;
                        result.Platform1Note12 = Sanitizer.Sanitize(model.Platform.Platform1Note12);
                        result.Platform1Status12 = model.Platform.Platform1Status12;
                        result.Platform1Note13 = Sanitizer.Sanitize(model.Platform.Platform1Note13);
                        result.Platform1Status13 = model.Platform.Platform1Status13;
                        result.Platform2Note = Sanitizer.Sanitize(model.Platform.Platform2Note);
                        result.Platform2Status = model.Platform.Platform2Status;
                        result.Platform2Note1 = Sanitizer.Sanitize(model.Platform.Platform2Note1);
                        result.Platform2Status1 = model.Platform.Platform2Status1;
                        result.Platform2Note2 = Sanitizer.Sanitize(model.Platform.Platform2Note2);
                        result.Platform2Status2 = model.Platform.Platform2Status2;
                        result.Platform2Note3 = Sanitizer.Sanitize(model.Platform.Platform2Note3);
                        result.Platform2Status3 = model.Platform.Platform2Status3;
                        result.Platform2Note4 = Sanitizer.Sanitize(model.Platform.Platform2Note4);
                        result.Platform2Status4 = model.Platform.Platform2Status4;
                        result.Platform2Note5 = Sanitizer.Sanitize(model.Platform.Platform2Note5);
                        result.Platform2Status5 = model.Platform.Platform2Status5;
                        result.Platform2Note6 = Sanitizer.Sanitize(model.Platform.Platform2Note6);
                        result.Platform2Status6 = model.Platform.Platform2Status6;
                        result.Platform2Note7 = Sanitizer.Sanitize(model.Platform.Platform2Note7);
                        result.Platform2Status7 = model.Platform.Platform2Status7;
                        result.Platform3Note = Sanitizer.Sanitize(model.Platform.Platform3Note);
                        result.Platform3Status = model.Platform.Platform3Status;
                        result.Platform3Note1 = Sanitizer.Sanitize(model.Platform.Platform3Note1);
                        result.Platform3Status1 = model.Platform.Platform3Status1;
                        result.Platform3Note2 = Sanitizer.Sanitize(model.Platform.Platform3Note2);
                        result.Platform3Status2 = model.Platform.Platform3Status2;
                        result.Platform3Note3 = Sanitizer.Sanitize(model.Platform.Platform3Note3);
                        result.Platform3Status3 = model.Platform.Platform3Status3;
                        result.Platform3Note4 = Sanitizer.Sanitize(model.Platform.Platform3Note4);
                        result.Platform3Status4 = model.Platform.Platform3Status4;
                        result.Platform3Note5 = Sanitizer.Sanitize(model.Platform.Platform3Note5);
                        result.Platform3Status5 = model.Platform.Platform3Status5;

                        result.Code1Note = Sanitizer.Sanitize(model.Code.Code1Note);
                        result.Code1Status = model.Code.Code1Status;
                        result.Code2Note = Sanitizer.Sanitize(model.Code.Code2Note);
                        result.Code2Status = model.Code.Code2Status;
                        result.Code2Note1 = Sanitizer.Sanitize(model.Code.Code2Note1);
                        result.Code2Status1 = model.Code.Code2Status1;
                        result.Code2Note2 = Sanitizer.Sanitize(model.Code.Code2Note2);
                        result.Code2Status2 = model.Code.Code2Status2;
                        result.Code3Note = Sanitizer.Sanitize(model.Code.Code3Note);
                        result.Code3Status = model.Code.Code3Status;
                        result.Code3Note1 = Sanitizer.Sanitize(model.Code.Code3Note1);
                        result.Code3Status1 = model.Code.Code3Status1;
                        result.Code3Note2 = Sanitizer.Sanitize(model.Code.Code3Note2);
                        result.Code3Status2 = model.Code.Code3Status2;
                        result.Code4Note = Sanitizer.Sanitize(model.Code.Code4Note);
                        result.Code4Status = model.Code.Code4Status;
                        result.Code4Note1 = Sanitizer.Sanitize(model.Code.Code4Note1);
                        result.Code4Status1 = model.Code.Code4Status1;
                        result.Code4Note2 = Sanitizer.Sanitize(model.Code.Code4Note2);
                        result.Code4Status2 = model.Code.Code4Status2;
                        result.Code4Note3 = Sanitizer.Sanitize(model.Code.Code4Note3);
                        result.Code4Status3 = model.Code.Code4Status3;
                        result.Code4Note4 = Sanitizer.Sanitize(model.Code.Code4Note4);
                        result.Code4Status4 = model.Code.Code4Status4;
                        result.Code4Note5 = Sanitizer.Sanitize(model.Code.Code4Note5);
                        result.Code4Status5 = model.Code.Code4Status5;
                        result.Code4Note6 = Sanitizer.Sanitize(model.Code.Code4Note6);
                        result.Code4Status6 = model.Code.Code4Status6;
                        result.Code4Note7 = Sanitizer.Sanitize(model.Code.Code4Note7);
                        result.Code4Status7 = model.Code.Code4Status7;
                        result.Code4Note8 = Sanitizer.Sanitize(model.Code.Code4Note8);
                        result.Code4Status8 = model.Code.Code4Status8;
                        result.Code4Note9 = Sanitizer.Sanitize(model.Code.Code4Note9);
                        result.Code4Status9 = model.Code.Code4Status9;
                        result.Code4Note10 = Sanitizer.Sanitize(model.Code.Code4Note10);
                        result.Code4Status10 = model.Code.Code4Status10;

                        result.Resilience1Note = Sanitizer.Sanitize(model.Resilience.Resilience1Note);
                        result.Resilience1Status = model.Resilience.Resilience1Status;
                        result.Resilience1Note1 = Sanitizer.Sanitize(model.Resilience.Resilience1Note1);
                        result.Resilience1Status1 = model.Resilience.Resilience1Status1;
                        result.Resilience1Note2 = Sanitizer.Sanitize(model.Resilience.Resilience1Note2);
                        result.Resilience1Status2 = model.Resilience.Resilience1Status2;
                        result.Resilience1Note3 = Sanitizer.Sanitize(model.Resilience.Resilience1Note3);
                        result.Resilience1Status3 = model.Resilience.Resilience1Status3;
                        result.Resilience1Note4 = Sanitizer.Sanitize(model.Resilience.Resilience1Note4);
                        result.Resilience1Status4 = model.Resilience.Resilience1Status4;
                        result.Resilience2Status = model.Resilience.Resilience2Status;
                        result.Resilience2Note = Sanitizer.Sanitize(model.Resilience.Resilience2Note);
                        result.Resilience2Status1 = model.Resilience.Resilience2Status1;
                        result.Resilience2Note1 = Sanitizer.Sanitize(model.Resilience.Resilience2Note1);
                        result.Resilience2Status2 = model.Resilience.Resilience2Status2;
                        result.Resilience2Note2 = Sanitizer.Sanitize(model.Resilience.Resilience2Note2);
                        result.Resilience2Status3 = model.Resilience.Resilience2Status3;
                        result.Resilience2Note3 = Sanitizer.Sanitize(model.Resilience.Resilience2Note3);
                        result.Resilience2Status4 = model.Resilience.Resilience2Status4;
                        result.Resilience2Note4 = Sanitizer.Sanitize(model.Resilience.Resilience2Note4);
                        result.Resilience2Status5 = model.Resilience.Resilience2Status5;
                        result.Resilience2Note5 = Sanitizer.Sanitize(model.Resilience.Resilience2Note5);
                        result.Resilience3Note = Sanitizer.Sanitize(model.Resilience.Resilience3Note);
                        result.Resilience3Status = model.Resilience.Resilience3Status;
                        result.Resilience3Note1 = Sanitizer.Sanitize(model.Resilience.Resilience3Note1);
                        result.Resilience3Status1 = model.Resilience.Resilience3Status1;
                        result.Resilience3Note2 = Sanitizer.Sanitize(model.Resilience.Resilience3Note2);
                        result.Resilience3Status2 = model.Resilience.Resilience3Status2;
                        result.Resilience3Note3 = Sanitizer.Sanitize(model.Resilience.Resilience3Note3);
                        result.Resilience3Status3 = model.Resilience.Resilience3Status3;
                        result.Resilience3Note4 = Sanitizer.Sanitize(model.Resilience.Resilience3Note4);
                        result.Resilience3Status4 = model.Resilience.Resilience3Status4;
                        result.Resilience3Note5 = Sanitizer.Sanitize(model.Resilience.Resilience3Note5);
                        result.Resilience3Status5 = model.Resilience.Resilience3Status5;
                        result.Resilience3Note6 = Sanitizer.Sanitize(model.Resilience.Resilience3Note6);
                        result.Resilience3Status6 = model.Resilience.Resilience3Status6;
                        result.Resilience4Note = Sanitizer.Sanitize(model.Resilience.Resilience4Note);
                        result.Resilience4Status = model.Resilience.Resilience4Status;
                        result.Resilience4Status1 = model.Resilience.Resilience4Status1;
                        result.Resilience4Note1 = Sanitizer.Sanitize(model.Resilience.Resilience4Note1);
                        result.Resilience4Status2 = model.Resilience.Resilience4Status2;
                        result.Resilience4Note2 = Sanitizer.Sanitize(model.Resilience.Resilience4Note2);
                        result.Resilience4Status3 = model.Resilience.Resilience4Status3;
                        result.Resilience4Note3 = Sanitizer.Sanitize(model.Resilience.Resilience4Note3);
                        result.Resilience4Status4 = model.Resilience.Resilience4Status4;
                        result.Resilience4Note4 = Sanitizer.Sanitize(model.Resilience.Resilience4Note4);
                        result.Resilience4Status5 = model.Resilience.Resilience4Status5;
                        result.Resilience4Note5 = Sanitizer.Sanitize(model.Resilience.Resilience4Note5);
                        result.Resilience4Status6 = model.Resilience.Resilience4Status6;
                        result.Resilience4Note6 = Sanitizer.Sanitize(model.Resilience.Resilience4Note6);

                        await mastgManager.Context.SaveChangesAsync();
                   _logger.LogInformation("User: {0} edited MASTG Checklist: {1}", aspNetUserId, result.Id);
                   return NoContent();
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
    [HasPermission(Permissions.ReportsAdd)]
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
               
               Report rep = new Report
               {
                   Id = Guid.NewGuid(),
                   Name = Sanitizer.Sanitize(model.Name),
                   ProjectId = model.ProjectId,
                   UserId = aspNetUserId,
                   CreatedDate = DateTime.Now.ToUniversalTime(),
                   Description = Sanitizer.Sanitize(model.Description),
                   Version = Sanitizer.Sanitize(model.Version),
                   Language = model.Language,
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
                
                var scriptObject = new ScriptObject();
                scriptObject.Add("OrganizationName", org.Name);
                scriptObject.Add("OrganizationEmail", org.ContactEmail);
                scriptObject.Add("OrganizationPhone", org.ContactPhone);
                scriptObject.Add("OrganizationDescription", org.Description);
                scriptObject.Add("OrganizationContactName", org.ContactName);
                scriptObject.Add("OrganizationUrl", org.Url);
                scriptObject.Add("ClientName", client.Name);
                scriptObject.Add("ClientDescription", client.Description);
                scriptObject.Add("ClientUrl", client.Url);
                scriptObject.Add("ClientContactName", client.ContactName);
                scriptObject.Add("ClientEmail", client.ContactEmail);
                scriptObject.Add("ClientPhone", client.ContactPhone);
                scriptObject.Add("Year", DateTime.Now.Year.ToString());
                scriptObject.Add("TargetName", target.Name);
                scriptObject.Add("TargetDescription", target.Description);
                scriptObject.Add("TargetType", target.Type.ToString());
                scriptObject.Add("PageBreak", @"<span style=""page-break-after: always;""></span>");
                scriptObject.Add("Today", DateTime.Now.ToShortDateString());
                scriptObject.Add("ProjectName", pro.Name);
                scriptObject.Add("ProjectDescription", pro.Description);
                var properties = mastg.GetType().GetProperties();
                foreach (var property in properties)
                {
                    var value = property.GetValue(mastg);
                    scriptObject.Add(property.Name, value);
                }
                
                    
                var context = new TemplateContext();
                context.PushGlobal(scriptObject);

                var templateScriban = Template.Parse(source);
                // Check for any errors
                if (templateScriban.HasErrors)
                {
                    foreach(var error in templateScriban.Messages)
                    {
                        Console.WriteLine(error);
                    }
                }
                
                var result = await templateScriban.RenderAsync(context);

                rep.HtmlCode = result;
                
                reportManager.Add(rep);
                reportManager.Context.SaveChanges();
                _logger.LogInformation("Report MASVS generated successfully. User: {0}",
                    aspNetUserId);
                return CreatedAtAction(nameof(GetReportById), new { reportId = rep.Id }, rep.Id);
               
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
      [HasPermission(Permissions.ReportsAdd)]
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

                Report rep = new Report
                {
                    Id = Guid.NewGuid(),
                    Name = Sanitizer.Sanitize(model.Name),
                    ProjectId = model.ProjectId,
                    UserId = aspNetUserId,
                    CreatedDate = DateTime.Now.ToUniversalTime(),
                    Description = Sanitizer.Sanitize(model.Description),
                    Version = Sanitizer.Sanitize(model.Version),
                    Language = model.Language,
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
                

                var scriptObject = new ScriptObject();
                scriptObject.Add("OrganizationName", Organization.Name);
                scriptObject.Add("OrganizationEmail", Organization.ContactEmail);
                scriptObject.Add("OrganizationPhone", Organization.ContactPhone);
                scriptObject.Add("OrganizationDescription", Organization.Description);
                scriptObject.Add("OrganizationContactName", Organization.ContactName);
                scriptObject.Add("OrganizationUrl", Organization.Url);
                scriptObject.Add("ClientName", Client.Name);
                scriptObject.Add("ClientDescription", Client.Description);
                scriptObject.Add("ClientUrl", Client.Url);
                scriptObject.Add("ClientContactName", Client.ContactName);
                scriptObject.Add("ClientEmail", Client.ContactEmail);
                scriptObject.Add("ClientPhone", Client.ContactPhone);
                scriptObject.Add("Year", DateTime.Now.Year.ToString());
                //scriptObject.Add("CreatedDate", DateTime.Now.ToUniversalTime().ToShortDateString());
                scriptObject.Add("TargetName", wstg.Target.Name);
                scriptObject.Add("TargetDescription", wstg.Target.Description);
                scriptObject.Add("TargetType", wstg.Target.Type.ToString());
                scriptObject.Add("PageBreak", @"<span style=""page-break-after: always;""></span>");
                scriptObject.Add("Today", DateTime.Now.ToShortDateString());
                scriptObject.Add("ProjectName", pro.Name);
                scriptObject.Add("ProjectDescription", pro.Description);
   
                var properties = wstg.GetType().GetProperties();
                foreach (var property in properties)
                {
                    var value = property.GetValue(wstg);
                    scriptObject.Add(property.Name, value);
                }
                
                    
                var context = new TemplateContext();
                context.PushGlobal(scriptObject);

                var templateScriban = Template.Parse(source);
                // Check for any errors
                if (templateScriban.HasErrors)
                {
                    foreach(var error in templateScriban.Messages)
                    {
                        Console.WriteLine(error);
                    }
                }
                
                var result = await templateScriban.RenderAsync(context);

                rep.HtmlCode = result;
                
                reportManager.Add(rep);
                reportManager.Context.SaveChanges();

                _logger.LogInformation("Report generated successfully. User: {0}",
                    aspNetUserId);
                return CreatedAtAction(nameof(GetReportById), new { reportId = rep.Id }, rep);
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

    [NonAction]
    public IActionResult GetReportById(Guid reportId)
    {
        try{
            var report = reportManager.GetById(reportId);
            if (report == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(report);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred generating report. User: {0}",
                aspNetUserId);
            throw;
        }
    }
}