using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rotativa.AspNetCore;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Cervantes.Web.Controllers;
[Authorize(Roles = "Admin,SuperUser,User")]
public class ReportController : Controller
{
    private readonly ILogger<ReportController> _logger = null;
    private readonly IHostingEnvironment _appEnvironment;
    private IProjectManager projectManager = null;
    private IClientManager clientManager = null;
    private IProjectUserManager projectUserManager = null;
    private IProjectNoteManager projectNoteManager = null;
    private IProjectAttachmentManager projectAttachmentManager = null;
    private ITargetManager targetManager = null;
    private ITaskManager taskManager = null;
    private IUserManager userManager = null;
    private IVulnManager vulnManager = null;
    private IVulnTargetManager vulnTargetManager = null;
    private IOrganizationManager organizationManager = null;
    private IReportManager reportManager = null;

    public ReportController(IProjectManager projectManager, IClientManager clientManager,
        IOrganizationManager organizationManager, IProjectUserManager projectUserManager,
        IProjectNoteManager projectNoteManager, IVulnTargetManager vulnTargetManager,
        IProjectAttachmentManager projectAttachmentManager, ITargetManager targetManager, ITaskManager taskManager,
        IUserManager userManager, IVulnManager vulnManager, IHostingEnvironment _appEnvironment,
        ILogger<ReportController> logger, IReportManager reportManager)
    {
        this.projectManager = projectManager;
        this.clientManager = clientManager;
        this.organizationManager = organizationManager;
        this.projectUserManager = projectUserManager;
        this.projectNoteManager = projectNoteManager;
        this.projectAttachmentManager = projectAttachmentManager;
        this.targetManager = targetManager;
        this.taskManager = taskManager;
        this.userManager = userManager;
        this.vulnManager = vulnManager;
        this.vulnTargetManager = vulnTargetManager;
        this._appEnvironment = _appEnvironment;
        this.reportManager = reportManager;
        _logger = logger;
    }


    [HttpPost]
    public IActionResult Generate(IFormCollection form)
    {
        try
        {
            var pro = projectManager.GetById(Guid.Parse(form["project"]));
            var vul = vulnManager.GetAll().Where(x => x.ProjectId == Guid.Parse(form["project"])).Select(y => y.Id)
                .ToList();
            var model = new ReportViewModel
            {
                Organization = organizationManager.GetAll().First(),
                Project = pro,
                Vulns = vulnManager.GetAll().Where(x => x.ProjectId == Guid.Parse(form["project"])).ToList(),
                Targets = targetManager.GetAll().Where(x => x.ProjectId == Guid.Parse(form["project"])).ToList(),
                Users = projectUserManager.GetAll().Where(x => x.ProjectId == Guid.Parse(form["project"])).ToList(),
                Reports = reportManager.GetAll().Where(x => x.ProjectId == Guid.Parse(form["project"])).ToList(),
                VulnTargets = vulnTargetManager.GetAll().Where(x => vul.Contains(x.VulnId))
            };


            //var report = new ViewAsPdf(model, ViewData);
            var uploads = Path.Combine(_appEnvironment.WebRootPath, "Attachments/Reports/" + form["project"] + "/");
            var uniqueName = Guid.NewGuid().ToString() + "_" + form["reportName"] + ".pdf";

            if (Directory.Exists(uploads))
            {
                using (var fileStream =
                       new FileStream(Path.Combine(uploads, uniqueName), FileMode.Create, FileAccess.Write))
                {
                    if (form["language"] == "2")
                    {
                        var pdfResult = new ViewAsPdf
                        {
                            ViewName = "TemplateEN",
                            Model = model,
                            FileName = uniqueName,
                            CustomSwitches =
                                "--footer-center \"  " + "Page: [page]/[toPage]\"" +
                                " --footer-line --footer-font-size \"8\" --footer-spacing 1 --footer-font-name \"Segoe UI\""
                        };
                        var pdfData = pdfResult.BuildFile(ControllerContext).Result;
                        fileStream.Write(pdfData, 0, pdfData.Length);
                    }
                    else if (form["language"] == "2")
                    {
                        var pdfResult = new ViewAsPdf
                        {
                            ViewName = "TemplateES",
                            Model = model,
                            FileName = uniqueName,
                            CustomSwitches =
                                "--footer-center \"  " + "Page: [page]/[toPage]\"" +
                                " --footer-line --footer-font-size \"8\" --footer-spacing 1 --footer-font-name \"Segoe UI\""
                        };
                        var pdfData = pdfResult.BuildFile(ControllerContext).Result;
                        fileStream.Write(pdfData, 0, pdfData.Length);
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(uploads);

                using (var fileStream =
                       new FileStream(Path.Combine(uploads, uniqueName), FileMode.Create, FileAccess.Write))
                {
                    var pdfResult = new ViewAsPdf(model)
                    {
                        FileName = uniqueName,
                        CustomSwitches =
                            "--footer-center \"  " + "Page: [page]/[toPage]\"" +
                            " --footer-line --footer-font-size \"8\" --footer-spacing 1 --footer-font-name \"Segoe UI\""
                    };
                    var pdfData = pdfResult.BuildFile(ControllerContext).Result;
                    fileStream.Write(pdfData, 0, pdfData.Length);
                }
            }

            var rep = new Report
            {
                Name = form["reportName"],
                ProjectId = Guid.Parse(form["project"]),
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                CreatedDate = DateTime.Now.ToUniversalTime(),
                Description = form["description"],
                Version = form["version"],
                FilePath = "Attachments/Reports/" + form["project"] + "/" + uniqueName,
                Language = (Language) int.Parse(form["language"])
            };

            reportManager.Add(rep);
            reportManager.Context.SaveChanges();

            TempData["reportCreated"] = "report created";

            return RedirectToAction("Details", "Project", new {id = form["project"]});
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error ocurred generating report for Project: {0}. User: {1}",
                form["project"], User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Index","Project");
        }
    }

    public ActionResult Download(Guid id)
    {
        var report = reportManager.GetById(id);

        var filePath = Path.Combine(_appEnvironment.WebRootPath, report.FilePath);
        var fileName = report.Project.Name + "_" + report.Name + "_v" + report.Version + ".pdf";

        var fileBytes = System.IO.File.ReadAllBytes(filePath);

        return File(fileBytes, "application/PDF", fileName);
    }

    [HttpPost]
    public ActionResult Delete(Guid id)
    {
        try
        {
            var report = reportManager.GetById(id);

            reportManager.Remove(report);
            reportManager.Context.SaveChanges();
            _logger.LogInformation("User: {0} deleted report {1} in Project: {2}", User.FindFirstValue(ClaimTypes.Name),
                id, report.ProjectId);
            TempData["reportDeleted"] = "report deleted";
            return RedirectToAction("Details", "Project", new {id = report.ProjectId});
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error ocurred deleting report for Project: {0}. User: {1}", id,
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Index", "Project");
        }
    }


    public IActionResult Details(Guid id)
    {
        try
        {
            var report = reportManager.GetById(id);
            var model = new Report
            {
                Name = report.Name,
                Description = report.Description,
                UserId = report.UserId,
                User = report.User,
                FilePath = report.FilePath,
                CreatedDate = report.CreatedDate.ToUniversalTime(),
                Version = report.Version
            };
            return View(model);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred loading Report Details. User: {0}. Report: {1}",
                User.FindFirstValue(ClaimTypes.Name), id);
            return RedirectToAction("Index", "Project");
        }
    }
}