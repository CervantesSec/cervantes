using System;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Cervantes.Web.Controllers;
[Authorize(Roles = "Admin,SuperUser,User")]
public class ImageController : Controller
{
    private readonly IHostingEnvironment _appEnvironment;

    public ImageController(IHostingEnvironment _appEnvironment)
    {
        this._appEnvironment = _appEnvironment;
    }

    [HttpPost]
    public JsonResult UploadClient(IFormFile image)
    {
        var vReturnImagePath = string.Empty;
        if (image.Length > 0)
        {
            var uploads = Path.Combine(_appEnvironment.WebRootPath, "Attachments/Images/Clients");
            var uniqueName = Guid.NewGuid().ToString() + "_" + image.FileName;
            using (var fileStream = new FileStream(Path.Combine(uploads, uniqueName), FileMode.Create))
            {
                image.CopyTo(fileStream);
            }

            vReturnImagePath = "/Attachments/Images/Clients/" + uniqueName;
            //here to add Image Path to You Database ,  
            TempData["message"] = string.Format("Image was Added Successfully");
        }

        return Json(Convert.ToString(vReturnImagePath));
    }

    [HttpPost]
    public JsonResult UploadProject(IFormFile image)
    {
        var vReturnImagePath = string.Empty;
        if (image.Length > 0)
        {
            var uploads = Path.Combine(_appEnvironment.WebRootPath, "Attachments/Images/Projects");
            var uniqueName = Guid.NewGuid().ToString() + "_" + image.FileName;
            using (var fileStream = new FileStream(Path.Combine(uploads, uniqueName), FileMode.Create))
            {
                image.CopyTo(fileStream);
            }

            vReturnImagePath = "/Attachments/Images/Projects/" + uniqueName;
            //here to add Image Path to You Database ,  
            TempData["message"] = string.Format("Image was Added Successfully");
        }

        return Json(Convert.ToString(vReturnImagePath));
    }

    [HttpPost]
    public JsonResult UploadDocument(IFormFile image)
    {
        var vReturnImagePath = string.Empty;
        if (image.Length > 0)
        {
            var uploads = Path.Combine(_appEnvironment.WebRootPath, "Attachments/Images/Documents");
            var uniqueName = Guid.NewGuid().ToString() + "_" + image.FileName;
            using (var fileStream = new FileStream(Path.Combine(uploads, uniqueName), FileMode.Create))
            {
                image.CopyTo(fileStream);
            }

            vReturnImagePath = "/Attachments/Images/Documents/" + uniqueName;
            //here to add Image Path to You Database ,  
            TempData["message"] = string.Format("Image was Added Successfully");
        }

        return Json(Convert.ToString(vReturnImagePath));
    }

    [HttpPost]
    public JsonResult UploadNote(IFormFile image)
    {
        var vReturnImagePath = string.Empty;
        if (image.Length > 0)
        {
            var uploads = Path.Combine(_appEnvironment.WebRootPath, "Attachments/Images/Note");
            var uniqueName = Guid.NewGuid().ToString() + "_" + image.FileName;
            using (var fileStream = new FileStream(Path.Combine(uploads, uniqueName), FileMode.Create))
            {
                image.CopyTo(fileStream);
            }

            vReturnImagePath = "/Attachments/Images/Note/" + uniqueName;
            //here to add Image Path to You Database ,  
            TempData["message"] = string.Format("Image was Added Successfully");
        }

        return Json(Convert.ToString(vReturnImagePath));
    }

    [HttpPost]
    public JsonResult UploadOrg(IFormFile image)
    {
        var vReturnImagePath = string.Empty;
        if (image.Length > 0)
        {
            var uploads = Path.Combine(_appEnvironment.WebRootPath, "Attachments/Images/Organization");
            var uniqueName = Guid.NewGuid().ToString() + "_" + image.FileName;
            using (var fileStream = new FileStream(Path.Combine(uploads, uniqueName), FileMode.Create))
            {
                image.CopyTo(fileStream);
            }

            vReturnImagePath = "/Attachments/Images/Organization/" + uniqueName;
            //here to add Image Path to You Database ,  
            TempData["message"] = string.Format("Image was Added Successfully");
        }

        return Json(Convert.ToString(vReturnImagePath));
    }

