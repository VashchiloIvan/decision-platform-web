using DecisionPlatformWeb.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DecisionPlatformWeb.Controllers;

// SupportController - контроллер, поддерживающий получение инфофрмации на интерфейсе
public class SupportController : Controller
{
    private readonly ILogger<SupportController> _logger;
    private readonly MultiCriteriaSolvingConfig _config;
    private readonly SupportedMethods _supportedMethods;
    
    public SupportController(
        ILogger<SupportController> logger, 
        IOptions<MultiCriteriaSolvingConfig> config)
    {
        _logger = logger;
        _config = config.Value;
        
        this._supportedMethods = new SupportedMethods();
        this.InitSupportedMethods();
    }

    [HttpGet("criteria-relation-info")]
    public IActionResult GetCriteriaRelationInfo()
    {
        _logger.LogDebug("Request: GetCriteriaRelationInfo");
        
        var convertAll = _config.CriteriaRelations.ConvertAll(x => x.Name);
        
        _logger.LogDebug($"Response: {convertAll}");
        
        return Json(convertAll);
    }
    
    [HttpGet("decision-method-info")]
    public IActionResult GetDecisionMethodInfo()
    {
        _logger.LogDebug("Request: GetCriteriaRelationInfo");
        
        _logger.LogDebug($"Response: {_supportedMethods}");
        
        return Json(_supportedMethods);
    }
    
    private void InitSupportedMethods()
    {
        foreach (var method in _config.OneStepMethods)
        {
            var optional = new Dictionary<string, List<string>>();

            if (method.OptionalMethods != null)
            {
                foreach (var opt in method.OptionalMethods)
                {
                    var optCfg = _config.OptionalMethods.Find(x => x.Key == opt);

                    if (optCfg != null)
                    {
                        var methodsList = optCfg.Methods.ConvertAll(x => x.Name);
                        optional.Add(optCfg.Name, methodsList);

                        continue;
                    }

                    _logger.LogError("Failed to initialize supported method list");
                    throw new ApplicationException("Wrong config");
                }
            }

            _supportedMethods.OneStepMethodInfo.Add(method.Name, optional);
        }
        
        foreach (var method in _config.MultiStepMethods)
        {
            var optional = new Dictionary<string, List<string>>();

            if (method.OptionalMethods != null)
            {
                foreach (var opt in method.OptionalMethods)
                {
                    var optCfg = _config.OptionalMethods.Find(x => x.Key == opt);

                    if (optCfg != null)
                    {
                        var methodsList = optCfg.Methods.ConvertAll(x => x.Name);
                        optional.Add(optCfg.Name, methodsList);

                        continue;
                    }

                    _logger.LogError("Failed to initialize supported method list");
                    throw new ApplicationException("Wrong config");
                }
            }

            _supportedMethods.MultiStepMethodInfo.Add(method.Name, optional);
        }
    }
}