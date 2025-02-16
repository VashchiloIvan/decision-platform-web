using DecisionPlatformWeb.Entity;
using DecisionPlatformWeb.Entity.Inner;
using DecisionPlatformWeb.Service.Parser;

namespace DecisionPlatformWeb.Service.Solver;

public class MultiStepSolver
{
    private readonly MathModelParser _mmParser;
    private readonly MultiStepMethodsParser _msmParser;

    private MultiStepMultiCriteriaMethodSolver _solver;
    private MathModel _mathModel;
    private CriteriaRelation _relation;
    private Method _method;

    public MultiStepSolver(MathModelParser mmParser, MultiStepMethodsParser msmParser)
    {
        _mmParser = mmParser;
        _msmParser = msmParser;
    }

    public MultiSolveProcess Solve(TaskCondition condition)
    {
        _mathModel = _mmParser.Parse(condition.CriteriaList, condition.AlternativeList);
        _relation = _mmParser.Parse(_mathModel, condition.CriteriaRelation);
        
        // TODO exception om len == 0
        _method = condition.MethodInfo.Methods[0];
        var method = _msmParser.Parse(_mathModel, _relation, _method);

        _solver = new MultiStepMultiCriteriaMethodSolver(_mathModel, _relation);
        _solver.setMethod(method);

        MultiMethodResult result = _solver.solve();

        return new MultiSolveProcess
        {
            Guid = default,
            Method = _method,
            Process = result.second.getProcess(),
            IsFinished = result.first.getStatus() == DecisionStatus.Optimal ||
                         result.first.getStatus() == DecisionStatus.Feasible ||
                         result.first.getStatus() == DecisionStatus.Infeasible,
            IsValidModel = result.first.getStatus() != DecisionStatus.InvalidTaskModel,
            Controls = _msmParser.GetControls(_method),
        };
    }

    public MultiSolveProcess MakeIteration(MakerInfo info)
    {
        var makerInfo = _msmParser.Parse(_method, info);
        
        MultiMethodResult result = _solver.makeIteration(makerInfo);

        var process = result.second.getProcess();
        foreach (var taskStep in process)
        {
            Console.WriteLine(taskStep.AsString());
        }
        
        return new MultiSolveProcess
        {
            Guid = info.Id,
            Method = _method,
            Process = process,
            IsFinished = result.first.getStatus() == DecisionStatus.Optimal ||
                         result.first.getStatus() == DecisionStatus.Feasible ||
                         result.first.getStatus() == DecisionStatus.Infeasible,
            IsValidModel = true,
            Controls = _msmParser.GetControls(_method),
        };
    }
}