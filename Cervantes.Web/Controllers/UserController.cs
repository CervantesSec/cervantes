using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Web;
using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.CORE.ViewModels;
using Cervantes.IFR.Email;
using Cervantes.IFR.File;
using Cervantes.Server.Helpers;
using Cervantes.Web.Helpers;
using Ganss.Xss;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
public class UserController: ControllerBase
{
    private readonly ILogger<UserController> _logger = null;
    private IUserManager usrManager = null;
    private IProjectManager projectManager = null;
    private IRoleManager roleManager = null;
    private IClientManager clientManager = null;
    private Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager = null;
    private readonly IWebHostEnvironment env;
    private IHttpContextAccessor HttpContextAccessor;
    private string aspNetUserId;
    private string link;
private IFileCheck fileCheck;
private IEmailService emailService;

private Sanitizer sanitizer;
    /// <summary>
    /// UserController Constructor
    /// </summary>
    /// <param name="usrManager">UserManager</param>
    /// <param name="roleManager">RoleManager</param>
    /// <param name="userManager">Identity UserManager</param>
    /// <param name="_appEnvironment">IHostingEnviroment</param>
    public UserController(IUserManager usrManager, IRoleManager roleManager,
        Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager, IProjectManager projectManager,
        IClientManager clientManager,IEmailService emailService,
        ILogger<UserController> logger, IWebHostEnvironment env,IHttpContextAccessor HttpContextAccessor, 
        IFileCheck fileCheck, Sanitizer sanitizer)
    {
        this.usrManager = usrManager;
        this.roleManager = roleManager;
        this.clientManager = clientManager;
        _userManager = userManager;
        this.projectManager = projectManager;
        _logger = logger;
        this.env = env;
        this.emailService = emailService;
        if (HttpContextAccessor.HttpContext.User.Identity.IsAuthenticated == true)
        {
            aspNetUserId = HttpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            link = HttpContextAccessor.HttpContext.Request.Scheme.ToString() +"://" + HttpContextAccessor.HttpContext.Request.Host.ToString() + "/Account/Login";
        }
        this.fileCheck = fileCheck;
        this.sanitizer = sanitizer;

    }
    
