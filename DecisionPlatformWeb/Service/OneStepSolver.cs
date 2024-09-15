using DecisionPlatformWeb.Entity;
using DecisionPlatformWeb.Exceptions;

namespace DecisionPlatformWeb.Service;

public class OneStepSolver
{
    private readonly MathModelParser _mmParser;
    private readonly OneStepMethodsParser _osmParser;

    public OneStepSolver(MathModelParser mmParser, OneStepMethodsParser osmParser)
    {
        _mmParser = mmParser;
        _osmParser = osmParser;
    }

    public SolveProcess Solve(TaskCondition condition)
    {
        var mathModel = _mmParser.Parse(condition.CriteriaList, condition.AlternativeList);
        var criteriaRelation = _mmParser.Parse(mathModel, condition.CriteriaRelation);

        var methods = _osmParser.Parse(mathModel, criteriaRelation, condition.MethodInfo);

        throw new ConfigException("GOIDA");
    }
}