using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Web;
using Cervantes.IFR.Email;
using Ganss.XSS;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using MimeDetective;

namespace Cervantes.Web.Controllers;

[Authorize(Roles = "Admin")]
public class UserController : Controller
{
    private readonly ILogger<UserController> _logger = null;
    private readonly IHostingEnvironment _appEnvironment;
    private IUserManager usrManager = null;
    private IProjectManager projectManager = null;
    private IRoleManager roleManager = null;
    private IClientManager clientManager = null;
    private Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager = null;
    private IEmailService email = null;


    /// <summary>
    /// UserController Constructor
    /// </summary>
    /// <param name="usrManager">UserManager</param>
    /// <param name="roleManager">RoleManager</param>
    /// <param name="userManager">Identity UserManager</param>
    /// <param name="_appEnvironment">IHostingEnviroment</param>
    public UserController(IUserManager usrManager, IRoleManager roleManager,
        Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager, IProjectManager projectManager,
       IClientManager clientManager, IHostingEnvironment _appEnvironment,
        ILogger<UserController> logger, IEmailService email)
    {
        this.usrManager = usrManager;
        this.roleManager = roleManager;
        this.clientManager = clientManager;
        this._appEnvironment = _appEnvironment;
        _userManager = userManager;
        this.projectManager = projectManager;
        this.email = email;
        _logger = logger;
    }

    /// <summary>
    /// Method show all users
    /// </summary>
    /// <returns></returns>
    public ActionResult Index()
    {
        try
        {
            var model = usrManager.GetAll().Select(e => new UserViewModel
            {
                Id = e.Id,
                Email = e.Email,
                FullName = e.FullName,
                LockoutEnabled = e.LockoutEnabled,
                TwoFactorEnabled = e.TwoFactorEnabled,
                Position = e.Position,
            });


            if (model != null)
            {
                return View(model);
            }
            else
            {
                TempData["empty"] = "No clients introduced";
                return View();
            }
        }
        catch (Exception ex)
        {
            TempData["errorLoading"] = "Error loading users!";

            _logger.LogError(ex, "An error ocurred loading User Index. by User: {0}",
                User.FindFirstValue(ClaimTypes.Name));
            return View();
        }
    }

    /// <summary>
    /// Method show user details
    /// </summary>
    /// <param name="id">User Id</param>
    /// <returns></returns>
    public ActionResult Details(string id)
    {
        try
        {
            var user = usrManager.GetByUserId(id);
            var rolUser = _userManager.GetRolesAsync(user);

            if (user != null)
            {
                var model = new UserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    FullName = user.FullName,
                    TwoFactorEnabled = user.TwoFactorEnabled,
                    Email = user.Email,
                    Avatar = user.Avatar,
                    Description = user.Description,
                    Position = user.Position,
                    PhoneNumber = user.PhoneNumber,
                    Option = rolUser.Result.FirstOrDefault().ToString(),
                    Project = projectManager.GetAll().Where(x => x.UserId == user.Id).ToList()
                };
                return View(model);
            }
        }
        catch (Exception e)
        {
            TempData["errorLoading"] = "Error loading user!";
            _logger.LogError(e, "An error ocurred loading User Id: {0}. by User: {1}", id,
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Error","Home");
        }

        return View();
    }

    /// <summary>
    /// Mehtos show user creation form
    /// </summary>
    /// <returns></returns>
    public ActionResult Create()
    {
        try
        {
            //get roles to a list
            var rol = roleManager.GetAll().Select(e => new RoleList
            {
                Id = e.Id.ToString(),
                Name = e.Name
            }).ToList();

            var li = new List<SelectListItem>();

            foreach (var item in rol) li.Add(new SelectListItem {Text = item.Name, Value = item.Name});
            
            var clients = clientManager.GetAll().Select(e => new ClientViewModel
            {
                Id = e.Id,
                Name = e.Name
            }).ToList();

            var liCli = new List<SelectListItem>();

            foreach (var item in clients) liCli.Add(new SelectListItem {Text = item.Name, Value = item.Id.ToString()});

            var model = new UserViewModel
            {
                ItemList = li,
                ItemListClient = liCli
            };

            return View(model);
        }
        catch (Exception ex)
        {
            TempData["errorLoading"] = "Error loading user!";
            _logger.LogError(ex, "An error ocurred loading user cration form. by User: {0}",
                User.FindFirstValue(ClaimTypes.Name));
            return View("Index");
        }
    }

