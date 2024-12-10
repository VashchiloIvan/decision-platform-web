using DecisionPlatformWeb.Entity;
using DecisionPlatformWeb.Service.Parser;

namespace DecisionPlatformWeb.Service.Solver;

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
        
        Console.WriteLine(mathModel.ToString());
        
        var solver = new OneStepMultiCriteriaMethodSolver(mathModel, criteriaRelation);
        solver.addMethods(methods);

        var methodsResults = solver.solve();

        var solveProcess = new SolveProcess();

        var i = 0;
        foreach (var methodsResult in methodsResults)
        {
            solveProcess.Processes.Add(new MethodProcess(){
                Method = condition.MethodInfo.Methods[i],
                Process = methodsResult.second.getProcess(),
                Result = methodsResult.second.getBestEstimateVector().getName()
            });
            
            i++;
        }

        return solveProcess;
    }
}