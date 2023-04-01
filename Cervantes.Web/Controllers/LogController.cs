using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Cervantes.Web.Controllers;

[Authorize(Roles = "Admin")]
public class LogController : Controller
{
    private readonly ILogger<LogController> _logger = null;
    private ILogManager logManager = null;

    public LogController(ILogger<LogController> logger, ILogManager logManager)
    {
        _logger = logger;
        this.logManager = logManager;
    }

    public ActionResult Index()
    {
        try
        {
            var logs = logManager.GetAll().Select(e => new Log
            {
                Id = e.Id,
                Level = e.Level,
                StackTrace = e.StackTrace,
                Message = e.Message,
                CreatedOn = e.CreatedOn,
                Exception = e.Exception,
                Url = e.Url,
                Logger = e.Logger
            }).ToList();

            var model = new LogViewModel
            {
                Logs = logs
            };
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error ocurred loading System Logs. by User: {0}",
                User.FindFirstValue(ClaimTypes.Name));
            return View();
        }
    }


    public ActionResult Export()
    {
        try
        {
            var logs = logManager.GetAll().Select(e => new Log
            {
                Id = e.Id,
                Level = e.Level,
                StackTrace = e.StackTrace,
                Message = e.Message,
                CreatedOn = e.CreatedOn,
                Exception = e.Exception,
                Url = e.Url,
                Logger = e.Logger
            });

            var jsonString = JsonSerializer.Serialize(logs);
            var fileName = "logs-export-" + DateTime.Now.ToString() + ".json";
            var mimeType = "text/plain";
            var fileBytes = Encoding.ASCII.GetBytes(jsonString);
            TempData["logsExported"] = "edited";
            return new FileContentResult(fileBytes, mimeType)
            {
                FileDownloadName = fileName
            };
        }
        catch (Exception e)
        {
            TempData["errorLogsExported"] = "edited";
            _logger.LogError(e, "An error ocurred exporting logs. by User: {0}",
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Index");
        }
        
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Delete()
    {
        try
        {
            var logs = logManager.GetAll();
            foreach (var log in logs)
            {
                logManager.Remove(log);
            }

            logManager.Context.SaveChanges();
            TempData["logsDeleted"] = "edited";
            return RedirectToAction("Index");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred deleting logs. by User: {0}",
                User.FindFirstValue(ClaimTypes.Name));
            TempData["errorLogsDeleted"] = "edited";
            return RedirectToAction("Index");
        }
    }
}