    /// <summary>
    /// Method save user creation form
    /// </summary>
    /// <param name="model">UserViewModel</param>
    /// <param name="upload">Avatar File</param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(UserViewModel model, IFormFile upload)
    {
        try
        {

            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMinimum8Chars = new Regex(@".{8,}");

            var isValidated = hasNumber.IsMatch(model.Password) && hasUpperChar.IsMatch(model.Password) && hasMinimum8Chars.IsMatch(model.Password);

            var sanitizer = new HtmlSanitizer();
            sanitizer.AllowedSchemes.Add("data");

            if (!ModelState.IsValid || isValidated == false)
            {
                var rol = roleManager.GetAll().Select(e => new RoleList
                {
                    Id = e.Id.ToString(),
                    Name = e.Name
                }).ToList();

                var li = new List<SelectListItem>();

                foreach (var item in rol) li.Add(new SelectListItem {Text = item.Name, Value = item.Name});
            
                var clients = clientManager.GetAll().Select(e => new ClientViewModel
                {
                    Id = e.Id,
                    Name = e.Name
                }).ToList();

                var liCli = new List<SelectListItem>();

                foreach (var item in clients) liCli.Add(new SelectListItem {Text = item.Name, Value = item.Id.ToString()});

                model.ItemList = li;
                model.ItemListClient = liCli;
                if (isValidated == false)
                {
                    TempData["passwordRequirements"] = "password";
                }
                return View("Create", model);
            }
            
            
            var file = model.upload;
            var hasher = new PasswordHasher<ApplicationUser>();


            if (file != null)
            {
                var Inspector = new ContentInspectorBuilder() {
                    Definitions = MimeDetective.Definitions.Default.FileTypes.Images.All()
                }.Build();
            
                var Results = Inspector.Inspect(file.OpenReadStream());

                if (Results.ByFileExtension().Length == 0 && Results.ByMimeType().Length == 0)
                {
                    TempData["fileNotPermitted"] = "User is not in the project";
                    var rol = roleManager.GetAll().Select(e => new RoleList
                    {
                        Id = e.Id.ToString(),
                        Name = e.Name
                    }).ToList();

                    var li = new List<SelectListItem>();

                    foreach (var item in rol) li.Add(new SelectListItem {Text = item.Name, Value = item.Name});
            
                    var clients = clientManager.GetAll().Select(e => new ClientViewModel
                    {
                        Id = e.Id,
                        Name = e.Name
                    }).ToList();

                    var liCli = new List<SelectListItem>();

                    foreach (var item in clients) liCli.Add(new SelectListItem {Text = item.Name, Value = item.Id.ToString()});

                    model.ItemList = li;
                    model.ItemListClient = liCli;
                    return View("Create", model);
                }
                
                var uploads = Path.Combine(_appEnvironment.WebRootPath, "Attachments/Images/Avatars");
                var uniqueName = Guid.NewGuid().ToString() + "_" + file.FileName;
                using (var fileStream = new FileStream(Path.Combine(uploads, uniqueName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                var user = new ApplicationUser();
                
                if (model.Option == "Client")
                {

                    user.UserName = model.Email;
                        user.Email = model.Email;
                        user.FullName = model.FullName;
                        user.Avatar = "/Attachments/Images/Avatars/" + uniqueName;
                        user.EmailConfirmed = true;
                        user.NormalizedEmail = model.Email.ToUpper();
                        user.NormalizedUserName = model.Email.ToUpper();
                        user.SecurityStamp = Guid.NewGuid().ToString();
                        user.PhoneNumber = model.PhoneNumber;
                        user.Position = model.Position;
                        user.Description = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Description));
                        user.ClientId = model.ClientId;

                }
                else
                {
                    user.UserName = model.Email;
                    user.Email = model.Email;
                    user.FullName = model.FullName;
                    user.Avatar = "/Attachments/Images/Avatars/" + uniqueName;
                    user.EmailConfirmed = true;
                    user.NormalizedEmail = model.Email.ToUpper();
                    user.NormalizedUserName = model.Email.ToUpper();
                    user.SecurityStamp = Guid.NewGuid().ToString();
                    user.PhoneNumber = model.PhoneNumber;
                    user.Position = model.Position;
                    user.Description = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Description));
                    
                }
                
                usrManager.Add(user);
                user.PasswordHash = hasher.HashPassword(user, model.Password);
                usrManager.Context.SaveChanges();
                _userManager.AddToRoleAsync(user, model.Option).Wait();
                TempData["userCreated"] = "created";
                _logger.LogInformation("User: {0} Created a new User: {1}",
                    User.FindFirstValue(ClaimTypes.Name),
                    user.UserName);
                
                var to = new List<EmailAddress>();
                to.Add(new EmailAddress
                {
                    Address = user.Email,
                    Name = user.FullName,
                });

                StreamReader sr =new StreamReader(_appEnvironment.WebRootPath + "/Resources/Email/UserCreated.html");
                string s = sr.ReadToEnd();
                s = s.Replace("{UserName}", user.FullName).Replace("{CervantesLink}",HttpContext.Request.Scheme.ToString() +"://" + HttpContext.Request.Host.ToString() + "/Identity/Account/Login");
                sr.Close();
                
                EmailMessage message = new EmailMessage
                {
                    ToAddresses = to,
                    Subject = "Cervantes - Created User",
                    Content = s
                };
             
                email.Send(message);
                
                return RedirectToAction("Index");
            }


            else
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    Avatar = "",
                    EmailConfirmed = true,
                    NormalizedEmail = model.Email.ToUpper(),
                    NormalizedUserName = model.UserName.ToUpper(),
                    SecurityStamp = Guid.NewGuid().ToString(),
                    PhoneNumber = model.PhoneNumber,
                    Position = model.Position,
                    Description = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Description))
                };
                usrManager.Add(user);
                user.PasswordHash = hasher.HashPassword(user, model.Password);
                usrManager.Context.SaveChanges();
                _userManager.AddToRoleAsync(user, model.Option).Wait();


                TempData["userCreated"] = "created";
                _logger.LogInformation("User: {0} Created a new User: {1}",
                    User.FindFirstValue(ClaimTypes.Name), user.UserName);
                
                var to = new List<EmailAddress>();
                to.Add(new EmailAddress
                {
                    Address = user.Email,
                    Name = user.FullName,
                });

                StreamReader sr =new StreamReader(_appEnvironment.WebRootPath + "/Resources/Email/UserCreated.html");
                string s = sr.ReadToEnd();
                s = s.Replace("{UserName}", user.FullName).Replace("{CervantesLink}",HttpContext.Request.Scheme.ToString() +"://" + HttpContext.Request.Host.ToString() + "/Identity/Account/Login");
                sr.Close();
                
                EmailMessage message = new EmailMessage
                {
                    ToAddresses = to,
                    Subject = "Cervantes - Created User",
                    Content = s
                };
             
                email.Send(message);
                
                return RedirectToAction("Index");
            }
        }
        catch (Exception ex)
        {
            TempData["errorUserCreated"] = "Error creating user!";
            _logger.LogError(ex, "An error ocurred adding a new user. by User: {0}",
                User.FindFirstValue(ClaimTypes.Name));
            return View();
        }
    }

    /// <summary>
    /// Method show edit form
    /// </summary>
    /// <param name="id">User Id</param>
    /// <returns></returns>
    public ActionResult Edit(string id)
    {
        try
        {
            //get roles to a list
            var rol = roleManager.GetAll().Select(e => new RoleList
            {
                Id = e.Id.ToString(),
                Name = e.Name
            }).ToList();

            var li = new List<SelectListItem>();

            foreach (var item in rol) li.Add(new SelectListItem {Text = item.Name, Value = item.Name});

            var user = usrManager.GetByUserId(id);
            var rolUser = _userManager.GetRolesAsync(user);
            var rolId = roleManager.GetByName(rolUser.Result.FirstOrDefault());


            if (user != null)
            {
                var model = new UserEditViewModel
                {
                    ItemList = li,
                    Id = user.Id,
                    UserName = user.UserName,
                    FullName = user.FullName,
                    TwoFactorEnabled = user.TwoFactorEnabled,
                    Email = user.Email,
                    Avatar = user.Avatar,
                    Description = user.Description,
                    Position = user.Position,
                    PhoneNumber = user.PhoneNumber,
                    LockoutEnabled = user.LockoutEnabled,
                    Option = rolUser.Result.FirstOrDefault()
                };

                return View(model);
            }
        }
        catch (Exception e)
        {
            TempData["errorLoading"] = "Error loading user!";
            _logger.LogError(e, "An error ocurred adding a new user. by User: {0}",
                User.FindFirstValue(ClaimTypes.Name));
            RedirectToAction("Index");
        }

        return View();
    }

    /// <summary>
    /// Method save edit form
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model">UserEditViewModel</param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(string id, UserEditViewModel model)
    {
        try
        {
            var sanitizer = new HtmlSanitizer();
            sanitizer.AllowedSchemes.Add("data");

            var hasher = new PasswordHasher<ApplicationUser>();
            var result = usrManager.GetByUserId(id);
            result.FullName = model.FullName;
            result.UserName = model.Email;
            result.Email = model.Email;
            result.Description = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Description));
            if (model.ConfirmPassword != null) result.PasswordHash = hasher.HashPassword(result,model.ConfirmPassword);
            result.Position = model.Position;
            result.PhoneNumber = model.PhoneNumber;
            result.LockoutEnabled = model.LockoutEnabled;
            result.TwoFactorEnabled = model.TwoFactorEnabled;


            if (model.upload != null)
            {
                var file = model.upload;
                
                var Inspector = new ContentInspectorBuilder() {
                    Definitions = MimeDetective.Definitions.Default.FileTypes.Images.All()
                }.Build();
            
                var Results = Inspector.Inspect(file.OpenReadStream());

                if (Results.ByFileExtension().Length == 0 && Results.ByMimeType().Length == 0)
                {
                    TempData["fileNotPermitted"] = "User is not in the project";
                    return RedirectToAction("Edit",new {id = id});
                }
                
                var uploads = Path.Combine(_appEnvironment.WebRootPath, "Attachments/Images/Avatars");
                var uniqueName = Guid.NewGuid().ToString() + "_" + file.FileName;
                using (var fileStream = new FileStream(Path.Combine(uploads, uniqueName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                result.Avatar = "/Attachments/Images/Avatars/" + uniqueName;
            }

            usrManager.Context.SaveChanges();
            var role = _userManager.GetRolesAsync(result).Result.FirstOrDefault();

            _userManager.RemoveFromRoleAsync(result,role).Wait();
            _userManager.AddToRoleAsync(result, model.Option).Wait();


            TempData["userEdited"] = "edited";
            _logger.LogInformation("User: {0} Edited User: {1}", User.FindFirstValue(ClaimTypes.Name), result.UserName);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            TempData["errorUserEdited"] = "Error editing user!";
            _logger.LogError(ex, "An error ocurred Editing user: {0}. by User: {1}", id,
                User.FindFirstValue(ClaimTypes.Name));
            return View();
        }
    }

    /// <summary>
    /// Method show delete page
    /// </summary>
    /// <param name="id">User Id</param>
    /// <returns></returns>
    public ActionResult Delete(string id)
    {
        try
        {
            var user = usrManager.GetByUserId(id);
            if (user != null)
            {
                var model = new UserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    FullName = user.FullName,
                    Email = user.Email,
                    Description = user.Description,
                    Position = user.Position
                };
                return View(model);
            }
        }
        catch (Exception e)
        {
            TempData["errorLoading"] = "Error loading user!";
            _logger.LogError(e, "An error ocurred loading user deletion form,by User: {0}",
                User.FindFirstValue(ClaimTypes.Name));
            RedirectToAction("Index");
        }

        return View();
    }

    /// <summary>
    /// Method confirm delete user
    /// </summary>
    /// <param name="id">User Id</param>
    /// <param name="collection"></param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(string id, IFormCollection collection)
    {
        try
        {
            var user = usrManager.GetByUserId(id);
            if (user != null)
            {
                if (user.Avatar != null)
                {
                    System.IO.File.Delete(_appEnvironment.WebRootPath+user.Avatar);
                }
                usrManager.Remove(user);
                usrManager.Context.SaveChanges();
            }
            

            TempData["userDeleted"] = "deleted";
            _logger.LogInformation("User: {0} Deleted User: {1}", User.FindFirstValue(ClaimTypes.Name), user.UserName);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            TempData["errorDeletedUser"] = "Error deleting user!";
            _logger.LogError(ex, "An error ocurred deleting user: {0}. by User: {1}", id,
                User.FindFirstValue(ClaimTypes.Name));
            return View();
        }
    }

    
    public ActionResult DeleteAvatar(string id)
    {
        try
        {
            var user = usrManager.GetByUserId(id);
            var pathFile = _appEnvironment.WebRootPath + user.Avatar;
            if (System.IO.File.Exists(pathFile)) System.IO.File.Delete(pathFile);


            user.Avatar = null;
            usrManager.Context.SaveChanges();

            TempData["avatar_deleted"] = "avatar deleted";
            _logger.LogInformation("User: {0} deleted User Avatar: {1}", User.FindFirstValue(ClaimTypes.Name),
                user.UserName);
            return RedirectToAction("Edit", "User", new {id = id});
        }
        catch (Exception ex)
        {
            TempData["errorDeleteAvatar"] = "Error deleting avatar user!";
            _logger.LogError(ex, "An error ocurred deleting user avatar: {0}. by User: {1}", id,
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Edit", "User", new {id = id});
        }
    }
}