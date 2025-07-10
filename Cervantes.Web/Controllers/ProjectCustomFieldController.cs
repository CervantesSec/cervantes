using Cervantes.Contracts;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthPermissions.AspNetCore;
using Cervantes.CORE;
using Cervantes.Web.Helpers;

namespace Cervantes.Web.Controllers;

[ApiController]
[Route("api/Project/CustomField")]
[Authorize]
public class ProjectCustomFieldController : ControllerBase
{
    private IProjectCustomFieldManager customFieldManager = null;
    private IProjectCustomFieldValueManager customFieldValueManager = null;
    private IHttpContextAccessor HttpContextAccessor;
    private string aspNetUserId;
    private readonly ILogger<ProjectCustomFieldController> _logger = null;
    private Sanitizer sanitizer;

    public ProjectCustomFieldController(
        IProjectCustomFieldManager customFieldManager, 
        IProjectCustomFieldValueManager customFieldValueManager,
        ILogger<ProjectCustomFieldController> logger,
        IHttpContextAccessor HttpContextAccessor,
        Sanitizer sanitizer)
    {
        this.customFieldManager = customFieldManager;
        this.customFieldValueManager = customFieldValueManager;
        this._logger = logger;
        this.HttpContextAccessor = HttpContextAccessor;
        aspNetUserId = HttpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        this.sanitizer = sanitizer;
    }

    [HttpGet]
    [HasPermission(Permissions.ProjectCustomFieldsRead)]
    public IEnumerable<ProjectCustomField> Get()
    {
        try
        {
            var customFields = customFieldManager.GetAll().ToList();
            return customFields;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving custom fields");
            return Enumerable.Empty<ProjectCustomField>();
        }
    }

    [HttpGet("active")]
    [HasPermission(Permissions.ProjectCustomFieldsRead)]
    public IEnumerable<ProjectCustomField> GetActive()
    {
        try
        {
            var customFields = customFieldManager.GetAll()
                .Where(cf => cf.IsActive)
                .OrderBy(cf => cf.Order)
                .ToList();
            return customFields;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving active custom fields");
            return Enumerable.Empty<ProjectCustomField>();
        }
    }

    [HttpGet("{id}")]
    [HasPermission(Permissions.ProjectCustomFieldsRead)]
    public ProjectCustomField Get(Guid id)
    {
        try
        {
            var customField = customFieldManager.GetById(id);
            return customField;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving custom field {Id}", id);
            return null;
        }
    }

