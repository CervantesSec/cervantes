using System.Security.Claims;
using System.Web;
using Cervantes.Contracts;
using Cervantes.CORE.ViewModel;
using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cervantes.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize (Roles = "Admin,SuperUser,User")]
public class KnowledgeBaseController : Controller
{
    private IKnowledgeBaseManager knowledgeBaseManager = null;
    private IKnowledgeBaseCategoryManager knowledgeBaseCategoryManager = null;
    private IKnowledgeBaseTagManager KnowledgeBaseTagManager = null;
    private readonly ILogger<KnowledgeBaseController> _logger = null;
    private IHttpContextAccessor HttpContextAccessor;
    private string aspNetUserId;
    
    public KnowledgeBaseController(IKnowledgeBaseManager knowledgeBaseManager, IKnowledgeBaseCategoryManager knowledgeBaseCategoryManager,
        IKnowledgeBaseTagManager KnowledgeBaseTagManager, ILogger<KnowledgeBaseController> logger,IHttpContextAccessor HttpContextAccessor)
    {
        this.knowledgeBaseManager = knowledgeBaseManager;
        this.knowledgeBaseCategoryManager = knowledgeBaseCategoryManager;
        this.KnowledgeBaseTagManager = KnowledgeBaseTagManager;
        _logger = logger;
        aspNetUserId = HttpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
    }
    [HttpGet]
    [Route("Page")]
    public IEnumerable<CORE.Entities.KnowledgeBase> GetPages()
    {
        IEnumerable<CORE.Entities.KnowledgeBase> model = knowledgeBaseManager.GetAll().
            Include(x => x.CreatedUser).Include(x => x.UpdatedUser).
            Include(x => x.Category).ToArray();
        return model;
    }
    
