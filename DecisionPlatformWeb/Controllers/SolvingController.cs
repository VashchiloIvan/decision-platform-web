using DecisionPlatformWeb.Config;
using DecisionPlatformWeb.Entity;
using DecisionPlatformWeb.Models;
using DecisionPlatformWeb.Service.Cache;
using DecisionPlatformWeb.Service.Parser;
using DecisionPlatformWeb.Service.Parser.MultiCriteriaSolving;
using DecisionPlatformWeb.Service.Solver;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DecisionPlatformWeb.Controllers;

// SolvingController - контроллер, содержаший ручки
// принятия решений
public class SolvingController : Controller
{
    private readonly MathModelParser _mmParser;
    private readonly OneStepMethodsParser _osmParser;
    private readonly MultiStepMethodsParser _msmParser;

    private readonly Cache _cache;

    private const string oneStepTaskType = "1";
    private const string multiStepTaskType = "2";

    public SolvingController(IOptions<MultiCriteriaSolvingConfig> config, Cache cache)
    {
        var factory = new CriteriaRelationParser(config.Value);
        
        _mmParser = new MathModelParser(factory);
        _osmParser = new OneStepMethodsParser(config);
        _msmParser = new MultiStepMethodsParser(config);

        _cache = cache;
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
            
            Guid guid = _cache.AddObject(solver);
            solveProcess.Guid = guid;

            return PartialView("_MultiStepSolveResult", solveProcess);
        }

