using System.Globalization;
using System.Security.Claims;
using System.Web;
using Cervantes.Contracts;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.CORE.ViewModels;
using Cervantes.IFR.File;
using Cervantes.IFR.Parsers.Nmap;
using CsvHelper;
using CsvHelper.Configuration;
using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeDetective;

namespace Cervantes.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,SuperUser,User")]
public class TargetController : ControllerBase
{
    private readonly ILogger<TargetController> _logger = null;
    private ITargetManager targetManager = null;
    private ITargetServicesManager targetServicesManager = null;
    private IProjectManager projectManager = null;
    private IProjectUserManager projectUserManager = null;
    private IProjectAttachmentManager projectAttachmentManager = null;
    private INmapParser nmapParser = null;
    private readonly IWebHostEnvironment env;
    private IHttpContextAccessor HttpContextAccessor;
    private string aspNetUserId;
    private IFileCheck fileCheck;

    /// <summary>
    /// Target Controller Constructor
    /// </summary>
    /// <param name="targetManager">TargetManager</param>
    /// <param name="targetServicesManager">TargetServiceManager</param>
    /// <param name="projectManager">ProjectManager</param>
    public TargetController(ITargetManager targetManager, ITargetServicesManager targetServicesManager,
        IProjectManager projectManager, IProjectUserManager projectUserManager, ILogger<TargetController> logger,
        IProjectAttachmentManager projectAttachmentManager, INmapParser nmapParser, IWebHostEnvironment env,
        IHttpContextAccessor HttpContextAccessor, IFileCheck fileCheck)
    {
        this.targetManager = targetManager;
        this.targetServicesManager = targetServicesManager;
        this.projectManager = projectManager;
        this.projectUserManager = projectUserManager;
        _logger = logger;
        this.projectAttachmentManager = projectAttachmentManager;
        this.nmapParser = nmapParser;
        this.env = env;
        aspNetUserId = HttpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        this.fileCheck = fileCheck;
    }

