using System.Security.Claims;
using Cervantes.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Imaging;
using System.Web;
using AuthPermissions.AspNetCore;
using Cervantes.CORE;
using Cervantes.CORE.ViewModel;
using Cervantes.IFR.File;
using Cervantes.Server.Helpers;
using Cervantes.Web.Helpers;
using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientsController : ControllerBase
{
    private IClientManager clientManager = null;
    private IProjectManager projectManager = null;
    private IUserManager userManager = null;
    private IClientCustomFieldManager clientCustomFieldManager = null;
    private IClientCustomFieldValueManager clientCustomFieldValueManager = null;
    private readonly IWebHostEnvironment env;
    private readonly ILogger<ClientsController> _logger = null;
    private IHttpContextAccessor HttpContextAccessor;
    private string aspNetUserId;
    private IFileCheck fileCheck;
    private Sanitizer sanitizer;

    /// <summary>
    /// Client Controller Constructor
    /// </summary>
    public ClientsController(IClientManager clientManager, IUserManager userManager, IProjectManager projectManager,
        IWebHostEnvironment env, ILogger<ClientsController> logger,IHttpContextAccessor HttpContextAccessor, 
        IFileCheck fileCheck, Sanitizer sanitizer, IClientCustomFieldManager clientCustomFieldManager,
        IClientCustomFieldValueManager clientCustomFieldValueManager)
    {
        this.clientManager = clientManager;
        this.projectManager = projectManager;
        this.userManager = userManager;
        this.clientCustomFieldManager = clientCustomFieldManager;
        this.clientCustomFieldValueManager = clientCustomFieldValueManager;
        this.env = env;
        _logger = logger;
        aspNetUserId = HttpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        this.fileCheck = fileCheck;
        this.sanitizer = sanitizer;

    }
    
    [HasPermission(Permissions.ClientsRead)]
    [HttpGet]
    public IEnumerable<CORE.Entities.Client> Get()
    {
        try
        {
            IEnumerable<CORE.Entities.Client> model = clientManager.GetAll().Include(x => x.User).ToArray();
            return model;
        }
        catch (Exception e)
        {
            _logger.LogError("An error ocurred getting clients. User: {0}",
                aspNetUserId);
            throw;
        }
        
    }
    
    [HasPermission(Permissions.ClientsRead)]
    [HttpGet]
    [Route("{clientId}")]
    public CORE.Entities.Client GetById(Guid clientId)
    {
        try
        {
            return clientManager.GetById(clientId);
        }
        catch (Exception e)
        {
            _logger.LogError(e,"An error ocurred getting a client. User: {0}",
                aspNetUserId);
            throw;
        }
        
    }
    
    [HasPermission(Permissions.ClientsRead)]
    [HttpGet]
    [Route("Name/{clientName}")]
    public IEnumerable<CORE.Entities.Client> GetByName(string clientName)
    {
        try
        {
            return clientManager.GetAll().Where(x => x.Name.Contains(clientName));
        }
        catch (Exception e)
        {
            _logger.LogError(e,"An error ocurred getting a client. User: {0}",
                aspNetUserId);
            throw;
        }
        
    }
    
    [HasPermission(Permissions.ClientsAdd)]
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] ClientCreateViewModel model)
    {
        try{
            if (ModelState.IsValid)
            {


                var path = "";
                var unique = "";
                if (model.FileContent != null)
                {
                    
                    if (fileCheck.CheckFile(model.FileContent))
                    {
                        
                        unique = Guid.NewGuid().ToString()+"."+fileCheck.GetExtension(model.FileContent);
                        path = $"{env.WebRootPath}/Attachments/Clients/{unique}";
                        var fs = System.IO.File.Create(path);
                        fs.Write(model.FileContent, 0, 
                            model.FileContent.Length);
                        fs.Close();
                    }
                    else
                    {
                        _logger.LogError("An error ocurred adding a client. User: {0}",
                            aspNetUserId);
                        return BadRequest("Invalid file type");
                    }
                
                }
                
            
                var client = new CORE.Entities.Client();
                client.Name = sanitizer.Sanitize(model.Name);
                client.Url = sanitizer.Sanitize(model.Url);
                client.Description = sanitizer.Sanitize(model.Description);
                client.ContactName = sanitizer.Sanitize(model.ContactName);
                client.ContactEmail = sanitizer.Sanitize(model.ContactEmail);
                client.ContactPhone = sanitizer.Sanitize(model.ContactPhone);
                client.CreatedDate = DateTime.Now.ToUniversalTime();
                client.UserId = aspNetUserId;
                if (model.FileContent != null)
                {
                    client.ImagePath = "Attachments/Clients/"+unique;

                }
                else
                {
                    client.ImagePath = "None";
                }

                await clientManager.AddAsync(client);
                await clientManager.Context.SaveChangesAsync();
                
                // Add custom field values
                if (model.CustomFieldValues != null && model.CustomFieldValues.Any())
                {
                    foreach (var kvp in model.CustomFieldValues)
                    {
                        if (!string.IsNullOrEmpty(kvp.Value))
                        {
                            var customFieldValue = new CORE.Entities.ClientCustomFieldValue
                            {
                                ClientId = client.Id,
                                ClientCustomFieldId = kvp.Key,
                                Value = sanitizer.Sanitize(kvp.Value),
                                UserId = aspNetUserId,
                                CreatedDate = DateTime.UtcNow,
                                ModifiedDate = DateTime.UtcNow
                            };
                            
                            await clientCustomFieldValueManager.AddAsync(customFieldValue);
                        }
                    }
                    await clientCustomFieldValueManager.Context.SaveChangesAsync();
                }
                
                _logger.LogInformation("Client added successfully. User: {0}",aspNetUserId);
                return CreatedAtAction(nameof(GetById), new { clientId = client.Id }, client);
            }
            _logger.LogError("Validation failed when adding a client. User: {0}",
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
            _logger.LogError(e,"An error occurred adding a client. User: {0}",
                aspNetUserId);
            return StatusCode(500, "An error occurred while creating the client. Please try again later.");
        }
    }
    
    [HasPermission(Permissions.ClientsDelete)]
    [HttpDelete]
    [Route("{clientId}")]
    public async Task<IActionResult> Delete(Guid clientId)
    {
        try
        {
            if (ModelState.IsValid)
            {

                var client = clientManager.GetById(clientId);
                if (client.Name != null)
                {
                    clientManager.Remove(client);
                    clientManager.Context.SaveChanges();
                    _logger.LogInformation("Client deleted successfully. User: {0}",
                        aspNetUserId);
                    return NoContent();
                }
                else
                {
                    _logger.LogError("Client not found for deletion. User: {0}",
                        aspNetUserId);
                    return NotFound($"Client with ID {clientId} not found");
                }

            }
            _logger.LogError("Validation failed when deleting client. User: {0}",
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
            _logger.LogError(e,"An error occurred deleting client. User: {0}",
                aspNetUserId);
            return StatusCode(500, "An error occurred while deleting the client. Please try again later.");
        }
        
    }
    
    [HasPermission(Permissions.ClientsEdit)]
    [HttpDelete]
    [Route("Avatar/{id}")]
    public async Task<IActionResult> DeleteAvatar(Guid id)
    {
        try
        {
            if (ModelState.IsValid)
            {

                var client = clientManager.GetById(id);
                var imagePath = $"{env.WebRootPath}/{client.ImagePath}";
                System.IO.File.Delete(imagePath);

                client.ImagePath = "None";

                clientManager.Context.SaveChanges();
                _logger.LogInformation("Client logo deleted successfully. User: {0}",
                    aspNetUserId);
                return NoContent();
            }
            _logger.LogError("An error ocurred deleting client logo. User: {0}",
                aspNetUserId);
            return BadRequest("Invalid request");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred deleting client logo. User: {0}",
                aspNetUserId);
            return BadRequest("Invalid request");
        }
    }
    
    [HasPermission(Permissions.ClientsEdit)]
    [HttpPut]
    public async Task<IActionResult> Edit([FromBody] ClientEditViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {

                var client = clientManager.GetById(model.Id);
                if (client != null)
                {
                    var path = "";
                    var unique = "";

                    if (client.ImagePath == "None" && model.FileContent != null)
                    {
                        
                        if (fileCheck.CheckFile(model.FileContent))
                        {
                            unique = Guid.NewGuid().ToString()+"."+fileCheck.GetExtension(model.FileContent);
                            path = $"{env.WebRootPath}/Attachments/Clients/{unique}";
                            var fs = System.IO.File.Create(path);
                            fs.Write(model.FileContent, 0, 
                                model.FileContent.Length);
                            fs.Close();
                        }
                        else
                        {
                            _logger.LogError("An error ocurred adding a client. User: {0}",
                                aspNetUserId);
                            return BadRequest("Invalid file type");
                        }

                        client.ImagePath = "Attachments/Clients/" + unique;
                    }
                    
                    client.Name = sanitizer.Sanitize(model.Name);
                    client.Url = sanitizer.Sanitize(model.Url);
                    client.Description = sanitizer.Sanitize(model.Description);
                    client.ContactName = sanitizer.Sanitize(model.ContactName);
                    client.ContactEmail = sanitizer.Sanitize(model.ContactEmail);
                    client.ContactPhone = sanitizer.Sanitize(model.ContactPhone);

                    await clientManager.Context.SaveChangesAsync();
                    _logger.LogInformation("Client edited successfully. User: {0}",
                        aspNetUserId);
                    return NoContent();
                }
                else
                {
                    _logger.LogError("An error ocurred editing a Client. User: {0}",
                        aspNetUserId);
                    return BadRequest("Invalid request");
                }
            }
            _logger.LogError("An error ocurred editing a Client. User: {0}",
                aspNetUserId);
            return BadRequest("Invalid request");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred editing a Client. User: {0}",
                aspNetUserId);
            return BadRequest("Invalid request");
        }
    }
    
    /// <summary>
    /// Get Custom Field Values for a Client
    /// </summary>
    /// <param name="clientId">Client ID</param>
    /// <returns>List of custom field values</returns>
    [HttpGet("{clientId}/custom-fields")]
    [HasPermission(Permissions.ClientCustomFieldsRead)]
    public async Task<IActionResult> GetCustomFieldValues(Guid clientId)
    {
        try
        {
            var client = clientManager.GetById(clientId);
            if (client == null)
            {
                return NotFound();
            }
            
            var customFieldValues = clientCustomFieldValueManager.GetAll()
                .Where(v => v.ClientId == clientId)
                .Include(v => v.ClientCustomField)
                .ToList();
            
            return Ok(customFieldValues);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting custom field values for client {ClientId}. User: {UserId}", 
                clientId, aspNetUserId);
            return BadRequest("Error retrieving custom field values");
        }
    }
    
    /// <summary>
    /// Update Custom Field Values for a Client
    /// </summary>
    /// <param name="clientId">Client ID</param>
    /// <param name="customFieldData">Dictionary of custom field values</param>
    /// <returns>Success or error result</returns>
    [HttpPut("{clientId}/custom-fields")]
    [HasPermission(Permissions.ClientCustomFieldsEdit)]
    public async Task<IActionResult> UpdateCustomFieldValues(Guid clientId, [FromBody] Dictionary<Guid, string> customFieldData)
    {
        try
        {
            var client = clientManager.GetById(clientId);
            if (client == null)
            {
                return NotFound();
            }
            
            // Remove existing custom field values
            var existingValues = clientCustomFieldValueManager.GetAll()
                .Where(v => v.ClientId == clientId)
                .ToList();
            
            foreach (var existingValue in existingValues)
            {
                clientCustomFieldValueManager.Remove(existingValue);
            }
            
            // Add new custom field values
            foreach (var kvp in customFieldData)
            {
                if (!string.IsNullOrEmpty(kvp.Value))
                {
                    var newValue = new CORE.Entities.ClientCustomFieldValue
                    {
                        ClientId = clientId,
                        ClientCustomFieldId = kvp.Key,
                        Value = sanitizer.Sanitize(kvp.Value),
                        UserId = aspNetUserId,
                        CreatedDate = DateTime.UtcNow,
                        ModifiedDate = DateTime.UtcNow
                    };
                    
                    await clientCustomFieldValueManager.AddAsync(newValue);
                }
            }
            
            await clientCustomFieldValueManager.Context.SaveChangesAsync();
            _logger.LogInformation("Custom field values updated for client {ClientId}. User: {UserId}", 
                clientId, aspNetUserId);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating custom field values for client {ClientId}. User: {UserId}", 
                clientId, aspNetUserId);
            return BadRequest("Error updating custom field values");
        }
    }
}