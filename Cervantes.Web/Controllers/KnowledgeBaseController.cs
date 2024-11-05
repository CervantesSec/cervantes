using System.Security.Claims;
using System.Web;
using AuthPermissions.AspNetCore;
using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.CORE.ViewModel;
using Cervantes.Web.Helpers;
using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cervantes.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class KnowledgeBaseController : Controller
{
    private IKnowledgeBaseManager knowledgeBaseManager = null;
    private IKnowledgeBaseCategoryManager knowledgeBaseCategoryManager = null;
    private IKnowledgeBaseTagManager KnowledgeBaseTagManager = null;
    private readonly ILogger<KnowledgeBaseController> _logger = null;
    private IHttpContextAccessor HttpContextAccessor;
    private string aspNetUserId;
    private Sanitizer sanitizer;
    
    public KnowledgeBaseController(IKnowledgeBaseManager knowledgeBaseManager, IKnowledgeBaseCategoryManager knowledgeBaseCategoryManager,
        IKnowledgeBaseTagManager KnowledgeBaseTagManager, ILogger<KnowledgeBaseController> logger,
        IHttpContextAccessor HttpContextAccessor, Sanitizer sanitizer)
    {
        this.knowledgeBaseManager = knowledgeBaseManager;
        this.knowledgeBaseCategoryManager = knowledgeBaseCategoryManager;
        this.KnowledgeBaseTagManager = KnowledgeBaseTagManager;
        _logger = logger;
        aspNetUserId = HttpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        this.sanitizer = sanitizer;
    }
    [HttpGet]
    [Route("Page")]
    [HasPermission(Permissions.KnowledgeBaseRead)]
    public IEnumerable<CORE.Entities.KnowledgeBase> GetPages()
    {
        IEnumerable<CORE.Entities.KnowledgeBase> model = knowledgeBaseManager.GetAll().
            Include(x => x.CreatedUser).Include(x => x.UpdatedUser).
            Include(x => x.Category).ToArray();
        return model;
    }
    
     [HttpPost]
     [Route("Page")]
     [HasPermission(Permissions.KnowledgeBaseAdd)]
    public async Task<IActionResult> AddPage([FromBody] KnowledgePageCreateViewModel model)
    {
        try{
            if (ModelState.IsValid)
            {
                
                var page = new CORE.Entities.KnowledgeBase();
                page.Id = Guid.NewGuid();
                page.Title = sanitizer.Sanitize(model.Title);
                page.Content = sanitizer.Sanitize(model.Content);
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
    [HasPermission(Permissions.KnowledgeBaseEdit)]
    public async Task<IActionResult> EditPage([FromBody] KnowledgePageEditViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {

                var page = knowledgeBaseManager.GetById(model.Id);
                if (page != null)
                {
                    
                    page.Title = sanitizer.Sanitize(model.Title);
                    page.Content = sanitizer.Sanitize(model.Content);
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
    [HasPermission(Permissions.KnowledgeBaseDelete)]
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
    [HasPermission(Permissions.KnowledgeBaseCategoryRead)]
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
     [HasPermission(Permissions.KnowledgeBaseCategoryAdd)]
    public async Task<IActionResult> AddCategory([FromBody] KnowledgeCategoryCreateVIewModel model)
    {
        try{
            if (ModelState.IsValid)
            {
                
                var cat = new CORE.Entities.KnowledgeBaseCategories();
                cat.Id = Guid.NewGuid();
                cat.Name = sanitizer.Sanitize(model.Name);
                cat.Description = sanitizer.Sanitize(model.Description);
                cat.Order = model.Order;
                cat.Icon = sanitizer.Sanitize(string.Empty);
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
    [HasPermission(Permissions.KnowledgeBaseCategoryEdit)]
    public async Task<IActionResult> EditCategory([FromBody] KnowledgeCategoryEditViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {

                var category = knowledgeBaseCategoryManager.GetById(model.Id);
                if (category != null)
                {

                    category.Name = sanitizer.Sanitize(model.Name);
                    category.Description = sanitizer.Sanitize(model.Description);
                    category.Order = model.Order;
                    if (model.Icon == null)
                    {
                        category.Icon = sanitizer.Sanitize(string.Empty);
                    }
                    else
                    {
                        category.Icon = sanitizer.Sanitize(model.Icon);

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
    [HasPermission(Permissions.KnowledgeBaseCategoryDelete)]
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