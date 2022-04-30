using Microsoft.AspNetCore.Mvc;

namespace Cervantes.Web.Controllers;

public class SearchController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}