    [HttpGet]
    public IEnumerable<CORE.Entities.ApplicationUser> Get()
    {
        try
        {
            IEnumerable<CORE.Entities.ApplicationUser> model = usrManager.GetAll().ToArray();
        
            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred getting users User: {0}",
                aspNetUserId);
            throw;
        }
        
    }
    
    [HttpGet]
    [Route("Roles")]
    public IEnumerable<IdentityRole> GetRoles()
    {
        try
        {
            IEnumerable<IdentityRole> model = roleManager.GetAll().ToArray();
        
            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred getting roles. User: {0}",
                aspNetUserId);
            throw;
        }
        
    }
    
    [HttpGet]
    [Route("Role")]
    public async Task<string> GetRole()
    {
        try
        {
            var user2 = usrManager.GetByUserId(aspNetUserId);
            var rolUser = await _userManager.GetRolesAsync(user2);
            var test = rolUser.First();
            return  await Task.FromResult(rolUser.ToString());
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred getting user roles. User: {0}",
                aspNetUserId);
            throw;
        }
       
    }

    [HttpGet]
    [Route("{userId}")]
    public ApplicationUser GetUser(string userId)
    {
        try
        {
            return usrManager.GetByUserId(userId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred getting user. User: {0}",
                aspNetUserId);
            throw;
        }
    }

    [HttpPost] 
    public async Task<IActionResult> Add([FromBody] UserCreateViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser();
                var path = "";
                var unique = "";
                
                if (model.FileContent != null)
                {
                    if (fileCheck.CheckFile(model.FileContent))
                    {
                        unique = Guid.NewGuid().ToString()+"."+fileCheck.GetExtension(model.FileContent);
                        path = $"{env.WebRootPath}/Attachments/Users/{unique}";
                        var fs = System.IO.File.Create(path);
                        fs.Write(model.FileContent, 0,
                            model.FileContent.Length);
                        fs.Close();
                    }
                    else
                    {
                        _logger.LogError("An error ocurred adding a User. User: {0}",
                            aspNetUserId);
                        return BadRequest("Invalid file type");
                    }
                  
                    user.Avatar = "Attachments/Users/" + unique;
                }


                user.Id = Guid.NewGuid().ToString();
                user.UserName = sanitizer.Sanitize(model.Email);
                user.NormalizedUserName = sanitizer.Sanitize(model.Email);
                user.Email = sanitizer.Sanitize(model.Email);
                user.NormalizedEmail = sanitizer.Sanitize(model.Email.ToUpper());
                user.SecurityStamp = Guid.NewGuid().ToString();
                user.PhoneNumber = sanitizer.Sanitize(model.PhoneNumber);
                user.FullName = sanitizer.Sanitize(model.FullName);
                user.Description = sanitizer.Sanitize(model.Description);
                user.Position = sanitizer.Sanitize(model.Position);
                user.LockoutEnabled = true;
                if (model.Role == "Client")
                {
                    user.ClientId = model.ClientId;
                }


                await usrManager.AddAsync(user);
                var hasher = new PasswordHasher<ApplicationUser>();
                user.PasswordHash = hasher.HashPassword(user, model.Password);
                await usrManager.Context.SaveChangesAsync();
                await _userManager.AddToRoleAsync(user, model.Role);
                _logger.LogInformation("User added successfully. User: {0}",
                    aspNetUserId);
                
                if (emailService.IsEnabled())
                {
                    BackgroundJob.Enqueue(
                        () => emailService.SendWelcome(user.Id,link));
                }
                
                return Ok();
            }
            _logger.LogError("An error ocurred adding User filetype not admitted. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            _logger.LogError(e, "An error ocurred adding a User. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        
    }
    
    [HttpPut] 
    public async Task<IActionResult> Edit([FromBody] UserEditViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var user = usrManager.GetByUserId(model.Id);
                var path = "";
                var unique = "";
                if (model.FileContent != null)
                {
                    
                    
                    if (fileCheck.CheckFile(model.FileContent))
                    {
                        unique = Guid.NewGuid().ToString()+"."+fileCheck.GetExtension(model.FileContent);
                        path = $"{env.WebRootPath}/Attachments/Users/{unique}";
                        var fs = System.IO.File.Create(path);
                        fs.Write(model.FileContent, 0,
                            model.FileContent.Length);
                        fs.Close();
                    }
                    else
                    {
                        _logger.LogError("An error ocurred editing User filetype not admitted. User: {{0}}",
                            aspNetUserId);
                        return BadRequest("Invalid file type");
                    }
                    

                    user.Avatar = "Attachments/Users/" + unique;
                    
                }
                user.UserName = sanitizer.Sanitize(model.Email);
                user.NormalizedUserName = sanitizer.Sanitize(model.Email);
                user.Email = sanitizer.Sanitize(model.Email);
                user.NormalizedEmail = sanitizer.Sanitize(model.Email.ToUpper());
                user.SecurityStamp = Guid.NewGuid().ToString();
                user.ConcurrencyStamp = Guid.NewGuid().ToString();
                user.PhoneNumber = sanitizer.Sanitize(model.PhoneNumber);
                user.FullName = sanitizer.Sanitize(model.FullName);
                user.Description = sanitizer.Sanitize(model.Description);
                user.Position = sanitizer.Sanitize(model.Position);
                user.TwoFactorEnabled = model.TwoFactorEnabled;
                if (model.Lockout)
                {
                    user.LockoutEnd = DateTimeOffset.MaxValue;
                }
                else
                {
                    user.LockoutEnd = null;
                }
                
                if (model.Role == "Client")
                {
                    user.ClientId = model.ClientId;
                }

                if (model.Password != null)
                {
                    var hasher = new PasswordHasher<ApplicationUser>();
                    user.PasswordHash = hasher.HashPassword(user, model.Password);
                }

                await usrManager.Context.SaveChangesAsync();

                if (!string.IsNullOrEmpty(model.Role))
                {
                    var role = await _userManager.GetRolesAsync(user);
                    await _userManager.RemoveFromRoleAsync(user, role.First());
                    await _userManager.AddToRoleAsync(user, model.Role);
                }
                
                _logger.LogInformation("User edited successfully. User: {0}",
                    aspNetUserId);
                return Ok();
            }

            _logger.LogError( "An error ocurred editing a User. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred editing a User. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
    }
    
    [HttpPut]
    [Route("Profile")]
    [Authorize(Roles = "Admin,SuperUser,User,Client")]
    public async Task<IActionResult> ProfileEdit([FromBody] ProfileEdit model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var user = usrManager.GetByUserId(model.Id);
                if (user != null)
                {
                    if (user.Id != aspNetUserId)
                    {
                        return BadRequest();

                    }
                    //user.UserName = sanitizer.Sanitize(model.Email));
                    //user.NormalizedUserName = sanitizer.Sanitize(model.Email));
                    //user.Email = sanitizer.Sanitize(model.Email));
                    user.PhoneNumber = sanitizer.Sanitize(model.PhoneNumber);
                    user.FullName = sanitizer.Sanitize(model.FullName);
                    user.Description = sanitizer.Sanitize(model.Description);
                    user.Position = sanitizer.Sanitize(model.Position);
                    await usrManager.Context.SaveChangesAsync();
                
                    _logger.LogInformation("User edited successfully. User: {0}",
                        aspNetUserId);
                    return Ok();
                }
                _logger.LogError( "An error ocurred editing a User. User: {0}",
                    aspNetUserId);
                return BadRequest();
            }

            _logger.LogError( "An error ocurred editing a User. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred editing a User. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
    }
    
    [HttpDelete]
    [Route("{userId}")]
    public async Task<IActionResult> Delete(string userId)
    {
        try
        {
            if (ModelState.IsValid)
            {

                var user = usrManager.GetByUserId(userId);
                if (user != null)
                {
                    usrManager.Remove(user);
                    await usrManager.Context.SaveChangesAsync();
                    _logger.LogInformation("User deleted successfully. User: {0}",
                        aspNetUserId);
                    return Ok();
                }

                _logger.LogError("An error ocurred deleting a User. User: {0}",
                    aspNetUserId);
                return BadRequest();
            }
            _logger.LogError("An error ocurred deleting a User. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred deleting a User. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
    }
    
    [HttpDelete]
    [Route("Logo/{id}")]
    public async Task<IActionResult> DeleteAvatar(string id)
    {
        try
        {
            var user = usrManager.GetByUserId(id);
            var imagePath = $"{env.WebRootPath}/{user.Avatar}";
            System.IO.File.Delete(imagePath);
            
            user.Avatar = null;

            await usrManager.Context.SaveChangesAsync();
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e,"An error deleting logo. User: {0}",
                aspNetUserId);
            throw;
        }
        
    }
    
    [HttpPost] 
    [Route("Avatar")]
    public async Task<IActionResult> UploadAvatar([FromBody] ProfileUploadAvatar model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var path = "";
                var unique = "";
                
                if (model.FileContent != null)
                {
                    var user = usrManager.GetByUserId(model.Id);
                    if (user.Id != aspNetUserId)
                    {
                        return BadRequest();

                    }
                    
                    if (fileCheck.CheckFile(model.FileContent))
                    {
                        unique = Guid.NewGuid().ToString()+"."+fileCheck.GetExtension(model.FileContent);
                        path = $"{env.WebRootPath}/Attachments/Users/{unique}";
                        var fs = System.IO.File.Create(path);
                        fs.Write(model.FileContent, 0,
                            model.FileContent.Length);
                        fs.Close();
                    }
                    else
                    {
                        _logger.LogError("An error ocurred adding User filetype not admitted. User: {0}",
                            aspNetUserId);
                        return BadRequest("Invalid file type");
                    }
                  
                    user.Avatar = "Attachments/Users/" + unique;
                    await usrManager.Context.SaveChangesAsync();
                    return Ok();
                }
                
                _logger.LogError("An error ocurred uploading avatar. User: {0}",
                    aspNetUserId);
                return BadRequest();
            }
            _logger.LogError("An error ocurred uploading avatar User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            _logger.LogError(e, "An error ocurred uploading avatar. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        
    }
}