    [HttpPost]
    [HasPermission(Permissions.ProjectCustomFieldsAdd)]
    public async Task<IActionResult> Post([FromBody] ProjectCustomFieldCreateViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if name already exists
            var existingField = customFieldManager.GetAll()
                .FirstOrDefault(cf => cf.Name == model.Name);
            
            if (existingField != null)
            {
                return BadRequest(new { message = "A custom field with this name already exists." });
            }

            var customField = new ProjectCustomField
            {
                Name = sanitizer.Sanitize(model.Name),
                Label = sanitizer.Sanitize(model.Label),
                Type = model.Type,
                IsRequired = model.IsRequired,
                IsUnique = model.IsUnique,
                IsSearchable = model.IsSearchable,
                IsVisible = model.IsVisible,
                Order = model.Order,
                Options = model.Options ?? string.Empty,
                DefaultValue = model.DefaultValue ?? string.Empty,
                Description = model.Description ?? string.Empty,
                UserId = aspNetUserId,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };

            await customFieldManager.AddAsync(customField);
            await customFieldManager.Context.SaveChangesAsync();

            return Ok(customField);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating custom field");
            return BadRequest(new { message = "An error occurred while creating custom field" });
        }
    }

    [HttpPut]
    [HasPermission(Permissions.ProjectCustomFieldsEdit)]
    public async Task<IActionResult> Put([FromBody] ProjectCustomFieldEditViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customField = customFieldManager.GetById(model.Id);
            if (customField == null)
            {
                return NotFound();
            }

            // Check if name already exists (excluding current field)
            var existingField = customFieldManager.GetAll()
                .FirstOrDefault(cf => cf.Name == model.Name && cf.Id != model.Id);
            
            if (existingField != null)
            {
                return BadRequest(new { message = "A custom field with this name already exists." });
            }

            customField.Name = sanitizer.Sanitize(model.Name);
            customField.Label = sanitizer.Sanitize(model.Label);
            customField.Type = model.Type;
            customField.IsRequired = model.IsRequired;
            customField.IsUnique = model.IsUnique;
            customField.IsSearchable = model.IsSearchable;
            customField.IsVisible = model.IsVisible;
            customField.Order = model.Order;
            customField.Options = model.Options ?? string.Empty;
            customField.DefaultValue = model.DefaultValue ?? string.Empty;
            customField.Description = model.Description ?? string.Empty;
            customField.IsActive = model.IsActive;
            customField.ModifiedDate = DateTime.UtcNow;

            customFieldManager.Update(customField);
            await customFieldManager.Context.SaveChangesAsync();

            return Ok(customField);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating custom field");
            return BadRequest(new { message = "An error occurred while updating custom field" });
        }
    }

    [HttpDelete("{id}")]
    [HasPermission(Permissions.ProjectCustomFieldsDelete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var customField = customFieldManager.GetById(id);
            if (customField == null)
            {
                return NotFound();
            }

            customFieldManager.Remove(customField);
            await customFieldManager.Context.SaveChangesAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting custom field");
            return BadRequest(new { message = "An error occurred while deleting custom field" });
        }
    }

    [HttpGet("values/{projectId}")]
    [HasPermission(Permissions.ProjectCustomFieldsRead)]
    public IEnumerable<ProjectCustomFieldValue> GetValues(Guid projectId)
    {
        try
        {
            var values = customFieldValueManager.GetAll()
                .Where(cfv => cfv.ProjectId == projectId)
                .Include(cfv => cfv.ProjectCustomField)
                .ToList();
            return values;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving custom field values for project {ProjectId}", projectId);
            return Enumerable.Empty<ProjectCustomFieldValue>();
        }
    }

    [HttpPost("values/{projectId}")]
    [HasPermission(Permissions.ProjectCustomFieldsEdit)]
    public async Task<IActionResult> SetValues(Guid projectId, [FromBody] Dictionary<Guid, string> customFieldValues)
    {
        try
        {
            // Remove existing values
            var existingValues = customFieldValueManager.GetAll()
                .Where(cfv => cfv.ProjectId == projectId)
                .ToList();
            
            foreach (var existingValue in existingValues)
            {
                customFieldValueManager.Remove(existingValue);
            }

            // Add new values
            foreach (var kvp in customFieldValues)
            {
                if (!string.IsNullOrEmpty(kvp.Value))
                {
                    var customFieldValue = new ProjectCustomFieldValue
                    {
                        ProjectId = projectId,
                        ProjectCustomFieldId = kvp.Key,
                        Value = sanitizer.Sanitize(kvp.Value),
                        UserId = aspNetUserId,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now
                    };

                    await customFieldValueManager.AddAsync(customFieldValue);
                }
            }

            await customFieldValueManager.Context.SaveChangesAsync();
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while setting custom field values for project {ProjectId}", projectId);
            return BadRequest(new { message = "An error occurred while setting custom field values" });
        }
    }

    [HttpGet("check-name")]
    [HasPermission(Permissions.ProjectCustomFieldsRead)]
    public IActionResult CheckName(string name, Guid? excludeId = null)
    {
        try
        {
            var query = customFieldManager.GetAll().Where(cf => cf.Name == name);
            
            if (excludeId.HasValue)
            {
                query = query.Where(cf => cf.Id != excludeId.Value);
            }

            var exists = query.Any();
            return Ok(new { exists });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while checking custom field name");
            return BadRequest(new { message = "An error occurred while checking custom field name" });
        }
    }
}