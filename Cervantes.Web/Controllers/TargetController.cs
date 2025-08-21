using System.Globalization;
using System.Security.Claims;
using System.Web;
using AuthPermissions.AspNetCore;
using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.CORE.ViewModels;
using Cervantes.IFR.File;
using Cervantes.IFR.Parsers.Nmap;
using Cervantes.Web.Helpers;
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
[Authorize]
public class TargetController : ControllerBase
{
    private readonly ILogger<TargetController> _logger = null;
    private ITargetManager targetManager = null;
    private ITargetServicesManager targetServicesManager = null;
    private IProjectManager projectManager = null;
    private IProjectUserManager projectUserManager = null;
    private IProjectAttachmentManager projectAttachmentManager = null;
    private INmapParser nmapParser = null;
    private ITargetCustomFieldManager targetCustomFieldManager = null;
    private ITargetCustomFieldValueManager targetCustomFieldValueManager = null;
    private readonly IWebHostEnvironment env;
    private IHttpContextAccessor HttpContextAccessor;
    private string aspNetUserId;
    private IFileCheck fileCheck;
    private Sanitizer sanitizer;

    /// <summary>
    /// Target Controller Constructor
    /// </summary>
    /// <param name="targetManager">TargetManager</param>
    /// <param name="targetServicesManager">TargetServiceManager</param>
    /// <param name="projectManager">ProjectManager</param>
    public TargetController(ITargetManager targetManager, ITargetServicesManager targetServicesManager,
        IProjectManager projectManager, IProjectUserManager projectUserManager, ILogger<TargetController> logger,
        IProjectAttachmentManager projectAttachmentManager, INmapParser nmapParser, IWebHostEnvironment env,
        IHttpContextAccessor HttpContextAccessor, IFileCheck fileCheck, Sanitizer sanitizer,
        ITargetCustomFieldManager targetCustomFieldManager, ITargetCustomFieldValueManager targetCustomFieldValueManager)
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
        this.sanitizer = sanitizer;
        this.targetCustomFieldManager = targetCustomFieldManager;
        this.targetCustomFieldValueManager = targetCustomFieldValueManager;
    }

    [HttpGet]
    [HasPermission(Permissions.TargetsRead)]
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
    [HasPermission(Permissions.TargetsRead)]
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
    
    [NonAction]
    public CORE.Entities.Target GetTargetById(Guid targetId)
    {
        try
        {
            return targetManager.GetById(targetId);
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
    [HasPermission(Permissions.TargetsAdd)]
    public async Task<IActionResult> Add([FromBody] TargetCreateViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var user = projectUserManager.VerifyUser(model.ProjectId.Value, aspNetUserId);
                if (user == null)
                {
                    return BadRequest("Access denied: User does not have permission to access this project");
                }
                
                var target = new CORE.Entities.Target();
                target.Id = Guid.NewGuid();
                target.Name = sanitizer.Sanitize(model.Name);
                target.Description = sanitizer.Sanitize(model.Description);
                target.Type = model.Type;
                target.ProjectId = model.ProjectId;
                target.UserId = aspNetUserId;

                await targetManager.AddAsync(target);
                await targetManager.Context.SaveChangesAsync();
                
                // Add custom field values
                if (model.CustomFieldValues != null && model.CustomFieldValues.Any())
                {
                    foreach (var kvp in model.CustomFieldValues)
                    {
                        if (!string.IsNullOrEmpty(kvp.Value))
                        {
                            var customFieldValue = new CORE.Entities.TargetCustomFieldValue
                            {
                                TargetId = target.Id,
                                TargetCustomFieldId = kvp.Key,
                                Value = sanitizer.Sanitize(kvp.Value),
                                UserId = aspNetUserId,
                                CreatedDate = DateTime.UtcNow,
                                ModifiedDate = DateTime.UtcNow
                            };
                            
                            await targetCustomFieldValueManager.AddAsync(customFieldValue);
                        }
                    }
                    await targetCustomFieldValueManager.Context.SaveChangesAsync();
                }
                
                _logger.LogInformation("Target added successfully. User: {0}",
                    aspNetUserId);
                return CreatedAtAction(nameof(GetTargetById),  new { targetId = target.Id }, target);
            }

            _logger.LogError("Validation failed when adding a Target. User: {0}",
                aspNetUserId);
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(
                    x => x.Key,
                    x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );
            return BadRequest(new { message = "Validation failed", errors });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred adding a Target. User: {0}",
                aspNetUserId);
            return StatusCode(500, "An error occurred while creating the target. Please try again later.");
        }
    }

    [HttpPut]
    [HasPermission(Permissions.TargetsEdit)]
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
                        return BadRequest("Access denied: User does not have permission to access this project");
                    }

                    result.Name = sanitizer.Sanitize(model.Name);
                    result.Description = sanitizer.Sanitize(model.Description);
                    result.Type = model.Type;
                    await targetManager.Context.SaveChangesAsync();
                    
                    // Update custom field values
                    if (model.CustomFieldValues != null && model.CustomFieldValues.Any())
                    {
                        // Remove existing custom field values
                        var existingValues = targetCustomFieldValueManager.GetAll()
                            .Where(v => v.TargetId == result.Id)
                            .ToList();
                        
                        foreach (var existingValue in existingValues)
                        {
                            targetCustomFieldValueManager.Remove(existingValue);
                        }
                        
                        // Add new custom field values
                        foreach (var kvp in model.CustomFieldValues)
                        {
                            if (!string.IsNullOrEmpty(kvp.Value))
                            {
                                var customFieldValue = new CORE.Entities.TargetCustomFieldValue
                                {
                                    TargetId = result.Id,
                                    TargetCustomFieldId = kvp.Key,
                                    Value = sanitizer.Sanitize(kvp.Value),
                                    UserId = aspNetUserId,
                                    CreatedDate = DateTime.UtcNow,
                                    ModifiedDate = DateTime.UtcNow
                                };
                                
                                await targetCustomFieldValueManager.AddAsync(customFieldValue);
                            }
                        }
                        await targetCustomFieldValueManager.Context.SaveChangesAsync();
                    }
                    
                    _logger.LogInformation("Target updated successfully. User: {0}",
                        aspNetUserId);
                    return NoContent();
                }

                _logger.LogError("An error ocurred adding a Target. User: {0}",
                    aspNetUserId);
                return NotFound();
            }

            _logger.LogError("Validation failed when editing a Target. User: {0}",
                aspNetUserId);
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(
                    x => x.Key,
                    x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );
            return BadRequest(new { message = "Validation failed", errors });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred editing a Target. User: {0}",
                aspNetUserId);
            return StatusCode(500, "An error occurred while updating the target. Please try again later.");
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
        return BadRequest("Invalid request");
    }*/

    [HttpDelete]
    [Route("{targetId}")]
    [HasPermission(Permissions.TargetsDelete)]
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
                        return BadRequest("Access denied: User does not have permission to access this project");
                    }

                    targetManager.Remove(result);
                    await targetManager.Context.SaveChangesAsync();
                    _logger.LogInformation("Target deleted successfully. User: {0}",
                        aspNetUserId);
                    return NoContent();
                }
                else
                {
                    return NotFound($"Target with ID {targetId} not found");
                }
            }
            else
            {
                _logger.LogError("Validation failed when deleting a Target. User: {0}",
                    aspNetUserId);
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                return BadRequest(new { message = "Validation failed", errors });
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred deleting a Target. User: {0}",
                aspNetUserId);
            return StatusCode(500, "An error occurred while deleting the target. Please try again later.");
        }
    }

    [HttpPost]
    [Route("Import")]
    [HasPermission(Permissions.TargetsImport)]
    public async Task<IActionResult> Import([FromBody] TargetImportViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var user = projectUserManager.VerifyUser(model.Project, aspNetUserId);
                if (user == null)
                {
                    return BadRequest("Access denied: User does not have permission to access this project");
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
                                unique = Guid.NewGuid().ToString()+"."+fileCheck.GetExtension(model.FileContent);
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
                                unique = Guid.NewGuid().ToString()+"."+fileCheck.GetExtension(model.FileContent);
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

                _logger.LogError("File content is required for import. User: {0}",
                    aspNetUserId);
                return BadRequest("File content is required");
            }

            _logger.LogError("Validation failed when importing targets. User: {0}",
                aspNetUserId);
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(
                    x => x.Key,
                    x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );
            return BadRequest(new { message = "Validation failed", errors });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred importing targets. User: {0}",
                aspNetUserId);
            return StatusCode(500, "An error occurred while importing targets. Please try again later.");
        }
    }

    [HttpGet]
    [Route("Services/{targetId}")]
    [HasPermission(Permissions.TargetsServicesRead)]
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

    [NonAction]
    public CORE.Entities.TargetServices GetServiceById(Guid serviceId)
    {
        try
        {

            return targetServicesManager.GetById(serviceId);
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
    [HasPermission(Permissions.TargetsServicesAdd)]
    public async Task<IActionResult> AddService([FromBody] TargetServiceCreateViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var service = new CORE.Entities.TargetServices();
                service.Name = sanitizer.Sanitize(model.Name);
                service.Description = sanitizer.Sanitize(model.Description);
                service.Port = model.Port;
                service.Version = sanitizer.Sanitize(model.Version);
                service.TargetId = model.TargetId;
                service.UserId = aspNetUserId;
                service.Note = sanitizer.Sanitize(model.Note);
                await targetServicesManager.AddAsync(service);
                await targetServicesManager.Context.SaveChangesAsync();
                _logger.LogInformation("Target added successfully. User: {0}",
                    aspNetUserId);
                return CreatedAtAction(nameof(GetServiceById), new { serviceId = service.Id }, service);
            }

            _logger.LogError("Validation failed when adding a Target Service. User: {0}",
                aspNetUserId);
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(
                    x => x.Key,
                    x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );
            return BadRequest(new { message = "Validation failed", errors });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred adding a Target Service. User: {0}",
                aspNetUserId);
            return StatusCode(500, "An error occurred while creating the target service. Please try again later.");
        }
    }

    [HttpPut]
    [Route("Service")]
    [HasPermission(Permissions.TargetsServicesEdit)]
    public async Task<IActionResult> EditService([FromBody] TargetServiceEditViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var service = targetServicesManager.GetById(model.Id);
                if (service != null)
                {
                    service.Name = sanitizer.Sanitize(model.Name);
                    service.Description = sanitizer.Sanitize(model.Description);
                    service.Port = model.Port;
                    service.Version = sanitizer.Sanitize(model.Version);
                    service.TargetId = model.TargetId;
                    service.UserId = aspNetUserId;
                    service.Note = sanitizer.Sanitize(model.Note);
                    await targetServicesManager.Context.SaveChangesAsync();
                    _logger.LogInformation("Target Service edited successfully. User: {0}",
                        aspNetUserId);
                    return NoContent();
                }

                _logger.LogError("An error ocurred editing a Target Service. User: {0}",
                    aspNetUserId);
                return NotFound();
            }

            _logger.LogError("Validation failed when editing a Target Service. User: {0}",
                aspNetUserId);
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(
                    x => x.Key,
                    x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );
            return BadRequest(new { message = "Validation failed", errors });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred editing a Target Service. User: {0}",
                aspNetUserId);
            return StatusCode(500, "An error occurred while updating the target service. Please try again later.");
        }
    }

    [HttpDelete]
    [Route("Service/{serviceId}")]
    [HasPermission(Permissions.TargetsServicesDelete)]
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
                    return NoContent();
                }

                _logger.LogError("An error occurred deleting a Target Service. User: {0}",
                    aspNetUserId);
                return NotFound();
            }
            else
            {
                _logger.LogError("Validation failed when deleting a Target Service. User: {0}",
                    aspNetUserId);
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                return BadRequest(new { message = "Validation failed", errors });
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred deleting a Target Service. User: {0}",
                aspNetUserId);
            return StatusCode(500, "An error occurred while deleting the target service. Please try again later.");
        }
    }
    
    /// <summary>
    /// Get Custom Field Values for a Target
    /// </summary>
    /// <param name="targetId">Target ID</param>
    /// <returns>List of custom field values</returns>
    [HttpGet("{targetId}/custom-fields")]
    [HasPermission(Permissions.TargetCustomFieldsRead)]
    public async Task<IActionResult> GetCustomFieldValues(Guid targetId)
    {
        try
        {
            var target = targetManager.GetById(targetId);
            if (target == null)
            {
                return NotFound();
            }
            
            var customFieldValues = targetCustomFieldValueManager.GetAll()
                .Where(v => v.TargetId == targetId)
                .Include(v => v.TargetCustomField)
                .ToList();
            
            return Ok(customFieldValues);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting custom field values for target {TargetId}. User: {UserId}", 
                targetId, aspNetUserId);
            return BadRequest("Error retrieving custom field values");
        }
    }
    
    /// <summary>
    /// Get Custom Field Values for a Target (Helper method for components)
    /// </summary>
    /// <param name="targetId">Target ID</param>
    /// <returns>List of custom field values</returns>
    [NonAction]
    public List<TargetCustomFieldValueViewModel> GetCustomFieldValuesByTargetId(Guid targetId)
    {
        var customFieldValues = targetCustomFieldValueManager.GetAll()
            .Where(v => v.TargetId == targetId)
            .Include(v => v.TargetCustomField)
            .ToList();

        return customFieldValues.Select(v => new TargetCustomFieldValueViewModel
        {
            Id = v.Id,
            TargetId = v.TargetId,
            TargetCustomFieldId = v.TargetCustomFieldId,
            Value = v.Value,
            Name = v.TargetCustomField.Name,
            Label = v.TargetCustomField.Label,
            Type = v.TargetCustomField.Type,
            IsRequired = v.TargetCustomField.IsRequired,
            IsUnique = v.TargetCustomField.IsUnique,
            IsSearchable = v.TargetCustomField.IsSearchable,
            IsVisible = v.TargetCustomField.IsVisible,
            Order = v.TargetCustomField.Order,
            Options = v.TargetCustomField.Options,
            DefaultValue = v.TargetCustomField.DefaultValue,
            Description = v.TargetCustomField.Description
        }).ToList();
    }
    
    /// <summary>
    /// Update Custom Field Values for a Target
    /// </summary>
    /// <param name="targetId">Target ID</param>
    /// <param name="customFieldData">Dictionary of custom field values</param>
    /// <returns>Success or error result</returns>
    [HttpPut("{targetId}/custom-fields")]
    [HasPermission(Permissions.TargetCustomFieldsEdit)]
    public async Task<IActionResult> UpdateCustomFieldValues(Guid targetId, [FromBody] Dictionary<Guid, string> customFieldData)
    {
        try
        {
            var target = targetManager.GetById(targetId);
            if (target == null)
            {
                return NotFound();
            }
            
            // Remove existing custom field values
            var existingValues = targetCustomFieldValueManager.GetAll()
                .Where(v => v.TargetId == targetId)
                .ToList();
            
            foreach (var existingValue in existingValues)
            {
                targetCustomFieldValueManager.Remove(existingValue);
            }
            
            // Add new custom field values
            foreach (var kvp in customFieldData)
            {
                if (!string.IsNullOrEmpty(kvp.Value))
                {
                    var newValue = new CORE.Entities.TargetCustomFieldValue
                    {
                        TargetId = targetId,
                        TargetCustomFieldId = kvp.Key,
                        Value = sanitizer.Sanitize(kvp.Value),
                        UserId = aspNetUserId,
                        CreatedDate = DateTime.UtcNow,
                        ModifiedDate = DateTime.UtcNow
                    };
                    
                    await targetCustomFieldValueManager.AddAsync(newValue);
                }
            }
            
            await targetCustomFieldValueManager.Context.SaveChangesAsync();
            _logger.LogInformation("Custom field values updated for target {TargetId}. User: {UserId}", 
                targetId, aspNetUserId);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating custom field values for target {TargetId}. User: {UserId}", 
                targetId, aspNetUserId);
            return BadRequest("Error updating custom field values");
        }
    }
}