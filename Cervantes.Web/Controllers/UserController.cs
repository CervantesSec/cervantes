using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Web;
using AuthPermissions.AdminCode;
using AuthPermissions.AspNetCore;
using AuthPermissions.BaseCode.DataLayer.Classes.SupportTypes;
using AuthPermissions.BaseCode.PermissionsCode;
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
[Authorize]
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
    private IAuthRolesAdminService authRolesAdminService;
    private IAuthUsersAdminService authUsersAdminService;
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
        IFileCheck fileCheck, Sanitizer sanitizer, IAuthRolesAdminService authRolesAdminService, IAuthUsersAdminService authUsersAdminService)
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
        this.authRolesAdminService = authRolesAdminService;
        this.authUsersAdminService = authUsersAdminService;
    }
    
    [HttpGet]
    [HasPermission(Permissions.UsersRead)]
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
    [HasPermission(Permissions.RolesRead)]
    public IEnumerable<RoleWithPermissionNamesDto> GetRoles()
    {
        try
        {
            IEnumerable<RoleWithPermissionNamesDto> model = authRolesAdminService.QueryRoleToPermissions().ToArray();
        
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
    [HasPermission(Permissions.RolesRead)]
    public async Task<string> GetRole(string userId)
    {
        try
        {
            var user2 = usrManager.GetByUserId(userId);

            var rolUser = await authUsersAdminService.FindAuthUserByUserIdAsync(user2.Id);
            if (rolUser.Result == null)
            {
                return String.Empty;
            }
            var test = rolUser.Result.UserRoles;

            if (test.Count == 0)
            {
                return String.Empty;
            }
            else
            {
                return test.First().RoleName;
            }
           
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred getting user roles. User: {0}",
                aspNetUserId);
            throw;
        }
       
    }
    
    [HttpGet]
    [Route("Role/{userId}")]
    [HasPermission(Permissions.RolesRead)]
    public IEnumerable<PermissionsViewModel> GetPermissions()
    {
        try
        {
            var permissions =authRolesAdminService.GetPermissionDisplay(false);
            List<PermissionsViewModel> model = new List<PermissionsViewModel>();
            foreach (var item in permissions)
            {
                model.Add(new PermissionsViewModel()
                {
                    Name = item.PermissionName,
                    ShortName = item.ShortName,
                    Group = item.GroupName,
                    Description = item.Description
                    });
            }
                return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred getting permissions. User: {0}",
                aspNetUserId);
            throw;
        }
       
    }
    
    [HttpPost]
    [Route("Role")]
    [HasPermission(Permissions.RolesRead)]
    public async Task<IActionResult> AddRole([FromBody] CreateRoleViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                await authRolesAdminService.CreateRoleToPermissionsAsync(model.Name, model.Permissions, model.Description);
                return Created();
            }
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
            _logger.LogError(e, "An error occurred adding role. User: {0}",
                aspNetUserId);
            return StatusCode(500, "An error occurred while creating the role. Please try again later.");
        }
       
    }
    
    [HttpPut]
    [Route("Role")]
    [HasPermission(Permissions.RolesEdit)]
    public async Task<IActionResult> EditRole([FromBody] CreateRoleViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (model.Permissions.Contains("Admin"))
                {
                    IEnumerable<string> permissionNames = new List<string>
                    {
                        "Admin","NotSet"
                    };
                    IEnumerable<string> permissionNames2 = Enum.GetValues<Permissions>()
                        .Select(p => p.ToString()).Except(permissionNames);
                    
                    await authRolesAdminService.UpdateRoleToPermissionsAsync(
                        model.Name, permissionNames2,
                        model.Description,
                        // Use a known valid role type to avoid NullReferenceException
                        RoleTypes.Normal);
                }
                else
                {
                    await authRolesAdminService.UpdateRoleToPermissionsAsync(
                        model.Name,
                        model.Permissions,
                        model.Description,
                        // Use a known valid role type to avoid NullReferenceException
                        RoleTypes.Normal);
                }
                
                return NoContent();
            }
            return BadRequest("Invalid model state");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred editing role. User: {0}", aspNetUserId);
            return BadRequest("An error occurred while updating the role");
        }
    }
    
    [HttpDelete]
    [Route("Role/{roleName}")]
    [HasPermission(Permissions.RolesDelete)]
    public async Task<IActionResult> DeleteRole(string roleName)
    {
        try
        {
            if (ModelState.IsValid)
            {
                await authRolesAdminService.DeleteRoleAsync(roleName, true);
                    _logger.LogInformation("User deleted successfully. User: {0}",
                        aspNetUserId);
                    return NoContent();
            }
            _logger.LogError("Validation failed when deleting a User. User: {0}",
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
            _logger.LogError(e, "An error occurred deleting a User. User: {0}",
                aspNetUserId);
            return StatusCode(500, "An error occurred while deleting the user. Please try again later.");
        }
    }

    [HttpGet]
    [Route("{userId}")]
    [HasPermission(Permissions.UsersRead)]
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
    [HasPermission(Permissions.UsersAdd)]
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
                user.NormalizedUserName = sanitizer.Sanitize(model.Email.ToUpper());
                user.Email = sanitizer.Sanitize(model.Email);
                user.NormalizedEmail = sanitizer.Sanitize(model.Email.ToUpper());
                user.SecurityStamp = Guid.NewGuid().ToString();
                user.PhoneNumber = sanitizer.Sanitize(model.PhoneNumber);
                user.FullName = sanitizer.Sanitize(model.FullName);
                user.Description = sanitizer.Sanitize(model.Description);
                user.Position = sanitizer.Sanitize(model.Position);
                user.LockoutEnabled = true;
                user.ExternalLogin = model.ExternalLogin;


                await usrManager.AddAsync(user);
                if (!model.ExternalLogin)
                {
                    var hasher = new PasswordHasher<ApplicationUser>();
                    user.PasswordHash = hasher.HashPassword(user, model.Password);
                }
                await usrManager.Context.SaveChangesAsync();
                List<string> roles = new List<string>();
                roles.Add(model.Role);
               await authUsersAdminService.AddNewUserAsync(user.Id,user.Email, user.UserName, roles);
                
                _logger.LogInformation("User added successfully. User: {0}",
                    aspNetUserId);
                
                if (emailService.IsEnabled())
                {
                    BackgroundJob.Enqueue(
                        () => emailService.SendWelcome(user.Id,link));
                }

                return CreatedAtAction(nameof(GetUser), new { userId = user.Id }, user);
            }
            _logger.LogError("An error ocurred adding User filetype not admitted. User: {0}",
                aspNetUserId);
            return BadRequest("Invalid request");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            _logger.LogError(e, "An error ocurred adding a User. User: {0}",
                aspNetUserId);
            return BadRequest("Invalid request");
        }
        
    }
    
    [HttpPut] 
    [HasPermission(Permissions.UsersEdit)]
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
                user.NormalizedUserName = sanitizer.Sanitize(model.Email.ToUpper());
                user.Email = sanitizer.Sanitize(model.Email);
                user.NormalizedEmail = sanitizer.Sanitize(model.Email.ToUpper());
                user.SecurityStamp = Guid.NewGuid().ToString();
                user.ConcurrencyStamp = Guid.NewGuid().ToString();
                user.PhoneNumber = sanitizer.Sanitize(model.PhoneNumber);
                user.FullName = sanitizer.Sanitize(model.FullName);
                user.Description = sanitizer.Sanitize(model.Description);
                user.Position = sanitizer.Sanitize(model.Position);
                user.TwoFactorEnabled = model.TwoFactorEnabled;
                user.ExternalLogin = model.ExternalLogin;
                if (model.Lockout)
                {
                    user.LockoutEnd = DateTimeOffset.MaxValue;
                }
                else
                {
                    user.LockoutEnd = null;
                }
                
                if (model.Password != null)
                {
                    var hasher = new PasswordHasher<ApplicationUser>();
                    user.PasswordHash = hasher.HashPassword(user, model.Password);
                }

                await usrManager.Context.SaveChangesAsync();

                if (!string.IsNullOrEmpty(model.Role))
                {
                    var roles = new List<string>();
                    roles.Add(model.Role);
                    await authUsersAdminService.UpdateUserAsync(user.Id, user.Email, user.UserName, roles);
                }
                
                _logger.LogInformation("User edited successfully. User: {0}",
                    aspNetUserId);
                return NoContent();
            }

            _logger.LogError( "An error ocurred editing a User. User: {0}",
                aspNetUserId);
            return BadRequest("Invalid request");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred editing a User. User: {0}",
                aspNetUserId);
            return BadRequest("Invalid request");
        }
    }
    
    [HttpPut]
    [Route("Profile")]
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
                        return BadRequest("Invalid request");

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
                    return NoContent();
                }
                _logger.LogError( "An error ocurred editing a User. User: {0}",
                    aspNetUserId);
                return BadRequest("Invalid request");
            }

            _logger.LogError( "An error ocurred editing a User. User: {0}",
                aspNetUserId);
            return BadRequest("Invalid request");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred editing a User. User: {0}",
                aspNetUserId);
            return BadRequest("Invalid request");
        }
    }
    
    [HttpDelete]
    [Route("{userId}")]
    [HasPermission(Permissions.UsersDelete)]
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
                    await authUsersAdminService.DeleteUserAsync(user.Id);
                    _logger.LogInformation("User deleted successfully. User: {0}",
                        aspNetUserId);
                    return NoContent();
                }

                _logger.LogError("An error ocurred deleting a User. User: {0}",
                    aspNetUserId);
                return BadRequest("Invalid request");
            }
            _logger.LogError("Validation failed when deleting a User. User: {0}",
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
            _logger.LogError(e, "An error occurred deleting a User. User: {0}",
                aspNetUserId);
            return StatusCode(500, "An error occurred while deleting the user. Please try again later.");
        }
    }
    
    [HttpDelete]
    [Route("Logo/{id}")]
    [HasPermission(Permissions.UsersEdit)]
    public async Task<IActionResult> DeleteAvatar(string id)
    {
        try
        {
            var user = usrManager.GetByUserId(id);
            var imagePath = $"{env.WebRootPath}/{user.Avatar}";
            System.IO.File.Delete(imagePath);
            
            user.Avatar = null;

            await usrManager.Context.SaveChangesAsync();
            return NoContent();
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
                        return BadRequest("Invalid request");

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
                    return NoContent();
                }
                
                _logger.LogError("An error ocurred uploading avatar. User: {0}",
                    aspNetUserId);
                return BadRequest("Invalid request");
            }
            _logger.LogError("An error ocurred uploading avatar User: {0}",
                aspNetUserId);
            return BadRequest("Invalid request");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            _logger.LogError(e, "An error ocurred uploading avatar. User: {0}",
                aspNetUserId);
            return BadRequest("Invalid request");
        }
        
    }
}