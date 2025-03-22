using DecisionPlatformWeb.Config;
using DecisionPlatformWeb.Config.MultiCriteriaConfigComponents;
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

        _supportedMethods = new SupportedMethods();
        InitSupportedMethods();
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
            var optional = GetOptional(method);

            _supportedMethods.OneStepMethodInfo.Add(method.Name, optional);
        }

        foreach (var method in _config.MultiStepMethods)
        {
            var optional = GetOptional(method);
            
            _supportedMethods.MultiStepMethodInfo.Add(method.Name, optional);
        }

        Dictionary<string, SupportedMethodData> GetOptional(DecisionMethodConfig method)
        {
            var optional = new Dictionary<string, SupportedMethodData>();

            if (method.OptionalMethods == null)
            {
                return optional;
            }
            
            foreach (var opt in method.OptionalMethods)
            {
                var optCfg = _config.OptionalMethods.Find(x => x.Key == opt);

                if (optCfg != null)
                {
                    var data = new SupportedMethodData
                    {
                        Type = optCfg.Type
                    };
                    
                    if (optCfg.Methods != null)
                    {
                        data.SelectList = optCfg.Methods;
                    }

                    optional.Add(optCfg.Name, data);
                    
                    continue;
                }

                _logger.LogError("Failed to initialize supported method list");
                throw new ApplicationException("Wrong config");
            }
            
            return optional;
        }
    }
}