        return PartialView("Error", new ErrorViewModel());
    }
    
    [HttpPost("stop-solving")]
    public IActionResult StopSolving([FromBody] StopSolvingModel model)
    {
        _cache.RemoveObject(model.id);
        return Json("aboba");
    }

    [HttpPost("make-iteration")]
    public IActionResult MakeIteration([FromBody] MakerInfo makerInfo)
    {
        var multiStepSolver = _cache.GetObject(makerInfo.Id);
        if (multiStepSolver == null)
        {
            // cache expired
            throw new Exception("cache expired");
        }

        MultiSolveProcess process = multiStepSolver.MakeIteration(makerInfo);
        _cache.UpdateObject(makerInfo.Id, multiStepSolver);
        return PartialView("_MultiProtocolResult", process);
    }

    // [HttpGet("is-solving-stopped")]
    // public IActionResult IsSolvingStopped([FromQuery] Guid guid)
    // {
    //     MultiStepResult multiStepResult = _methodResultStorage.getMultiStepResult(guid);
    //     if (multiStepResult == null)
    //         return Json(true);
    //     if (multiStepResult.IsSolutionComplete)
    //         _methodResultStorage.removeMultiStepResult(multiStepResult);
    //     return Json(multiStepResult.IsSolutionComplete);
    // }
    // private MathModel getMathModelFromUiObjects(InnerCriteria[] criteriaArray, Alternative[] alternatives,
    //     out Dictionary<string, int> dictionary)
    // {
    //     dictionary = new Dictionary<string, int>();
    //
    //     CriteriaType StringToCriteriaType(string type) =>
    //         type == "max" ? CriteriaType.MAXIMIZATION : CriteriaType.MINIMIZATION;
    //
    //     List<Criteria> criteriaList = new List<Criteria>();
    //     List<EstimateVector> estimateVectors = new List<EstimateVector>();
    //     int id = 1;
    //     foreach (InnerCriteria innerCriteria in criteriaArray)
    //     {
    //         dictionary.Add(innerCriteria.Name, id);
    //         criteriaList.Add(new Criteria(id++, innerCriteria.Name, StringToCriteriaType(innerCriteria.Type)));
    //     }
    //
    //     id = 1;
    //     int criteriaCount = criteriaList.Count;
    //     foreach (var alternative in alternatives)
    //     {
    //         estimateVectors.Add(new EstimateVector(
    //             id++,
    //             alternative.Name,
    //             alternative.Marks,
    //             criteriaCount
    //         ));
    //     }
    //
    //     return new MathModel(criteriaList.ToArray(), estimateVectors.ToArray());
    // }
    //
    // private ICriteriaRelation getCriteriaRelationFromUiObject(CriteriaRelation criteriaRelation,
    //     Dictionary<string, int> criteriaIdMap)
    // {
    //     CriteriaConstraint toCriteriaConstraint(string criteriaConstraint)
    //     {
    //         switch (criteriaConstraint)
    //         {
    //             case ">":
    //                 return CriteriaConstraint.More;
    //             case ">=":
    //                 return CriteriaConstraint.MoreOrEquivalent;
    //             case "<":
    //                 return CriteriaConstraint.Less;
    //             case "<=":
    //                 return CriteriaConstraint.LessOrEquivalent;
    //             default:
    //                 return CriteriaConstraint.Equivalent;
    //         }
    //     }
    //
    //     Type relationType = _criteriaRelationReferenceInfo.Values[criteriaRelation.Name];
    //     int criteriaCount = criteriaIdMap.Count;
    //
    //     if (relationType == typeof(SimpleCriteriaRelation))
    //     {
    //         List<TwoCriteriaRelation> relations = new List<TwoCriteriaRelation>();
    //         foreach (CriteriaInfo criteriaInfo in criteriaRelation.Info)
    //         {
    //             relations.Add(new TwoCriteriaRelation(
    //                 criteriaIdMap[criteriaInfo.First],
    //                 toCriteriaConstraint(criteriaInfo.Constraint),
    //                 criteriaIdMap[criteriaInfo.Second]
    //             ));
    //         }
    //
    //         return new SimpleCriteriaRelation(relations.ToArray(), criteriaCount);
    //     }
    //
    //     if (relationType == typeof(AllCriteriaRelation))
    //     {
    //         CriteriaInfo[] criteriaInfos = criteriaRelation.Info.OrderBy(cR => cR.Value).ToArray();
    //         var idSeq = new List<int>();
    //         foreach (CriteriaInfo criteriaInfo in criteriaInfos)
    //         {
    //             idSeq.Add(criteriaIdMap[criteriaInfo.CriteriaName]);
    //         }
    //
    //         return new AllCriteriaRelation(criteriaCount, idSeq.ToArray());
    //     }
    //
    //     if (relationType == typeof(SimpleRankingMethod) || relationType == typeof(ProportionalMethod))
    //     {
    //         Dictionary<int, double> map = new Dictionary<int, double>();
    //         foreach (CriteriaInfo criteriaInfo in criteriaRelation.Info)
    //         {
    //             map.Add(criteriaIdMap[criteriaInfo.CriteriaName], (double)criteriaInfo.Value);
    //         }
    //
    //         return relationType == typeof(SimpleRankingMethod)
    //             ? new SimpleRankingMethod(criteriaCount, map)
    //             : new ProportionalMethod(criteriaCount, map);
    //     }
    //
    //     throw new ArgumentException("Информация о важности критериев не поддерживается!");
    // }
    //
    // private List<AbstractOneStepMethod> getOneStepMethodsFromUiObject(Method[] methods)
    // {
    //     var oneStepMethods = new List<AbstractOneStepMethod>();
    //     foreach (Method method in methods)
    //     {
    //         Type methodType = _oneStepReferenceInfo.Values[method.Name];
    //         if (methodType == typeof(LexicographicOptimization))
    //         {
    //             oneStepMethods.Add(new LexicographicOptimization());
    //         }
    //
    //         if (methodType == typeof(CriteriaAggregationMethod))
    //         {
    //             AbstractAggregationOperator aggregationOperator = null;
    //             INormalizer normalizer = null;
    //             foreach (AdditionalMethod additionalMethod in method.AdditionalMethods)
    //             {
    //                 if (_aggregationOperatorReferenceInfo.Name.Equals(additionalMethod.Name))
    //                 {
    //                     Type type = _aggregationOperatorReferenceInfo.Values[additionalMethod.Value];
    //                     if (type == typeof(AddictiveAggregationOperator))
    //                         aggregationOperator = new AddictiveAggregationOperator();
    //                     if (type == typeof(MultiplicativeAggregationOperator))
    //                         aggregationOperator = new MultiplicativeAggregationOperator();
    //                     if (type == typeof(IdealDistanceAggregationOperator))
    //                         aggregationOperator = new IdealDistanceAggregationOperator();
    //                 }
    //
    //                 if (_normalizerReferenceInfo.Name.Equals(additionalMethod.Name))
    //                 {
    //                     Type type = _normalizerReferenceInfo.Values[additionalMethod.Value];
    //                     if (type == typeof(MinMaxNormalizer))
    //                         normalizer = new MinMaxNormalizer();
    //                 }
    //             }
    //
    //             oneStepMethods.Add(new CriteriaAggregationMethod(aggregationOperator, normalizer));
    //         }
    //     }
    //
    //     return oneStepMethods;
    // }
    //
    // private AbstractMultiStepMethod getMultiStepMethodFromUiObject(Method method)
    // {
    //     Type methodType = _multiStepReferenceInfo.Values[method.Name];
    //     AbstractMultiStepMethod resultMethod = null;
    //
    //     if (methodType == typeof(SuccessiveConcessionsMethod))
    //     {
    //         resultMethod = new SuccessiveConcessionsMethod();
    //     }
    //
    //     return resultMethod;
    // }
}
