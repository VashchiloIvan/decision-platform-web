using System.Diagnostics;
using DecisionPlatformWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace DecisionPlatformWeb.Controllers;

// HomeController - Контроллер, отвечающий за редиректы по страницам интерфейса
// TODO rename to RedirectController
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        _logger.LogInformation("Redirect: Index");
        
        return View();
    }
    
    public IActionResult Privacy()
    {
        _logger.LogInformation("Redirect: Privacy");
        
        return View();
    }
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        _logger.LogError($"Redirect: Error: {HttpContext.TraceIdentifier}");
        
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}