using DecisionPlatformWeb.Config;
using DecisionPlatformWeb.Entity;
using DecisionPlatformWeb.Models;
using DecisionPlatformWeb.Service.Parser.MultiCriteriaSolving;
using DecisionPlatformWeb.Service.Solver;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace DecisionPlatformWeb.Controllers;

public class SolvingController : Controller
{
    private readonly MathModelParser _mmParser;
    private readonly OneStepMethodsParser _osmParser;
    private readonly MultiStepMethodsParser _msmParser;
    private readonly IMemoryCache _memoryCache;
    private readonly int _cacheMinutes;

    private const string oneStepTaskType = "1";
    private const string multiStepTaskType = "2";

    public SolvingController(
        IOptions<MultiCriteriaSolvingConfig> config,
        IMemoryCache memoryCache)
    {
        _cacheMinutes = config.Value.CacheTimeout;

        var factory = new CriteriaRelationParser(config.Value);

        _mmParser = new MathModelParser(factory);
        _osmParser = new OneStepMethodsParser(config);
        _msmParser = new MultiStepMethodsParser(config);

        _memoryCache = memoryCache;
    }

    [HttpPost("solve-task")]
    public IActionResult SolveTask([FromBody] TaskCondition taskCondition)
    {
        if (taskCondition.MethodInfo.Type == oneStepTaskType)
        {
            OneStepSolver solver = new OneStepSolver(_mmParser, _osmParser);
            var solveProcess = solver.Solve(taskCondition);

            return PartialView("_OneStepSolveResult", solveProcess);
        }

        if (taskCondition.MethodInfo.Type == multiStepTaskType)
        {
            MultiStepSolver solver = new MultiStepSolver(_mmParser, _msmParser);
            var solveProcess = solver.Solve(taskCondition);

            Guid guid = Guid.NewGuid();
            _memoryCache.Set(guid, solver, TimeSpan.FromMinutes(_cacheMinutes));
            solveProcess.Guid = guid;

            return PartialView("_MultiStepSolveResult", solveProcess);
        }

        return PartialView("Error", new ErrorViewModel());
    }

    [HttpPost("stop-solving")]
    public IActionResult StopSolving([FromBody] StopSolvingModel model)
    {
        _memoryCache.Remove(model.id);
        return Json("");
    }

    [HttpPost("make-iteration")]
    public IActionResult MakeIteration([FromBody] MakerInfo makerInfo)
    {
        if (!_memoryCache.TryGetValue(makerInfo.Id, out MultiStepSolver multiStepSolver))
        {
            throw new Exception("cache expired");
        }

        MultiSolveProcess process = multiStepSolver.MakeIteration(makerInfo);
        _memoryCache.Set(makerInfo.Id, multiStepSolver, TimeSpan.FromMinutes(_cacheMinutes));
        return PartialView("_MultiProtocolResult", process);
    }
}