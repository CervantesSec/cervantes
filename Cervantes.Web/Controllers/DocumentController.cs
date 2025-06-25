using System.Security.Claims;
using System.Web;
using AuthPermissions.AspNetCore;
using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.CORE.ViewModel;
using Cervantes.CORE.ViewModels;
using Cervantes.IFR.File;
using Cervantes.Web.Helpers;
using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cervantes.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DocumentController : Controller
{
    private IDocumentManager docManager = null;
    private readonly ILogger<DocumentController> _logger = null;
    private readonly IWebHostEnvironment env;
    private IHttpContextAccessor HttpContextAccessor;
    private string aspNetUserId;
    private IFileCheck fileCheck;
    private Sanitizer sanitizer;

    public DocumentController(IDocumentManager docManager,ILogger<DocumentController> logger, IWebHostEnvironment env
        ,IHttpContextAccessor HttpContextAccessor, IFileCheck fileCheck, Sanitizer sanitizer)
    {
        this.docManager = docManager;
        _logger = logger;
        this.env = env;
        aspNetUserId = HttpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        this.fileCheck = fileCheck;
        this.sanitizer = sanitizer;
    }
    
    [HttpGet]
    [HasPermission(Permissions.DocumentsRead)]
    public IEnumerable<CORE.Entities.Document> Get()
    {
        try
        {
            var model = docManager.GetAll().Include(x => x.User).ToArray();
        
            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e,"An error ocurred getting Documents. User: {0}",
                aspNetUserId);
            throw;
        }
            
    }
    
    [NonAction]
    public CORE.Entities.Document GetById(Guid documentId)
    {
        try
        {
            var model = docManager.GetById(documentId);
            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e,"An error ocurred getting Documents. User: {0}",
                aspNetUserId);
            throw;
        }
            
    }
    
     [HttpPost]
     [HasPermission(Permissions.DocumentsAdd)]
    public async Task<IActionResult> Add([FromBody] DocumentCreateViewModel model)
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
                        path = $"{env.WebRootPath}/Attachments/Documents/{unique}";
                        var fs = System.IO.File.Create(path);
                        fs.Write(model.FileContent, 0, 
                            model.FileContent.Length);
                        fs.Close();
                    }
                    else
                    {
                        _logger.LogError("An error ocurred adding a document filetype not admitted. User: {0}",
                            aspNetUserId);
                        return BadRequest("Invalid file type");
                    }
                    
                }
                
                var doc = new CORE.Entities.Document();
                doc.Name = sanitizer.Sanitize(model.Name);
                doc.Description = sanitizer.Sanitize(model.Description);
                doc.CreatedDate = DateTime.Now.ToUniversalTime();
                doc.UserId = aspNetUserId;
                doc.FilePath = "Attachments/Documents/"+unique;

                await docManager.AddAsync(doc);
                await docManager.Context.SaveChangesAsync();
                _logger.LogInformation("Document added successfully. User: {0}",aspNetUserId);
                return CreatedAtAction(nameof(GetById), new { documentId = doc.Id }, doc);
            }
            _logger.LogError("An error ocurred adding a Document. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError("An error ocurred adding a Document. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
    }
    
    [HttpDelete]
    [Route("{docId}")]
    [HasPermission(Permissions.DocumentsDelete)]
    public async Task<IActionResult> Delete(Guid docId)
    {
        try
        {
            if (ModelState.IsValid)
            {

                var doc = docManager.GetById(docId);
                if (doc != null)
                {
                    var filePath = $"{env.WebRootPath}/{doc.FilePath}";
                    System.IO.File.Delete(filePath);
                    docManager.Remove(doc);
                    docManager.Context.SaveChanges();
                    _logger.LogInformation("Document deleted successfully. User: {0}",
                        aspNetUserId);
                    return NoContent();
                }
                else
                {
                    _logger.LogError("An error ocurred deleting Document. User: {0}",
                        aspNetUserId);
                    return BadRequest();
                }

            }
            _logger.LogError("An error ocurred deleting Document. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError("An error ocurred deleting Document. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        
    }
    
    [HttpPut]
    [HasPermission(Permissions.DocumentsEdit)]
    public async Task<IActionResult> Edit([FromBody] DocumentEditViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {

                var doc = docManager.GetById(model.Id);
                if (doc != null)
                {
                    
                    doc.Name = sanitizer.Sanitize(model.Name);
                    doc.Description = sanitizer.Sanitize(model.Description);
                    
                    await docManager.Context.SaveChangesAsync();
                    _logger.LogInformation("Document edited successfully. User: {0}",
                        aspNetUserId);
                    return NoContent();
                }
                else
                {
                    _logger.LogError("An error ocurred editing a Document. User: {0}",
                        aspNetUserId);
                    return BadRequest();
                }
            }
            _logger.LogError("An error ocurred editing a Document. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred editing a Document. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
    }
    
}