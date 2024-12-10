using DecisionPlatformWeb.Config;
using DecisionPlatformWeb.Entity.Inner;
using DecisionPlatformWeb.Exceptions;
using Microsoft.Extensions.Options;

namespace DecisionPlatformWeb.Service.Parser;

public class OneStepMethodsParser
{
    private const string lexicographicOptimization = "Метод лексикографической оптимизации";
    private const string criteriaAggregationMethod = "Метод свертки критериев";
    
    private readonly NormalizerParser _normalizerParser;
    private readonly AggregationMethodParser _aggregationMethodParser;

    public OneStepMethodsParser(IOptions<MultiCriteriaSolvingConfig> config)
    {
        var cfg = config.Value;
        _normalizerParser = new NormalizerParser(cfg.OptionalMethods);
        _aggregationMethodParser = new AggregationMethodParser(cfg.OptionalMethods);

        var supported = new[]
        {
            lexicographicOptimization, criteriaAggregationMethod
        };

        var configMethods = cfg.OneStepMethods.Select(m => m.Method).ToArray();

        if (ConfigChecker.IsValid(supported, configMethods))
        {
            throw new ConfigException("unsupported methods on OneStepMethods section config");
        }
    }
    public OneStepMethods Parse(MathModel model, CriteriaRelation relation, MethodInfo methodInfo)
    {
        OneStepMethods methods = new OneStepMethods();

        foreach (var info in methodInfo.Methods)
        {
            methods.Add(parse(model, relation, info));
        }

        return methods;
    }

    private OneStepMethod parse(MathModel model, CriteriaRelation relation, Method methodInfo)
    {
        OneStepMethod method;
        
        switch (methodInfo.Name)
        {
            case lexicographicOptimization:
                method = parseLexicographicOptimization();
                
                break;
            case criteriaAggregationMethod:
                method = parseCriteriaAggregationMethod(methodInfo);
                
                break;
            default:
                throw new InvalidDataException($"unsupported method: {methodInfo.Name}");
        }
        
        method.setMathModel(model);
        method.setCriteriaRelation(relation);
        method.withSolveProcessLog();

        return method;
    }
    private LexicographicOptimization parseLexicographicOptimization()
    {
        LexicographicOptimization method = new LexicographicOptimization();
        
        return method;
    }
    private CriteriaAggregationMethod parseCriteriaAggregationMethod(Method methodInfo)
    {
        var normalizer = _normalizerParser.Parse(methodInfo.AdditionalMethods);
        var aggregationMethod = _aggregationMethodParser.Parse(methodInfo.AdditionalMethods);
        
        CriteriaAggregationMethod method = new CriteriaAggregationMethod(aggregationMethod, normalizer);

        return method;
    }
}