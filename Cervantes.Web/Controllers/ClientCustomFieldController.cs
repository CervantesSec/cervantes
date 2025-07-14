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
[Route("api/Client/CustomField")]
[Authorize]
public class ClientCustomFieldController : ControllerBase
{
    private IClientCustomFieldManager customFieldManager = null;
    private IClientCustomFieldValueManager customFieldValueManager = null;
    private IHttpContextAccessor HttpContextAccessor;
    private string aspNetUserId;
    private readonly ILogger<ClientCustomFieldController> _logger = null;
    private Sanitizer sanitizer;

    public ClientCustomFieldController(
        IClientCustomFieldManager customFieldManager, 
        IClientCustomFieldValueManager customFieldValueManager,
        ILogger<ClientCustomFieldController> logger,
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
    [HasPermission(Permissions.ClientCustomFieldsRead)]
    public IEnumerable<ClientCustomField> Get()
    {
        try
        {
            var customFields = customFieldManager.GetAll().ToList();
            return customFields;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving client custom fields");
            return Enumerable.Empty<ClientCustomField>();
        }
    }

    [HttpGet("active")]
    [HasPermission(Permissions.ClientCustomFieldsRead)]
    public IEnumerable<ClientCustomField> GetActive()
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
            _logger.LogError(ex, "An error occurred while retrieving active client custom fields");
            return Enumerable.Empty<ClientCustomField>();
        }
    }

    [HttpGet("{id}")]
    [HasPermission(Permissions.ClientCustomFieldsRead)]
    public ClientCustomField Get(Guid id)
    {
        try
        {
            var customField = customFieldManager.GetById(id);
            return customField;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving client custom field {Id}", id);
            return null;
        }
    }

    [HttpPost]
    [HasPermission(Permissions.ClientCustomFieldsAdd)]
    public async Task<IActionResult> Post([FromBody] ClientCustomFieldCreateViewModel model)
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

            var customField = new ClientCustomField
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
            _logger.LogError(ex, "An error occurred while creating client custom field");
            return BadRequest(new { message = "An error occurred while creating client custom field" });
        }
    }

    [HttpPut]
    [HasPermission(Permissions.ClientCustomFieldsEdit)]
    public async Task<IActionResult> Put([FromBody] ClientCustomFieldEditViewModel model)
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
            _logger.LogError(ex, "An error occurred while updating client custom field");
            return BadRequest(new { message = "An error occurred while updating client custom field" });
        }
    }

    [HttpDelete("{id}")]
    [HasPermission(Permissions.ClientCustomFieldsDelete)]
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
            _logger.LogError(ex, "An error occurred while deleting client custom field");
            return BadRequest(new { message = "An error occurred while deleting client custom field" });
        }
    }

    [HttpGet("values/{clientId}")]
    [HasPermission(Permissions.ClientCustomFieldsRead)]
    public IEnumerable<ClientCustomFieldValue> GetValues(Guid clientId)
    {
        try
        {
            var values = customFieldValueManager.GetAll()
                .Where(cfv => cfv.ClientId == clientId)
                .Include(cfv => cfv.ClientCustomField)
                .ToList();
            return values;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving client custom field values for client {ClientId}", clientId);
            return Enumerable.Empty<ClientCustomFieldValue>();
        }
    }

    [HttpPost("values/{clientId}")]
    [HasPermission(Permissions.ClientCustomFieldsEdit)]
    public async Task<IActionResult> SetValues(Guid clientId, [FromBody] Dictionary<Guid, string> customFieldValues)
    {
        try
        {
            // Remove existing values
            var existingValues = customFieldValueManager.GetAll()
                .Where(cfv => cfv.ClientId == clientId)
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
                    var customFieldValue = new ClientCustomFieldValue
                    {
                        ClientId = clientId,
                        ClientCustomFieldId = kvp.Key,
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
            _logger.LogError(ex, "An error occurred while setting client custom field values for client {ClientId}", clientId);
            return BadRequest(new { message = "An error occurred while setting client custom field values" });
        }
    }

    [HttpGet("check-name")]
    [HasPermission(Permissions.ClientCustomFieldsRead)]
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
            _logger.LogError(ex, "An error occurred while checking client custom field name");
            return BadRequest(new { message = "An error occurred while checking client custom field name" });
        }
    }
}