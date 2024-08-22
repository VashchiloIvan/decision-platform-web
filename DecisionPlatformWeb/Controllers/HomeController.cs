using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DecisionPlatformWeb.Config;
using DecisionPlatformWeb.Models;
using Microsoft.Extensions.Options;

namespace DecisionPlatformWeb.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly MultiCriteriaSolvingConfig _criteriaSolvingConfig;

    public HomeController(ILogger<HomeController> logger, IOptions<MultiCriteriaSolvingConfig> criteriaSolvingConfig)
    {
        _logger = logger;
        _criteriaSolvingConfig = criteriaSolvingConfig.Value;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}