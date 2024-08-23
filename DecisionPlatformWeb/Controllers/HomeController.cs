using System.Diagnostics;
using DecisionPlatformWeb.Config;
using DecisionPlatformWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DecisionPlatformWeb.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly MultiCriteriaSolvingConfig _config;
    private readonly SupportedMethods _supportedMethods;

    public HomeController(ILogger<HomeController> logger, 
        IOptions<MultiCriteriaSolvingConfig> config)
    {
        _logger = logger;
        _config = config.Value;

        this._supportedMethods = new SupportedMethods();
        this.InitSupportedMethods();
    }

    public IActionResult Index()
    {
        _logger.LogDebug("Request: Index");
        
        return View();
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

    // [HttpPost("export-xml")]
    // public IActionResult ExportXml([FromBody] TaskCondition taskCondition)
    // {
    //     var stream = new MemoryStream();
    //     XmlDocument document = new XmlDocument();
    //     XmlNode xmlNode = XmlRepositoryUtils.ObjectToXml(document, taskCondition);
    //     document.AppendChild(xmlNode);
    //     document.Save(stream);
    //
    //     stream.Position = 0;
    //     var a = new FileStreamResult(stream, "text/plain")
    //     {
    //         FileDownloadName = "model.xml"
    //     };
    //     return a;
    // }
    //
    // [HttpPost("export-json")]
    // public IActionResult ExportJson([FromBody] TaskCondition taskCondition)
    // {
    //     MemoryStream stream = new MemoryStream();
    //     JsonSerializer.Serialize(stream, taskCondition);
    //
    //     stream.Position = 0;
    //     var a = new FileStreamResult(stream, "text/plain")
    //     {
    //         FileDownloadName = "model.json"
    //     };
    //     return a;
    // }
    //
    // [HttpPost("import")]
    // public IActionResult Import(IFormFile file)
    // {
    //     int nameLength = file.FileName.Length;
    //     if (file.FileName[(nameLength - 4)..].Equals(".xml"))
    //     {
    //         var stream = file.OpenReadStream();
    //         XmlDocument document = new XmlDocument();
    //         document.Load(stream);
    //         TaskCondition taskCondition = XmlRepositoryUtils.XmlNodeToObject<TaskCondition>(document.SelectSingleNode("TaskCondition"),
    //             "TaskCondition");
    //         return Json(taskCondition);
    //     }
    //
    //     if (file.FileName[(nameLength - 5)..].Equals(".json"))
    //     {
    //         TaskCondition? taskCondition = JsonSerializer.Deserialize<TaskCondition>(file.OpenReadStream());
    //         return Json(taskCondition);
    //     }
    //     
    //     return Json(new TaskCondition());
    // }

    // [HttpPost("solve-task")]
    // public IActionResult SolveTask([FromBody] TaskCondition taskCondition)
    // {
    //     return Json("");
    //     
    //     // return PartialView("_FieldsDontFilled");
    // }

    public IActionResult Privacy()
    {
        return View();
    }

    // [HttpPost("make-iteration")]
    // public IActionResult MakeIteration([FromBody] DecisionMakerInfo makerInfo)
    // {
    //     MultiStepResult multiStepResult = _methodResultStorage.getMultiStepResult(makerInfo.Guid);
    //     var decisionMakerInfo = DecisionMakerInfoFactory.CreateDecisionMakerInfo(makerInfo);
    //     List<string> protocol = multiStepResult.MakeIteration(decisionMakerInfo);
    //     return PartialView("_ProtocolResult", protocol);
    // }
        
    // [HttpPost("stop-solving")]
    // public IActionResult StopSolving([FromBody] DecisionMakerInfo makerInfo)
    // {
    //     MultiStepResult multiStepResult = _methodResultStorage.getMultiStepResult(makerInfo.Guid);
    //     _methodResultStorage.removeMultiStepResult(multiStepResult);
    //     return Json("aboba");
    // }

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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

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

    private void InitSupportedMethods()
    {
        foreach (var method in _config.OneStepMethods)
        {
            Dictionary<string, List<string>> optional = new Dictionary<string, List<string>>();

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
            Dictionary<string, List<string>> optional = new Dictionary<string, List<string>>();

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