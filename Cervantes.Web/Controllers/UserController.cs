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
using StatusGeneric;
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
        this.HttpContextAccessor = HttpContextAccessor;
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

    /// <summary>
    /// [HasPermission] is not evaluated when Blazor components call this controller in-process, and the profile
    /// page (open to every signed-in user) calls Delete and DeleteAvatar. A user may act on their own account;
    /// acting on any other account needs the permission the attribute names.
    /// </summary>
    [NonAction]
    private bool CanActOnUser(string targetUserId, Permissions permission) =>
        (aspNetUserId != null && targetUserId == aspNetUserId) ||
        HttpContextAccessor?.HttpContext?.User.HasPermission(permission) == true;

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
    [HasPermission(Permissions.RolesAdd)]
    public async Task<IActionResult> AddRole([FromBody] CreateRoleViewModel model)
    {
        // [HasPermission] is not evaluated when a Blazor component calls this action in-process.
        if (!HttpContextAccessor.CurrentUserHas(Permissions.RolesAdd))
        {
            return StatusCode(403, "You do not have permission to manage roles");
        }

        try
        {
            if (ModelState.IsValid)
            {
                await authRolesAdminService.CreateRoleToPermissionsAsync(model.Name, model.Permissions, sanitizer.Sanitize(model.Description));
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
        // [HasPermission] is not evaluated when a Blazor component calls this action in-process.
        if (!HttpContextAccessor.CurrentUserHas(Permissions.RolesEdit))
        {
            return StatusCode(403, "You do not have permission to manage roles");
        }

        try
        {
            if (ModelState.IsValid)
            {
                // An omitted (null) description keeps the stored one.
                var description = model.Description ?? authRolesAdminService.QueryRoleToPermissions()
                    .Where(x => x.RoleName == model.Name).Select(x => x.Description).FirstOrDefault();

                IStatusGeneric status;
                if (model.Permissions.Contains("Admin"))
                {
                    IEnumerable<string> permissionNames = new List<string>
                    {
                        "Admin","NotSet"
                    };
                    IEnumerable<string> permissionNames2 = Enum.GetValues<Permissions>()
                        .Select(p => p.ToString()).Except(permissionNames);
                    
                    status = await authRolesAdminService.UpdateRoleToPermissionsAsync(
                        model.Name, permissionNames2,
                        sanitizer.Sanitize(description),
                        // Use a known valid role type to avoid NullReferenceException
                        RoleTypes.Normal);
                }
                else
                {
                    status = await authRolesAdminService.UpdateRoleToPermissionsAsync(
                        model.Name,
                        model.Permissions,
                        sanitizer.Sanitize(description),
                        // Use a known valid role type to avoid NullReferenceException
                        RoleTypes.Normal);
                }

                // AuthP finds the role by name; a renamed role does not exist and nothing is saved.
                if (status.HasErrors)
                {
                    return BadRequest(status.GetAllErrors());
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
        // [HasPermission] is not evaluated when a Blazor component calls this action in-process.
        if (!HttpContextAccessor.CurrentUserHas(Permissions.RolesDelete))
        {
            return StatusCode(403, "You do not have permission to manage roles");
        }

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
                // A local account needs a password and a matching confirmation; external (LDAP/OpenID) accounts do not.
                if (!model.ExternalLogin && (string.IsNullOrWhiteSpace(model.Password) || model.Password != model.ConfirmPassword))
                {
                    return BadRequest("A password and a matching confirmation are required for local accounts");
                }

                var user = new ApplicationUser();
                var path = "";
                var unique = "";
                
                if (model.FileContent != null)
                {
                    if (fileCheck.CheckImage(model.FileContent))
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
                // The password only changes when a new one is sent, and it must match its confirmation.
                if (!string.IsNullOrWhiteSpace(model.Password) && model.Password != model.ConfirmPassword)
                {
                    return BadRequest("The password confirmation does not match");
                }

                var user = usrManager.GetByUserId(model.Id);
                var path = "";
                var unique = "";
                if (model.FileContent != null)
                {
                    
                    
                    if (fileCheck.CheckImage(model.FileContent))
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
                user.FullName = sanitizer.Sanitize(model.FullName);
                // Optional profile fields: an omitted (null) value keeps the stored one.
                if (model.PhoneNumber != null)
                {
                    user.PhoneNumber = sanitizer.Sanitize(model.PhoneNumber);
                }

                if (model.Description != null)
                {
                    user.Description = sanitizer.Sanitize(model.Description);
                }

                if (model.Position != null)
                {
                    user.Position = sanitizer.Sanitize(model.Position);
                }

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
                
                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    var hasher = new PasswordHasher<ApplicationUser>();
                    user.PasswordHash = hasher.HashPassword(user, model.Password);
                }

                await usrManager.Context.SaveChangesAsync();

                // Keep the AuthPermissions copy of the user (email, user name, roles) in sync. An omitted role keeps the
                // user's current roles.
                var roles = new List<string>();
                if (!string.IsNullOrEmpty(model.Role))
                {
                    roles.Add(model.Role);
                }
                else
                {
                    var authUser = await authUsersAdminService.FindAuthUserByUserIdAsync(user.Id);
                    if (authUser.Result != null)
                    {
                        roles.AddRange(authUser.Result.UserRoles.Select(x => x.RoleName));
                    }
                }

                if (roles.Count > 0)
                {
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
                    user.FullName = sanitizer.Sanitize(model.FullName);
                    // Optional profile fields: an omitted (null) value keeps the stored one.
                    if (model.PhoneNumber != null)
                    {
                        user.PhoneNumber = sanitizer.Sanitize(model.PhoneNumber);
                    }

                    if (model.Description != null)
                    {
                        user.Description = sanitizer.Sanitize(model.Description);
                    }

                    if (model.Position != null)
                    {
                        user.Position = sanitizer.Sanitize(model.Position);
                    }

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
        // Checked outside the try so a denial is not logged as an error.
        if (!CanActOnUser(userId, Permissions.UsersDelete))
        {
            _logger.LogWarning("Access denied deleting user {TargetUserId}. User: {UserId}", userId, aspNetUserId);
            return Forbid();
        }

        try
        {
            if (ModelState.IsValid)
            {

                var user = usrManager.GetByUserId(userId);
                if (user != null)
                {
                    // Deleting the user would cascade through everything they created (other people's work under
                    // their clients and KB categories included) or fail on the rows that block it, so a user who owns
                    // shared content is refused; locking the user is the way to take away access.
                    var content = SharedContent(user.Id);
                    if (content.Count > 0)
                    {
                        _logger.LogWarning("User {TargetUserId} owns shared content and was not deleted. User: {UserId}", user.Id, aspNetUserId);
                        return Conflict(new { message = "The user owns content; lock the user instead of deleting it.", content });
                    }

                    usrManager.Remove(user);
                    await usrManager.Context.SaveChangesAsync();
                    var authStatus = await authUsersAdminService.DeleteUserAsync(user.Id);
                    if (authStatus.HasErrors)
                    {
                        _logger.LogWarning("User {TargetUserId} was deleted but its AuthPermissions user was not: {Errors}",
                            user.Id, authStatus.GetAllErrors());
                    }
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
    
    /// <summary>
    /// What a user owns that other people work with, by kind, leaving out zero counts. Personal data (API keys, chats,
    /// notifications, RSS feeds, Identity rows) is not counted: it goes with the user.
    /// </summary>
    internal Dictionary<string, int> SharedContent(string userId)
    {
        var db = usrManager.Context;
        var counts = new Dictionary<string, int>
        {
            ["projects"] = db.Set<Project>().Count(x => x.UserId == userId),
            ["clients"] = db.Set<Client>().Count(x => x.UserId == userId),
            ["vulns"] = db.Set<Vuln>().Count(x => x.UserId == userId),
            ["tasks"] = db.Set<CORE.Entities.Task>().Count(x => x.CreatedUserId == userId || x.AsignedUserId == userId),
            ["targets"] = db.Set<Target>().Count(x => x.UserId == userId),
            ["targetServices"] = db.Set<TargetServices>().Count(x => x.UserId == userId),
            ["documents"] = db.Set<Document>().Count(x => x.UserId == userId),
            ["notes"] = db.Set<Note>().Count(x => x.UserId == userId),
            ["reports"] = db.Set<Report>().Count(x => x.UserId == userId),
            ["reportTemplates"] = db.Set<ReportTemplate>().Count(x => x.UserId == userId),
            ["vaults"] = db.Set<Vault>().Count(x => x.UserId == userId),
            ["knowledgeBase"] = db.Set<KnowledgeBase>().Count(x => x.CreatedUserId == userId || x.UpdatedUserId == userId)
                                + db.Set<KnowledgeBaseCategories>().Count(x => x.UserId == userId),
            ["checklists"] = db.Set<Checklist>().Count(x => x.UserId == userId) + db.Set<ChecklistTemplate>().Count(x => x.UserId == userId),
            ["wstg"] = db.Set<WSTG>().Count(x => x.UserId == userId),
            ["mastg"] = db.Set<MASTG>().Count(x => x.UserId == userId),
            ["jira"] = db.Set<Jira>().Count(x => x.UserId == userId),
            ["projectNotes"] = db.Set<ProjectNote>().Count(x => x.UserId == userId),
            ["vulnNotes"] = db.Set<VulnNote>().Count(x => x.UserId == userId),
            ["taskNotes"] = db.Set<TaskNote>().Count(x => x.UserId == userId),
            ["attachments"] = db.Set<ProjectAttachment>().Count(x => x.UserId == userId)
                              + db.Set<VulnAttachment>().Count(x => x.UserId == userId)
                              + db.Set<TaskAttachment>().Count(x => x.UserId == userId),
            ["customFields"] = db.Set<VulnCustomField>().Count(x => x.UserId == userId)
                               + db.Set<VulnCustomFieldValue>().Count(x => x.UserId == userId)
                               + db.Set<ProjectCustomField>().Count(x => x.UserId == userId)
                               + db.Set<ProjectCustomFieldValue>().Count(x => x.UserId == userId)
                               + db.Set<ClientCustomField>().Count(x => x.UserId == userId)
                               + db.Set<ClientCustomFieldValue>().Count(x => x.UserId == userId)
                               + db.Set<TargetCustomField>().Count(x => x.UserId == userId)
                               + db.Set<TargetCustomFieldValue>().Count(x => x.UserId == userId),
        };
        return counts.Where(x => x.Value > 0).ToDictionary(x => x.Key, x => x.Value);
    }

    [HttpDelete]
    [Route("Logo/{id}")]
    [HasPermission(Permissions.UsersEdit)]
    public async Task<IActionResult> DeleteAvatar(string id)
    {
        // Checked outside the try so a denial is not logged as an error.
        if (!CanActOnUser(id, Permissions.UsersEdit))
        {
            _logger.LogWarning("Access denied deleting the avatar of user {TargetUserId}. User: {UserId}", id, aspNetUserId);
            return Forbid();
        }

        try
        {
            var user = usrManager.GetByUserId(id);
            if (AttachmentPath.TryResolve(env.WebRootPath, user.Avatar, "Users", out var imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
            else if (!string.IsNullOrWhiteSpace(user.Avatar))
            {
                _logger.LogWarning("Stored file of {Entity} {Id} is outside its attachments folder, file not deleted. User: {UserId}",
                    "User", user.Id, aspNetUserId);
            }

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
                    
                    if (fileCheck.CheckImage(model.FileContent))
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