    [HttpPost]
    public JsonResult UploadUser(IFormFile image)
    {
        var vReturnImagePath = string.Empty;
        if (image.Length > 0)
        {
            var uploads = Path.Combine(_appEnvironment.WebRootPath, "Attachments/Images/Avatars");
            var uniqueName = Guid.NewGuid().ToString() + "_" + image.FileName;
            using (var fileStream = new FileStream(Path.Combine(uploads, uniqueName), FileMode.Create))
            {
                image.CopyTo(fileStream);
            }

            vReturnImagePath = "/Attachments/Images/Avatars/" + uniqueName;
            //here to add Image Path to You Database ,  
            TempData["message"] = string.Format("Image was Added Successfully");
        }

        return Json(Convert.ToString(vReturnImagePath));
    }

    [HttpPost]
    public JsonResult UploadTask(IFormFile image)
    {
        var vReturnImagePath = string.Empty;
        if (image.Length > 0)
        {
            var uploads = Path.Combine(_appEnvironment.WebRootPath, "Attachments/Images/Tasks");
            var uniqueName = Guid.NewGuid().ToString() + "_" + image.FileName;
            using (var fileStream = new FileStream(Path.Combine(uploads, uniqueName), FileMode.Create))
            {
                image.CopyTo(fileStream);
            }

            vReturnImagePath = "/Attachments/Images/Tasks/" + uniqueName;
            //here to add Image Path to You Database ,  
            TempData["message"] = string.Format("Image was Added Successfully");
        }

        return Json(Convert.ToString(vReturnImagePath));
    }

    [HttpPost]
    public JsonResult UploadTarget(IFormFile image)
    {
        var vReturnImagePath = string.Empty;
        if (image.Length > 0)
        {
            var uploads = Path.Combine(_appEnvironment.WebRootPath, "Attachments/Images/Targets");
            var uniqueName = Guid.NewGuid().ToString() + "_" + image.FileName;
            using (var fileStream = new FileStream(Path.Combine(uploads, uniqueName), FileMode.Create))
            {
                image.CopyTo(fileStream);
            }

            vReturnImagePath = "/Attachments/Images/Targets/" + uniqueName;
            //here to add Image Path to You Database ,  
            TempData["message"] = string.Format("Image was Added Successfully");
        }

        return Json(Convert.ToString(vReturnImagePath));
    }

    [HttpPost]
    public JsonResult UploadVuln(IFormFile image)
    {
        var vReturnImagePath = string.Empty;
        if (image.Length > 0)
        {
            var uploads = Path.Combine(_appEnvironment.WebRootPath, "Attachments/Images/Vulns");
            var uniqueName = Guid.NewGuid().ToString() + "_" + image.FileName;
            using (var fileStream = new FileStream(Path.Combine(uploads, uniqueName), FileMode.Create))
            {
                image.CopyTo(fileStream);
            }

            vReturnImagePath = "/Attachments/Images/Vulns/" + uniqueName;
            //here to add Image Path to You Database ,  
            TempData["message"] = string.Format("Image was Added Successfully");
        }

        return Json(Convert.ToString(vReturnImagePath));
    }
    
    [HttpPost]
    public JsonResult UploadVault(IFormFile image)
    {
        var vReturnImagePath = string.Empty;
        if (image.Length > 0)
        {
            var uploads = Path.Combine(_appEnvironment.WebRootPath, "Attachments/Images/Vault");
            var uniqueName = Guid.NewGuid().ToString() + "_" + image.FileName;
            using (var fileStream = new FileStream(Path.Combine(uploads, uniqueName), FileMode.Create))
            {
                image.CopyTo(fileStream);
            }

            vReturnImagePath = "/Attachments/Images/Vault/" + uniqueName;
            //here to add Image Path to You Database ,  
            TempData["message"] = string.Format("Image was Added Successfully");
        }

        return Json(Convert.ToString(vReturnImagePath));
    }
}