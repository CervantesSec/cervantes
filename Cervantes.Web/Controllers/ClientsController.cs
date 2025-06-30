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

namespace Cervantes.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientsController : ControllerBase
{
    private IClientManager clientManager = null;
    private IProjectManager projectManager = null;
    private IUserManager userManager = null;
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
        IFileCheck fileCheck, Sanitizer sanitizer)
    {
        this.clientManager = clientManager;
        this.projectManager = projectManager;
        this.userManager = userManager;
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
                _logger.LogInformation("Client added successfully. User: {0}",aspNetUserId);
                return CreatedAtAction(nameof(GetById), new { clientId = client.Id }, client);
            }
            _logger.LogError("An error ocurred adding a client. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e,"An error ocurred adding a client. User: {0}",
                aspNetUserId);
            return BadRequest();
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
                    _logger.LogError("An error ocurred deleting client. User: {0}",
                        aspNetUserId);
                    return BadRequest();
                }

            }
            _logger.LogError("An error ocurred deleting client. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e,"An error ocurred deleting client. User: {0}",
                aspNetUserId);
            return BadRequest();
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
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred deleting client logo. User: {0}",
                aspNetUserId);
            return BadRequest();
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
                    return BadRequest();
                }
            }
            _logger.LogError("An error ocurred editing a Client. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred editing a Client. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
    }
}