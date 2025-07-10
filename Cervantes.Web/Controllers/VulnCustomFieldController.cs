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
[Route("api/Vuln/CustomField")]
[Authorize]
public class VulnCustomFieldController : ControllerBase
{
    private IVulnCustomFieldManager customFieldManager = null;
    private IVulnCustomFieldValueManager customFieldValueManager = null;
    private IHttpContextAccessor HttpContextAccessor;
    private string aspNetUserId;
    private readonly ILogger<VulnCustomFieldController> _logger = null;
    private Sanitizer sanitizer;

    public VulnCustomFieldController(
        IVulnCustomFieldManager customFieldManager, 
        IVulnCustomFieldValueManager customFieldValueManager,
        ILogger<VulnCustomFieldController> logger,
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
    [HasPermission(Permissions.VulnCustomFieldsRead)]
    public IEnumerable<VulnCustomField> Get()
    {
        try
        {
            IEnumerable<VulnCustomField> model = customFieldManager.GetAll().ToArray();
            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred getting custom fields. User: {0}",
                aspNetUserId);
            throw;
        }
    }

    [HttpGet("active")]
    [HasPermission(Permissions.VulnCustomFieldsRead)]
    public IEnumerable<VulnCustomField> GetActive()
    {
        try
        {
            IEnumerable<VulnCustomField> model = customFieldManager.GetAll().Where(x => x.IsActive).ToArray();
            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred getting active custom fields. User: {0}",
                aspNetUserId);
            throw;
        }
    }

    [HttpGet("{id}")]
    [HasPermission(Permissions.VulnCustomFieldsRead)]
    public VulnCustomField GetById(Guid id)
    {
        try
        {
            return customFieldManager.GetById(id);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred getting custom field. User: {0}",
                aspNetUserId);
            throw;
        }
    }

    [HttpPost]
    [HasPermission(Permissions.VulnCustomFieldsAdd)]
    public async Task<IActionResult> Add([FromBody] VulnCustomFieldCreateViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var customField = new VulnCustomField();
                customField.Name = sanitizer.Sanitize(model.Name);
                customField.Label = sanitizer.Sanitize(model.Label);
                customField.Type = model.Type;
                customField.IsRequired = model.IsRequired;
                customField.IsUnique = model.IsUnique;
                customField.IsSearchable = model.IsSearchable;
                customField.IsVisible = model.IsVisible;
                customField.Order = model.Order;
                customField.Options = sanitizer.Sanitize(model.Options);
                customField.DefaultValue = sanitizer.Sanitize(model.DefaultValue);
                customField.Description = sanitizer.Sanitize(model.Description);
                customField.UserId = aspNetUserId;
                customField.CreatedDate = DateTime.Now.ToUniversalTime();
                customField.ModifiedDate = DateTime.Now.ToUniversalTime();
                customField.IsActive = true;

                await customFieldManager.AddAsync(customField);
                await customFieldManager.Context.SaveChangesAsync();
                _logger.LogInformation("Custom field created successfully. User: {0}",
                    aspNetUserId);
                return CreatedAtAction(nameof(GetById), new { id = customField.Id }, customField);
            }

            _logger.LogError("An error ocurred adding custom field. User: {0}",
                aspNetUserId);
            return BadRequest("Invalid request");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred adding custom field. User: {0}",
                aspNetUserId);
            throw;
        }
    }

    [HttpPut]
    [HasPermission(Permissions.VulnCustomFieldsEdit)]
    public async Task<IActionResult> Edit([FromBody] VulnCustomFieldEditViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var customField = customFieldManager.GetById(model.Id);
                if (customField != null)
                {
                    customField.Name = sanitizer.Sanitize(model.Name);
                    customField.Label = sanitizer.Sanitize(model.Label);
                    customField.Type = model.Type;
                    customField.IsRequired = model.IsRequired;
                    customField.IsUnique = model.IsUnique;
                    customField.IsSearchable = model.IsSearchable;
                    customField.IsVisible = model.IsVisible;
                    customField.Order = model.Order;
                    customField.Options = sanitizer.Sanitize(model.Options);
                    customField.DefaultValue = sanitizer.Sanitize(model.DefaultValue);
                    customField.Description = sanitizer.Sanitize(model.Description);
                    customField.IsActive = model.IsActive;
                    customField.ModifiedDate = DateTime.Now.ToUniversalTime();
                }
                else
                {
                    return NotFound();
                }

                await customFieldManager.Context.SaveChangesAsync();
                _logger.LogInformation("Custom field edited successfully. User: {0}",
                    aspNetUserId);
                return NoContent();
            }

            _logger.LogError("An error ocurred editing custom field. User: {0}",
                aspNetUserId);
            return BadRequest("Invalid request");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred editing custom field. User: {0}",
                aspNetUserId);
            throw;
        }
    }

    [HttpDelete]
    [Route("{id}")]
    [HasPermission(Permissions.VulnCustomFieldsDelete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var customField = customFieldManager.GetById(id);
                if (customField != null)
                {
                    customFieldManager.Remove(customField);
                }
                else
                {
                    return NotFound();
                }

                await customFieldManager.Context.SaveChangesAsync();
                _logger.LogInformation("Custom field deleted successfully. User: {0}",
                    aspNetUserId);
                return NoContent();
            }

            _logger.LogError("An error ocurred deleting custom field. User: {0}",
                aspNetUserId);
            return BadRequest("Invalid request");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred deleting custom field. User: {0}",
                aspNetUserId);
            throw;
        }
    }

    [HttpGet("values/{vulnId}")]
    [HasPermission(Permissions.VulnCustomFieldsRead)]
    public IEnumerable<VulnCustomFieldValue> GetValues(Guid vulnId)
    {
        try
        {
            IEnumerable<VulnCustomFieldValue> model = customFieldValueManager.GetAll()
                .Where(x => x.VulnId == vulnId).Include(x => x.VulnCustomField).ToArray();
            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred getting custom field values. User: {0}",
                aspNetUserId);
            throw;
        }
    }

    [HttpPost("values/{vulnId}")]
    [HasPermission(Permissions.VulnCustomFieldsEdit)]
    public async Task<IActionResult> SetValues(Guid vulnId, [FromBody] Dictionary<Guid, string> values)
    {
        try
        {
            if (ModelState.IsValid)
            {
                // Remove existing values
                var existingValues = customFieldValueManager.GetAll().Where(x => x.VulnId == vulnId);
                foreach (var existingValue in existingValues)
                {
                    customFieldValueManager.Remove(existingValue);
                }
                await customFieldValueManager.Context.SaveChangesAsync();

                // Add new values
                foreach (var kvp in values)
                {
                    var customFieldValue = new VulnCustomFieldValue
                    {
                        Id = Guid.NewGuid(),
                        VulnId = vulnId,
                        VulnCustomFieldId = kvp.Key,
                        Value = sanitizer.Sanitize(kvp.Value),
                        CreatedDate = DateTime.UtcNow,
                        ModifiedDate = DateTime.UtcNow,
                        UserId = aspNetUserId
                    };
                    await customFieldValueManager.AddAsync(customFieldValue);
                }
                await customFieldValueManager.Context.SaveChangesAsync();

                _logger.LogInformation("Custom field values set successfully. User: {0}",
                    aspNetUserId);
                return NoContent();
            }

            _logger.LogError("An error ocurred setting custom field values. User: {0}",
                aspNetUserId);
            return BadRequest("Invalid request");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred setting custom field values. User: {0}",
                aspNetUserId);
            throw;
        }
    }

    [HttpGet("check-name")]
    [HasPermission(Permissions.VulnCustomFieldsRead)]
    public bool CheckNameAvailability([FromQuery] string name, [FromQuery] Guid? excludeId = null)
    {
        try
        {
            var query = customFieldManager.GetAll().Where(cf => cf.Name == name);
            if (excludeId.HasValue)
            {
                query = query.Where(cf => cf.Id != excludeId.Value);
            }
            return !query.Any();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred checking name availability. User: {0}",
                aspNetUserId);
            throw;
        }
    }

}