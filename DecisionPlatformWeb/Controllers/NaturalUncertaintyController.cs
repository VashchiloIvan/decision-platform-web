using DecisionPlatformWeb.Config;
using DecisionPlatformWeb.Entity.NaturalUncertainty;
using DecisionPlatformWeb.Models;
using DecisionPlatformWeb.Service.Parser.NaturalUncertainty;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DecisionPlatformWeb.Controllers;

// NaturalUncertaintyController - контроллер, содержаший ручки
// принятия решений в условиях природной неопределенности
public class NaturalUncertaintyController : Controller
{
    private readonly ILogger<RedirectController> _logger;
    private readonly NaturalUncertaintyConfig _config;
    private readonly MathModelParser _mathModelParser;
    private readonly CriteriaParser _criteriaParser;

    public NaturalUncertaintyController(ILogger<RedirectController> logger, IOptions<NaturalUncertaintyConfig> config)
    {
        _logger = logger;
        _config = config.Value;
        _mathModelParser = new MathModelParser();
        _criteriaParser = new CriteriaParser(_config);
    }

    [HttpGet("nu-config")]
    public IActionResult GetNaturalUncertaintyConfig()
    {
        _logger.LogDebug("Request: GetNaturalUncertaintyConfig");

        _logger.LogDebug($"Response: {_config}");

        return Json(_config);
    }

    [HttpPost("nu-solve")]
    public IActionResult SolveTask([FromBody] RequestData taskCondition)
    {
        var parsedMathModel = _mathModelParser.Parse(taskCondition.MathModel);
        var criteriaList = _criteriaParser.Parse(taskCondition.Criterias, parsedMathModel);

        var protocols = new Protocols();
        for (int i = 0; i < criteriaList.Count; i++)
        {
            var criterion = criteriaList[i];
            
            criterion.withLoggingProcess();
            criterion.solve();
            var process = criterion.getProcess();

            var criteriaName = _config.Criterias.First(cr => cr.Method == taskCondition.Criterias[i].CriteriaName).Name;
            var parameters = new List<CriteriaParameter>();
            
            foreach (var parameter in taskCondition.Criterias[i].Parameters)
            {
                parameters.Add(new CriteriaParameter
                {
                    Key = _config.Parameters.First(par => par.Key == parameter.Key).Name,
                    Value = parameter.Value
                });
            }
            
            protocols.SolvingProtocols.Add(new Protocol
            {
                Criteria = new Criteria
                {
                    CriteriaName = criteriaName,
                    Parameters = parameters
                },
                SolvingProtocol = process
            });
        }
        
        return PartialView("_NaturalUncertaintySolveResult", protocols);
    }
}