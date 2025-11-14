using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

namespace MvcMovie.Controllers;

public class HelloWorldController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Welcome(string name, int count)
    {
        ViewData["Message"] = HtmlEncoder.Default.Encode($"Hello {name}");
        ViewData["Count"] = count;
        return View();
    }
}