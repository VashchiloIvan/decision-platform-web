using System.Diagnostics;
using DecisionPlatformWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace DecisionPlatformWeb.Controllers;

// RedirectController - Контроллер, отвечающий за редиректы по страницам интерфейса
public class RedirectController : Controller
{
    private readonly ILogger<RedirectController> _logger;

    public RedirectController(ILogger<RedirectController> logger)
    {
        _logger = logger;
    }

    public IActionResult MultiCriteriaSolving()
    {
        _logger.LogInformation("Redirect: MultiCriteriaSolving");
        
        return View();
    }
    
    public IActionResult Documentation()
    {
        _logger.LogInformation("Redirect: Documentation");
        
        return View();
    }
    
    public IActionResult NaturalUncertainty()
    {
        _logger.LogInformation("Redirect: NaturalUncertainty");
        
        return View();
    }
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        _logger.LogError($"Redirect: Error: {HttpContext.TraceIdentifier}");
        
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}