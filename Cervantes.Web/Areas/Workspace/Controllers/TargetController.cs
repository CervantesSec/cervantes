using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.Web.Areas.Workspace.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;

namespace Cervantes.Web.Areas.Workspace.Controllers
{
    [Area("Workspace")]
    public class TargetController : Controller
    {
        private readonly ILogger<TargetController> _logger = null;
        ITargetManager targetManager = null;
        ITargetServicesManager targetServicesManager = null;
        IProjectManager projectManager = null;

        /// <summary>
        /// Target Controller Constructor
        /// </summary>
        /// <param name="targetManager">TargetManager</param>
        /// <param name="targetServicesManager">TargetServiceManager</param>
        /// <param name="projectManager">ProjectManager</param>
        public TargetController(ITargetManager targetManager, ITargetServicesManager targetServicesManager, IProjectManager projectManager, ILogger<TargetController> logger)
        {
            this.targetManager = targetManager;
            this.targetServicesManager = targetServicesManager;
            this.projectManager = projectManager;
            _logger = logger;
        }

        /// <summary>
        /// Method return index Target
        /// </summary>
        /// <param name="project">project id</param>
        /// <returns></returns>
        public ActionResult Index(int project)
        {
            try
            {
                TargetViewModel model = new TargetViewModel
                {
                    Project = projectManager.GetById(project),
                    Target = targetManager.GetAll().Where(x => x.ProjectId == project).ToList()
                };
                return View(model);
            }
            catch (Exception e)
            {
                TempData["error"] = "Error loading targets!";

                _logger.LogError(e, "An error ocurred loading Target Workspace Index. Project: {0} User: {1}", project, User.FindFirstValue(ClaimTypes.Name));
                return View("Error");
            }
        }

        /// <summary>
        /// Method retusn details of a Target
        /// </summary>
        /// <param name="project">Project Id</param>
        /// <param name="id">Target Id</param>
        /// <returns></returns>
        public ActionResult Details(int project, int id)
        {
            try
            {
                TargetDetailsViewModel model = new TargetDetailsViewModel
                {
                    Project = projectManager.GetById(project),
                    Target = targetManager.GetById(id),
                    TargetServices = targetServicesManager.GetAll().Where(x => x.TargetId == id)

                };
                return View(model);
            }
            catch (Exception e)
            {
                TempData["error"] = "Error loading target!";

                _logger.LogError(e, "An error ocurred loading Target Workspace Details. Project: {0} Target: {1} User: {2}", project, id, User.FindFirstValue(ClaimTypes.Name));
                return View("Error");
            }
        }

        /// <summary>
        /// Method retur creation form of Traget
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Method saves Target form
        /// </summary>
        /// <param name="project">Project Id</param>
        /// <param name="model">Target</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int project, Target model)
        {
            try
            {
                Target target = new Target
                {
                    Name = model.Name,
                    Type = model.Type,
                    Description = model.Description,
                    ProjectId = project,
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                };
                targetManager.Add(target);
                targetManager.Context.SaveChanges();
                TempData["addedTarget"] = "Target added successfully!";

                _logger.LogInformation("User: {0} added a new target on Project: {1}", User.FindFirstValue(ClaimTypes.Name), project);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                TempData["error"] = "Error creating target!";

                _logger.LogError(e, "An error ocurred adding a new Target Workspace on Project: {0} User: {1}", project, User.FindFirstValue(ClaimTypes.Name));
                return View();
            }
        }

        /// <summary>
        /// Method return edit form of a target
        /// </summary>
        /// <param name="project">Project Id</param>
        /// <param name="id">Target Id</param>
        /// <returns></returns>
        public ActionResult Edit(int project, int id)
        {
            try
            {
                var model = targetManager.GetById(id);
                return View(model);
            }
            catch (Exception e)
            {
                TempData["error"] = "Error loading target!";

                _logger.LogError(e, "An error ocurred loading Target Workspace Edit form. Project: {0} Target: {1} User: {2}", project, id, User.FindFirstValue(ClaimTypes.Name));
                return View();
            }
        }

        /// <summary>
        /// Method saves edit form of a Target
        /// </summary>
        /// <param name="project">Project Id</param>
        /// <param name="model">Target</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int project, Target model)
        {
            try
            {
                var result = targetManager.GetById(model.Id);
                result.Name = model.Name;
                result.Type = model.Type;
                result.Description = model.Description;
                result.ProjectId = project;

                targetManager.Context.SaveChanges();
                TempData["edited"] = "edited";
                _logger.LogInformation("User: {0} edited target: {1} on Project: {2}", User.FindFirstValue(ClaimTypes.Name), model.Name, project);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                TempData["error"] = "Error editing target!";

                _logger.LogError(e, "An error ocurred editing Target Workspace Details. Project: {0} Target: {1} User: {2}", project, model.Name, User.FindFirstValue(ClaimTypes.Name));
                return View();
            }
        }

        /// <summary>
        /// Method return view to delete target
        /// </summary>
        /// <param name="project">Porject Id</param>
        /// <param name="id">Target Id</param>
        /// <returns></returns>
        public ActionResult Delete(int project, int id)
        {
            try
            {
                var model = targetManager.GetById(id);
                return View(model);
            }
            catch (Exception e)
            {
                TempData["error"] = "Error loading target!";

                _logger.LogError(e, "An error ocurred loading Target Workspace Delete form. Project: {0} Target: {1} User: {2}", project, id, User.FindFirstValue(ClaimTypes.Name));
                return View();
            }
        }