     [HttpPost]
     [Route("Page")]
    public async Task<IActionResult> AddPage([FromBody] KnowledgePageCreateViewModel model)
    {
        try{
            if (ModelState.IsValid)
            {
                var sanitizer = new HtmlSanitizer();
                sanitizer.AllowedSchemes.Add("data");
                
                var page = new CORE.Entities.KnowledgeBase();
                page.Id = Guid.NewGuid();
                page.Title = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Title));
                page.Content = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Content));
                page.CategoryId = model.CategoryId;
                page.Order = model.Order;
                page.CreatedAt = DateTime.Now.ToUniversalTime();
                page.UpdatedAt = DateTime.Now.ToUniversalTime();
                page.CreatedUserId = aspNetUserId;
                page.UpdatedUserId = aspNetUserId;


                await knowledgeBaseManager.AddAsync(page);
                await knowledgeBaseManager.Context.SaveChangesAsync();
                _logger.LogInformation("Knowledge Page added successfully. User: {0}",aspNetUserId);
                return Ok();
            }
            _logger.LogError("An error ocurred adding a Knowledge Page. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError("An error ocurred adding a Knowledge Page. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
    }
    
    [HttpPut]
    [Route("Page")]
    public async Task<IActionResult> EditPage([FromBody] KnowledgePageEditViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {

                var page = knowledgeBaseManager.GetById(model.Id);
                if (page != null)
                {
                    var sanitizer = new HtmlSanitizer();
                    sanitizer.AllowedSchemes.Add("data");
                    
                    page.Title = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Title));
                    page.Content = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Content));
                    page.CategoryId = model.CategoryId;
                    page.Order = model.Order;
                    page.UpdatedAt = DateTime.Now.ToUniversalTime();
                    page.UpdatedUserId = aspNetUserId;
                   
                    await knowledgeBaseManager.Context.SaveChangesAsync();
                    _logger.LogInformation("Knowledge Page edited successfully. User: {0}",
                        aspNetUserId);
                    return Ok();
                }
                else
                {
                    _logger.LogError("An error ocurred editing a Knowledge Page. User: {0}",
                        aspNetUserId);
                    return BadRequest();
                }
            }
            _logger.LogError("An error ocurred editing a Knowledge Page. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred editing a Knowledge Page. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
    }

    [HttpDelete]
    [Route("Page/{pageId}")]
    public async Task<IActionResult> DeletePage(Guid pageId)
    {
        try
        {
            if (ModelState.IsValid)
            {

                var page = knowledgeBaseManager.GetById(pageId);
                if (page != null)
                {
                    knowledgeBaseManager.Remove(page);
                    knowledgeBaseManager.Context.SaveChanges();
                    _logger.LogInformation("Knowledge Page deleted successfully. User: {0}",
                        aspNetUserId);
                    return Ok();
                }
                else
                {
                    _logger.LogError("An error ocurred deleting Knowledge Page. User: {0}",
                        aspNetUserId);
                    return BadRequest();
                }

            }

            _logger.LogError("An error ocurred deleting Knowledge Page. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError("An error ocurred deleting Knowledge Page. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
    }


    [HttpGet]
    [Route("Category")]
    public IEnumerable<CORE.Entities.KnowledgeBaseCategories> GetCategories()
    {
        try
        {
            IEnumerable<CORE.Entities.KnowledgeBaseCategories> model = knowledgeBaseCategoryManager.GetAll().Include(x => x.User).ToArray();
            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e,"An error ocurred deleting Knowledge Page. User: {0}",
                aspNetUserId);
            throw;
        }
        
    }
    
     [HttpPost]
     [Route("Category")]
    public async Task<IActionResult> AddCategory([FromBody] KnowledgeCategoryCreateVIewModel model)
    {
        try{
            if (ModelState.IsValid)
            {
                var sanitizer = new HtmlSanitizer();
                sanitizer.AllowedSchemes.Add("data");
                
                var cat = new CORE.Entities.KnowledgeBaseCategories();
                cat.Id = Guid.NewGuid();
                cat.Name = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Name));
                cat.Description = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Description));
                cat.Order = model.Order;
                cat.Icon = sanitizer.Sanitize(HttpUtility.HtmlDecode(string.Empty));
                cat.UserId = aspNetUserId;


                await knowledgeBaseCategoryManager.AddAsync(cat);
                await knowledgeBaseCategoryManager.Context.SaveChangesAsync();
                _logger.LogInformation("Knowledge Category added successfully. User: {0}",aspNetUserId);
                return Ok();
            }
            _logger.LogError("An error ocurred adding a Knowledge Category. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e,"An error ocurred adding a Knowledge Category. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
    }
    [HttpPut]
    [Route("Category")]
    public async Task<IActionResult> EditCategory([FromBody] KnowledgeCategoryEditViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {

                var category = knowledgeBaseCategoryManager.GetById(model.Id);
                if (category != null)
                {
                    
                    var sanitizer = new HtmlSanitizer();
                    sanitizer.AllowedSchemes.Add("data");
                    
                    category.Name = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Name));
                    category.Description = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Description));
                    category.Order = model.Order;
                    if (model.Icon == null)
                    {
                        category.Icon = sanitizer.Sanitize(HttpUtility.HtmlDecode(string.Empty));
                    }
                    else
                    {
                        category.Icon = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Icon));

                    }

                    await knowledgeBaseCategoryManager.Context.SaveChangesAsync();
                    _logger.LogInformation("Knowledge Category edited successfully. User: {0}",
                        aspNetUserId);
                    return Ok();
                }
                else
                {
                    _logger.LogError("An error ocurred editing a Knowledge Category. User: {0}",
                        aspNetUserId);
                    return BadRequest();
                }
            }
            _logger.LogError("An error ocurred editing a Knowledge Category. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred editing a Knowledge Category. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
    }
    
    [HttpDelete]
    [Route("Category/{categoryId}")]
    public async Task<IActionResult> DeleteCategory(Guid categoryId)
    {
        try
        {
            if (ModelState.IsValid)
            {

                var category = knowledgeBaseCategoryManager.GetById(categoryId);
                if (category.Name != null)
                {
                    knowledgeBaseCategoryManager.Remove(category);
                    knowledgeBaseCategoryManager.Context.SaveChanges();
                    _logger.LogInformation("Knowledge Category deleted successfully. User: {0}",
                        aspNetUserId);
                    return Ok();
                }
                else
                {
                    _logger.LogError("An error ocurred deleting Knowledge Category. User: {0}",
                        aspNetUserId);
                    return BadRequest();
                }

            }
            _logger.LogError("An error ocurred deleting Knowledge Category. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e,"An error ocurred deleting Knowledge Category. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        
    }
    
}