    [HttpGet]
    public IEnumerable<CORE.Entities.Target> GetTargets()
    {
        try
        {
            IEnumerable<CORE.Entities.Target> model = targetManager.GetAll().ToArray();
            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred getting targets. User: {0}",
                aspNetUserId);
            throw;
        }
    }

    [HttpGet]
    [Route("Project/{id}")]
    public IEnumerable<CORE.Entities.Target> GetByProjectId(Guid id)
    {
        try
        {
            IEnumerable<CORE.Entities.Target> model = targetManager.GetAll().Where(x => x.ProjectId == id)
                .Include(X => X.User).ToArray();
            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred getting project targets. User: {0}",
                aspNetUserId);
            throw;
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] TargetCreateViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var user = projectUserManager.VerifyUser(model.ProjectId.Value, aspNetUserId);
                if (user == null)
                {
                    return BadRequest();
                }

                var sanitizer = new HtmlSanitizer();
                sanitizer.AllowedSchemes.Add("data");

                var target = new CORE.Entities.Target();
                target.Id = Guid.NewGuid();
                target.Name = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Name));
                target.Description = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Description));
                target.ProjectId = model.ProjectId;
                target.UserId = aspNetUserId;

                await targetManager.AddAsync(target);
                await targetManager.Context.SaveChangesAsync();
                _logger.LogInformation("Target added successfully. User: {0}",
                    aspNetUserId);
                return Ok();
            }

            _logger.LogError("An error ocurred adding a Target. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred adding a Target. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
    }

    [HttpPut]
    public async Task<IActionResult> Edit([FromBody] TargetEditViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = targetManager.GetById(model.Id);
                if (result != null)
                {
                    var user = projectUserManager.VerifyUser(model.ProjectId.Value, aspNetUserId);
                    if (user == null)
                    {
                        return BadRequest();
                    }

                    var sanitizer = new HtmlSanitizer();
                    sanitizer.AllowedSchemes.Add("data");

                    result.Name = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Name));
                    result.Description = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Description));
                    result.Type = model.Type;
                    await targetManager.Context.SaveChangesAsync();
                    _logger.LogInformation("Target added successfully. User: {0}",
                        aspNetUserId);
                    return Ok();
                }

                _logger.LogError("An error ocurred adding a Target. User: {0}",
                    aspNetUserId);
                return BadRequest();
            }

            _logger.LogError("An error ocurred adding a Target. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred adding a Target. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
    }

    /*[HttpPost]
    [Route("Delete")]
    public async Task<IActionResult> Delete([FromBody] Target model)
    {
        if (ModelState.IsValid)
        {
            targetManager.Remove(model);
            targetManager.Context.SaveChanges();
            return Ok();
        }
        return BadRequest();
    }*/

    [HttpDelete]
    [Route("{targetId}")]
    public async Task<IActionResult> Delete(Guid targetId)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = targetManager.GetById(targetId);
                if (result != null)
                {
                    var user = projectUserManager.VerifyUser(result.ProjectId.Value, aspNetUserId);
                    if (user == null)
                    {
                        return BadRequest();
                    }

                    targetManager.Remove(result);
                    await targetManager.Context.SaveChangesAsync();
                    _logger.LogInformation("Target deleted successfully. User: {0}",
                        aspNetUserId);
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                _logger.LogError("An error occurred deleting a Target. User: {0}",
                    aspNetUserId);
                return BadRequest();
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred deleting a Target. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
    }

    [HttpPost]
    [Route("Import")]
    public async Task<IActionResult> Import([FromBody] TargetImportViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var user = projectUserManager.VerifyUser(model.Project, aspNetUserId);
                if (user == null)
                {
                    return BadRequest();
                }
                var path = "";
                var unique = "";
                var file = "";
                if (model.FileContent != null)
                {
                    switch (model.Type)
                    {
                        #region Nmap

                        case TargetImportType.Nmap:

                            if (fileCheck.CheckFile(model.FileContent))
                            {
                                unique = Guid.NewGuid().ToString();
                                path = $"{env.WebRootPath}/Attachments/Projects/{model.Project}";
                                file = $"{env.WebRootPath}/Attachments/Projects/{model.Project}/{unique}";

                                if (Directory.Exists(path))
                                {
                                    var fs = System.IO.File.Create(file);
                                    fs.Write(model.FileContent, 0,
                                        model.FileContent.Length);
                                    fs.Close();
                                }
                                else
                                {
                                    Directory.CreateDirectory(path);
                                    var fs = System.IO.File.Create(file);
                                    fs.Write(model.FileContent, 0,
                                        model.FileContent.Length);
                                    fs.Close();
                                }

                                var attachment = new ProjectAttachment
                                {
                                    Name = "Nmap Scan Upload",
                                    ProjectId = model.Project,
                                    UserId = aspNetUserId,
                                    FilePath = "Attachments/Projects/" + model.Project + "/" + unique
                                };

                                await projectAttachmentManager.AddAsync(attachment);
                                await projectAttachmentManager.Context.SaveChangesAsync();

                                nmapParser.Parse(model.Project, aspNetUserId,
                                    file);
                                return Ok();
                            }
                            else
                            {
                                _logger.LogError("An error ocurred importing a nmap filetype not admitted. User: {0}",
                                    aspNetUserId);
                                return BadRequest("Invalid file type");
                            }

                        #endregion

                        #region CSV

                        case TargetImportType.CSV:
                            if (fileCheck.CheckFile(model.FileContent))
                            {
                                unique = Guid.NewGuid().ToString();
                                path = $"{env.WebRootPath}/Attachments/Projects/{model.Project}";
                                file = $"{env.WebRootPath}/Attachments/Projects/{model.Project}/{unique}";

                                if (Directory.Exists(path))
                                {
                                    var fs = System.IO.File.Create(file);
                                    fs.Write(model.FileContent, 0,
                                        model.FileContent.Length);
                                    fs.Close();
                                }
                                else
                                {
                                    Directory.CreateDirectory(path);
                                    var fs = System.IO.File.Create(file);
                                    fs.Write(model.FileContent, 0,
                                        model.FileContent.Length);
                                    fs.Close();
                                }

                                var attachment2 = new ProjectAttachment
                                {
                                    Name = "CSV Upload",
                                    ProjectId = model.Project,
                                    UserId = aspNetUserId,
                                    FilePath = "Attachments/Projects/" + model.Project + "/" + unique
                                };

                                await projectAttachmentManager.AddAsync(attachment2);
                                await projectAttachmentManager.Context.SaveChangesAsync();

                                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                                {
                                    Delimiter = ";"
                                };
                                using (var reader = new StreamReader(file))
                                using (var csv = new CsvReader(reader, config))
                                {
                                    var records = csv.GetRecords<TargetImportCSV>();

                                    foreach (var tar in records)
                                    {
                                        Target target = new Target();
                                        target.Name = tar.Name;
                                        target.ProjectId = model.Project;
                                        target.UserId = aspNetUserId;
                                        target.Description = tar.Description;
                                        target.Type = tar.Type;

                                        await targetManager.AddAsync(target);
                                        await targetManager.Context.SaveChangesAsync();
                                    }
                                }

                                _logger.LogInformation("Targets imported successfully. User: {0}",
                                    aspNetUserId);
                                return Ok();
                            }
                            else
                            {
                                _logger.LogError("An error ocurred importing a csv filetype not admitted. User: {0}",
                                    aspNetUserId);
                                return BadRequest("Invalid file type");
                            }

                        #endregion
                    }
                }

                _logger.LogError("An error ocurred importing a csv filetype not admitted. User: {0}",
                    aspNetUserId);
                return BadRequest();
            }

            _logger.LogError("An error ocurred importing a csv filetype not admitted. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError("An error ocurred importing targets. User: {0}",
                aspNetUserId);
            throw;
        }
    }

    [HttpGet]
    [Route("Services/{targetId}")]
    public IEnumerable<CORE.Entities.TargetServices> GetServices(Guid targetId)
    {
        try
        {
            IEnumerable<CORE.Entities.TargetServices> model = targetServicesManager.GetAll()
                .Where(x => x.TargetId == targetId)
                .Include(X => X.User).ToArray();
            return model;
        }
        catch (Exception e)
        {
            _logger.LogError("An error ocurred geeting target services. User: {0}",
                aspNetUserId);
            throw;
        }
    }

    [HttpPost]
    [Route("Service")]
    public async Task<IActionResult> AddService([FromBody] TargetServiceCreateViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var sanitizer = new HtmlSanitizer();
                sanitizer.AllowedSchemes.Add("data");

                var service = new CORE.Entities.TargetServices();
                service.Name = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Name));
                service.Description = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Description));
                service.Port = model.Port;
                service.Version = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Version));
                service.TargetId = model.TargetId;
                service.UserId = aspNetUserId;
                service.Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Note));
                await targetServicesManager.AddAsync(service);
                await targetServicesManager.Context.SaveChangesAsync();
                _logger.LogInformation("Target added successfully. User: {0}",
                    aspNetUserId);
                return Ok();
            }

            _logger.LogError("An error ocurred adding a Target. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred adding a Target. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
    }

    [HttpPut]
    [Route("Service")]
    public async Task<IActionResult> EditService([FromBody] TargetServiceEditViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var sanitizer = new HtmlSanitizer();
                sanitizer.AllowedSchemes.Add("data");

                var service = targetServicesManager.GetById(model.Id);
                if (service != null)
                {
                    service.Name = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Name));
                    service.Description = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Description));
                    service.Port = model.Port;
                    service.Version = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Version));
                    service.TargetId = model.TargetId;
                    service.UserId = aspNetUserId;
                    service.Note = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Note));
                    await targetServicesManager.Context.SaveChangesAsync();
                    _logger.LogInformation("Target Service edited successfully. User: {0}",
                        aspNetUserId);
                    return Ok();
                }

                _logger.LogError("An error ocurred editing a Target Service. User: {0}",
                    aspNetUserId);
                return BadRequest();
            }

            _logger.LogError("An error ocurred editing a Target Service. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred editing a Target Service. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
    }

    [HttpDelete]
    [Route("Service/{serviceId}")]
    public async Task<IActionResult> DeleteService(Guid serviceId)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = targetServicesManager.GetById(serviceId);
                if (result != null)
                {
                    targetServicesManager.Remove(result);
                    await targetManager.Context.SaveChangesAsync();
                    _logger.LogInformation("Target Service deleted successfully. User: {0}",
                        aspNetUserId);
                    return Ok();
                }

                _logger.LogError("An error occurred deleting a Target Service. User: {0}",
                    aspNetUserId);
                return BadRequest();
            }
            else
            {
                _logger.LogError("An error occurred deleting a Target Service. User: {0}",
                    aspNetUserId);
                return BadRequest();
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred deleting a Target Service. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
    }
}