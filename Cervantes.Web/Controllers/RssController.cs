using System.Security.Claims;
using AuthPermissions.AspNetCore;
using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.IFR.File;
using Cervantes.Web.Helpers;
using CodeHollow.FeedReader;
using DocumentFormat.OpenXml.Office.CustomUI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cervantes.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RssController : ControllerBase
{
    private ILogger<RssController> _logger = null;
    private IRssNewsManager rssNewsManager = null;
    private IRssSourceManager rssSourceManager = null;
    private IRssCategoryManager rssCategorymanager;
    private IFileCheck fileCheck;
    private Sanitizer sanitizer;
    private readonly IWebHostEnvironment env;
    private IHttpContextAccessor HttpContextAccessor;
    private string aspNetUserId;
    
    public RssController(IRssNewsManager rssNewsManager, IRssSourceManager rssSourceManager,Sanitizer sanitizer,
        IWebHostEnvironment env, IHttpContextAccessor HttpContextAccessor, IFileCheck fileCheck,
        IRssCategoryManager rssCategorymanager,ILogger<RssController> logger)
    {
        this.rssNewsManager = rssNewsManager;
        this.rssSourceManager = rssSourceManager;
        this.rssCategorymanager = rssCategorymanager;
        this.sanitizer = sanitizer;
        this.env = env;
        this.HttpContextAccessor = HttpContextAccessor;
        this.fileCheck = fileCheck;
        _logger = logger;
        aspNetUserId = HttpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
    }
    
    [HttpGet]
    [Route("Sources")]
    [HasPermission(Permissions.RssSourcesRead)]
    public IEnumerable<CORE.Entities.RssSource> GetSources()
    {
        try
        {
            IEnumerable<CORE.Entities.RssSource> model = rssSourceManager.GetAll().
                Include(x => x.Category).Include(x => x.User).ToArray();
            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred getting RSS Sources. User: {0}",
                aspNetUserId);
            throw;
        }
    }
    
    [HttpPost]
    [Route("Sources")]
    [HasPermission(Permissions.RssSourcesAdd)]
    public async Task<IActionResult> AddSource([FromBody] RssSourceAddViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var source = new RssSource();
                source.Name = sanitizer.Sanitize(model.Name);
                source.Url = sanitizer.Sanitize(model.Url);
                source.UserId = aspNetUserId;
                source.CategoryId = model.CategoryId;
                
                var feed = await FeedReader.ReadAsync(model.Url);
                if (!string.IsNullOrWhiteSpace(feed.ImageUrl))
                {
                    using (HttpClient client = new HttpClient())
                    {
                        // Download the image as a byte array
                        byte[] imageBytes = await client.GetByteArrayAsync(feed.ImageUrl);
                    
                        // Convert the byte array to a Base64 string
                        source.ImagePath = Convert.ToBase64String(imageBytes);
                    }
                }
                else
                {
                    source.ImagePath = string.Empty;
                }
                
                await rssSourceManager.AddAsync(source);
                await rssSourceManager.Context.SaveChangesAsync();

                _logger.LogInformation("Report template added successfully. User: {0}",
                    aspNetUserId);
                return Ok();
            }
            _logger.LogError("An error ocurred adding report templates. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred adding report templates. User: {0}",
                aspNetUserId);
            throw;
        }
    }
    
    [HttpPut]
    [Route("Sources")]
    [HasPermission(Permissions.RssSourcesEdit)]
    public async Task<IActionResult> EditSource([FromBody] RssSourceEditViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var source = rssSourceManager.GetById(model.Id);
                if (source != null)
                {
                    

                    source.Name = sanitizer.Sanitize(model.Name);
                    source.Url = sanitizer.Sanitize(model.Url);
                    source.UserId = aspNetUserId;
                    source.CategoryId = model.CategoryId;
                    await rssSourceManager.Context.SaveChangesAsync();
                    _logger.LogInformation("Report template edited successfully. User: {0}",
                        aspNetUserId);
                    return Ok();
                }

                _logger.LogError("An error ocurred editing report templates. User: {0}",
                    aspNetUserId);
                return BadRequest();
            }

            _logger.LogError("An error ocurred editing report templates. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred editing report templates. User: {0}",
                aspNetUserId);
            throw;
        }
    }
    
    [HttpDelete]
    [Route("Sources/{id}")]
    [HasPermission(Permissions.RssSourcesDelete)]
    public async Task<IActionResult> DeleteSource(Guid id)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var source = rssSourceManager.GetById(id);
                if (source != null)
                {

                    rssSourceManager.Remove(source);
                    await rssSourceManager.Context.SaveChangesAsync();
                    _logger.LogInformation("Report template deletes successfully. User: {0}",
                        aspNetUserId);
                    return Ok();
                }

                _logger.LogError("An error ocurred deleting report templates. User: {0}",
                    aspNetUserId);
                return BadRequest();
            }

            _logger.LogError("An error ocurred deleting report templates. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred deleting report templates. User: {0}",
                aspNetUserId);
            throw;
        }
    }
    
    [HttpGet]
    [Route("Categories")]
    [HasPermission(Permissions.RssCategoriesRead)]
    public IEnumerable<CORE.Entities.RssCategory> GetCategories()
    {
        try
        {
            IEnumerable<CORE.Entities.RssCategory> model = rssCategorymanager.GetAll().ToArray();
            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred getting RSS Categories. User: {0}",
                aspNetUserId);
            throw;
        }
    }
    
    [HttpPost]
    [Route("Categories")]
    [HasPermission(Permissions.RssCategoriesAdd)]
    public async Task<IActionResult> AddCategory([FromBody] RssCategoryAddViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var cat = new RssCategory();
                cat.Name = sanitizer.Sanitize(model.Name);
                cat.Description = sanitizer.Sanitize(model.Description);
                await rssCategorymanager.AddAsync(cat);
                await rssCategorymanager.Context.SaveChangesAsync();

                _logger.LogInformation("Report template added successfully. User: {0}",
                    aspNetUserId);
                return Ok();
            }
            _logger.LogError("An error ocurred adding report templates. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred adding report templates. User: {0}",
                aspNetUserId);
            throw;
        }
    }
    
    [HttpPut]
    [Route("Categories")]
    [HasPermission(Permissions.RssCategoriesEdit)]
    public async Task<IActionResult> EditCategory([FromBody] RssCategoryEditViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var cat = rssCategorymanager.GetById(model.Id);
                if (cat != null)
                {
                    cat.Name = sanitizer.Sanitize(model.Name);
                    cat.Description = sanitizer.Sanitize(model.Description);

                    await rssCategorymanager.Context.SaveChangesAsync();
                    _logger.LogInformation("Report template edited successfully. User: {0}",
                        aspNetUserId);
                    return Ok();
                }

                _logger.LogError("An error ocurred editing report templates. User: {0}",
                    aspNetUserId);
                return BadRequest();
            }

            _logger.LogError("An error ocurred editing report templates. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred editing report templates. User: {0}",
                aspNetUserId);
            throw;
        }
    }
    
    [HttpDelete]
    [Route("Categories/{id}")]
    [HasPermission(Permissions.RssCategoriesDelete)]
    public async Task<IActionResult> DeletCategory(Guid id)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var cat = rssCategorymanager.GetById(id);
                if (cat != null)
                {

                    rssCategorymanager.Remove(cat);
                    await rssCategorymanager.Context.SaveChangesAsync();
                    _logger.LogInformation("Report template deletes successfully. User: {0}",
                        aspNetUserId);
                    return Ok();
                }

                _logger.LogError("An error ocurred deleting report templates. User: {0}",
                    aspNetUserId);
                return BadRequest();
            }

            _logger.LogError("An error ocurred deleting report templates. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred deleting report templates. User: {0}",
                aspNetUserId);
            throw;
        }
    }
}