        /// <summary>
        /// Method confirm delete of a target
        /// </summary>
        /// <param name="project">Project Id</param>
        /// <param name="id">Target Id</param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int project, int id, IFormCollection collection)
        {
            try
            {
                var result = targetManager.GetById(id);
                if (result != null)
                {
                    targetManager.Remove(result);
                    targetManager.Context.SaveChanges();
                }

                TempData["deleted"] = "deleted";
                _logger.LogInformation("User: {0} deleted target: {1} on Project: {2}", User.FindFirstValue(ClaimTypes.Name), id, project);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                TempData["error"] = "Error deleting target!";

                _logger.LogError(e, "An error ocurred deleting Target Workspace. Project: {0} Target: {1} User: {2}", project, id, User.FindFirstValue(ClaimTypes.Name));
                return View();
            }
        }


        /// <summary>
        /// Method return a Traget Service Details
        /// </summary>
        /// <param name="project">Project Id</param>
        /// <param name="id">Target Service Id</param>
        /// <returns></returns>
        public ActionResult Service(int project, int id)
        {
            try
            {
                TargetServiceViewModel model = new TargetServiceViewModel
                {
                    Project = projectManager.GetById(project),
                    Target = targetManager.GetById(id),
                    TargetService = targetServicesManager.GetById(id),

                };
                return View(model);
            }
            catch (Exception e)
            {
                TempData["error"] = "Error loading service!";

                _logger.LogError(e, "An error ocurred loading Target Workspace Service details. Project: {0} Target: {1} User: {2}", project, id, User.FindFirstValue(ClaimTypes.Name));
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddService(int project, int target, IFormCollection form)
        {
            try
            {
                if (target != 0 && project != 0)
                {
                    TargetServices service = new TargetServices()
                    {
                        Name = form["name"],
                        Version = form["version"],
                        Port = Int32.Parse(form["port"]),
                        Description = form["description"],
                        Note = form["note"],
                        UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                        TargetId = target
                        
                    };

                    targetServicesManager.Add(service);
                    targetServicesManager.Context.SaveChanges();
                    TempData["error"] = "Error adding service!";
                    _logger.LogInformation("User: {0} added a new Target Service on Target {1} on Project {2}", User.FindFirstValue(ClaimTypes.Name), target, project);
                    return RedirectToAction("Details", "Target", new {project = project, id = target});
                }

                return RedirectToAction("Details", "Target", new {project = project, id = target});

            }
            catch (Exception e)
            {
                TempData["error"] = "Error adding service!";

                _logger.LogError(e, "An error ocurred adding Target. Project: {0} Target: {1} User: {2}", project, target, User.FindFirstValue(ClaimTypes.Name));
                return RedirectToAction("Details", "Target", new {project = project, id = target});
            }
        }
        

        public ActionResult EditService(int project, int id, int target)
        {
            try
            {
                TargetServiceViewModel model = new TargetServiceViewModel
                {
                    Project = projectManager.GetById(project),
                    Target = targetManager.GetById(target),
                    TargetService = targetServicesManager.GetById(id)

                };
                return View(model);
            }
            catch (Exception e)
            {
                TempData["error"] = "Error loading service!";

                _logger.LogError(e, "An error ocurred loading Target Workspace Service edit form. Project: {0} Target: {1} User: {2}", project, id, User.FindFirstValue(ClaimTypes.Name));
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditService(int project, TargetServiceViewModel model )
        {
            try
            {

                var result = targetServicesManager.GetById(model.TargetService.Id);
                result.Name = model.TargetService.Name;
                result.Version = model.TargetService.Version;
                result.Port = model.TargetService.Port;
                result.Description = model.TargetService.Description;
                result.Note = model.TargetService.Note;

                targetServicesManager.Context.SaveChanges();
                TempData["editedService"] = "edit service";
                _logger.LogInformation("User: {0} edited target service on Target: {1} on Project: {2}", project, result.Target.Name,User.FindFirstValue(ClaimTypes.Name));
                return RedirectToAction("Details","Target", new {project = result.Target.ProjectId, id = result.TargetId});
            }
            catch (Exception e)
            {
                TempData["error"] = "Error editing service!";
                _logger.LogError(e, "An error ocurred editing Target Workspace Service. Project: {0} User: {1}",project, User.FindFirstValue(ClaimTypes.Name));
                return RedirectToAction("Index","Target");
            }

        }

        public ActionResult DeleteService(int project, int id)
        {
            try
            {
                var model = targetServicesManager.GetById(id);
                return View(model);
            }
            catch (Exception e)
            {
                TempData["error"] = "Error loading service!";

                _logger.LogError(e, "An error ocurred loading Target Workspace Service delete form. Project: {0} Target: {1} User: {2}", project, id, User.FindFirstValue(ClaimTypes.Name));
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteService(int project, int id, IFormCollection collection)
        {
            try
            {
                var result = targetServicesManager.GetById(id);
                if (result != null)
                {
                    targetServicesManager.Remove(result);
                    targetServicesManager.Context.SaveChanges();
                    TempData["deletedService"] = "deleted service";
                    _logger.LogInformation("User: {0} deleted target service on Target: {1} on Project: {2}", User.FindFirstValue(ClaimTypes.Name), id, project);
                    return RedirectToAction("Details","Target",new {project = project, id = result.TargetId});
                }

                return View("Index");
            }
            catch (Exception e)
            {
                TempData["error"] = "Error deleting service!";
                _logger.LogError(e, "An error ocurred deleting Target Workspace Service: {0} Project: {1} User: {2}", id, project, User.FindFirstValue(ClaimTypes.Name));
                return View("Index");
            }
        }




    }
}
