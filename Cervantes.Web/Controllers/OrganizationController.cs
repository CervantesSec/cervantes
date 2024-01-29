using System.Security.Claims;
using System.Web;
using Cervantes.Contracts;
using Cervantes.CORE.ViewModel;
using Cervantes.IFR.File;
using Cervantes.Server.Helpers;
using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cervantes.Web.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,SuperUser")]

public class OrganizationController: ControllerBase
{
    private readonly ILogger<OrganizationController> _logger = null;
    private IOrganizationManager organizationManager = null;
    private readonly IWebHostEnvironment env;
    private IFileCheck fileCheck;
    private IHttpContextAccessor HttpContextAccessor;
    private string aspNetUserId;
    /// <summary>
    /// Organization Controller Constructor
    /// </summary>
    public OrganizationController(IOrganizationManager organizationManager,
        ILogger<OrganizationController> logger,IWebHostEnvironment env,IHttpContextAccessor HttpContextAccessor, IFileCheck fileCheck)
    {
        this.organizationManager = organizationManager;
        this.env = env;        
        _logger = logger;
        aspNetUserId = HttpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        this.fileCheck = fileCheck;

    }
    
    [HttpGet]
    public CORE.Entities.Organization Get()
    {
        try
        {
            CORE.Entities.Organization model = organizationManager.GetAll().First();

            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e,"An error ocurred getting organization. User: {0}",
                aspNetUserId);
            throw;
        }
        
    }
    
    [HttpPut]
    public async Task<IActionResult> Save([FromBody] OrganizationViewModel model)
    {
        try
        {
 if (ModelState.IsValid)
        {

            var result = organizationManager.GetAll().First();
            
            if (result != null)
            {
                var sanitizer = new HtmlSanitizer();
                sanitizer.AllowedSchemes.Add("data");
                
                var path = "";
                var unique = "";
                if (model.FileContent != null)
                {
                    if (fileCheck.CheckFile(model.FileContent))
                    {
                        unique = Guid.NewGuid().ToString()+"."+fileCheck.GetExtension(model.FileContent);
                        path = $"{env.WebRootPath}/Attachments/Organization/{unique}";
                        var fs = System.IO.File.Create(path);
                        fs.Write(model.FileContent, 0, 
                            model.FileContent.Length);
                        fs.Close();
                    }
                    else
                    {
                        _logger.LogError("An error ocurred saving a organization filetype not admitted. User: {0}",
                            aspNetUserId);
                        return BadRequest("Invalid file type");
                    }
                   
                    result.ImagePath = "Attachments/Organization/"+unique;
                }
                
                result.Name = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Name));
                result.Description = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Description));
                result.ContactEmail = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.ContactEmail));
                result.ContactName = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.ContactName));
                result.ContactPhone = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.ContactPhone));
                result.Url = model.Url;
               
            }
            else
            {
                _logger.LogError("An error ocurred saving organization. User: {0}",
                    aspNetUserId);
                return BadRequest();
            }
            _logger.LogInformation("Organization saved successfully. User: {0}",
                aspNetUserId);
            await organizationManager.Context.SaveChangesAsync();
            
            return Ok();
        }
        return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e,"An error ocurred saving organization. User: {0}",
                aspNetUserId);
            throw;
        }
       
    }
    
    [HttpDelete]
    [Route("Logo")]
    public async Task<IActionResult> DeleteAvatar()
    {
        try
        {
            var org = organizationManager.GetAll().First();
            var imagePath = $"{env.WebRootPath}/{org.ImagePath}";
            System.IO.File.Delete(imagePath);
            
            org.ImagePath = null;

            await organizationManager.Context.SaveChangesAsync();
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e,"An error deleting logo. User: {0}",
                aspNetUserId);
            throw;
        }
        
    }
    
}