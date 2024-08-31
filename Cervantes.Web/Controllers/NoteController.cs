using System.Security.Claims;
using System.Web;
using Cervantes.Contracts;
using Cervantes.CORE.ViewModel;
using Cervantes.Web.Helpers;
using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cervantes.Web.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize (Roles = "Admin,SuperUser,User")]
public class NoteController : Controller
{
    private INoteManager noteManager = null;
    private readonly ILogger<NoteController> _logger = null;
    private readonly IWebHostEnvironment env;
    private IHttpContextAccessor HttpContextAccessor;
    private string aspNetUserId;
    private Sanitizer sanitizer;

    public NoteController(INoteManager noteManager,ILogger<NoteController> logger, IWebHostEnvironment env,
        IHttpContextAccessor HttpContextAccessor,Sanitizer sanitizer)
    {
        this.noteManager = noteManager;
        _logger = logger;
        this.env = env;
        aspNetUserId = HttpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        this.sanitizer = sanitizer;
    }
    
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] NoteCreateViewModel model)
    {
        try{
            if (ModelState.IsValid)
            {
                
                var note = new CORE.Entities.Note();
                note.Name = sanitizer.Sanitize(model.Name);
                note.Description = sanitizer.Sanitize(model.Description);
                note.CreatedDate = DateTime.Now.ToUniversalTime();
                note.UserId = aspNetUserId;
                
                await noteManager.AddAsync(note);
                await noteManager.Context.SaveChangesAsync();
                _logger.LogInformation("Note added successfully. User: {0}",aspNetUserId);
                return Ok();
            }
            _logger.LogError("An error ocurred adding a Note. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e,"An error ocurred adding a Note. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
    }
    
    [HttpPut]
    public async Task<IActionResult> Edit([FromBody] NoteEditViewModel model)
    {
        try{
            if (ModelState.IsValid)
            {
                var note = noteManager.GetById(model.Id);
                if (note != null)
                {
                    
                    note.Name = sanitizer.Sanitize(model.Name);
                    note.Description = sanitizer.Sanitize(model.Description);
                    await noteManager.Context.SaveChangesAsync();
                    _logger.LogInformation("Note edited successfully. User: {0}",aspNetUserId);
                    return Ok();
                }
                _logger.LogError("An error ocurred editing a Note. User: {0}",
                    aspNetUserId);
                return BadRequest();
            }
            
            _logger.LogError("An error ocurred editing a Note. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e,"An error editing adding a Note. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
    }
    
    [HttpDelete]
    [Route("{noteId}")]
    public async Task<IActionResult> Delete(Guid noteId)
    {
        try{
            if (ModelState.IsValid)
            {
                var note = noteManager.GetById(noteId);
                if (note != null)
                {
                    if (note.UserId == aspNetUserId)
                    {
                        noteManager.Remove(note);
                        await noteManager.Context.SaveChangesAsync();
                        _logger.LogInformation("Note deleted successfully. User: {0}",aspNetUserId);
                        return Ok();
                    }
                    return BadRequest();

                }
                _logger.LogError("An error ocurred deleting a Note. User: {0}",
                    aspNetUserId);
                return BadRequest();
            }
            
            _logger.LogError("An error ocurred deleting a Note. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e,"An error deleting adding a Note. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
    }

    
    [HttpGet]
    public IEnumerable<CORE.Entities.Note> GetByUserId()
    {
        try
        {
            var model = noteManager.GetAll().Where(x => x.UserId == aspNetUserId).ToArray();
        
            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e,"An error deleting getting notes. User: {0}",
                aspNetUserId);
            throw;
        }
        
    }
}