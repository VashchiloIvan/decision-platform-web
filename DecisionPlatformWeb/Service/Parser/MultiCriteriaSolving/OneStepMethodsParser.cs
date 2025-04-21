using DecisionPlatformWeb.Config;
using DecisionPlatformWeb.Entity.Inner;
using DecisionPlatformWeb.Exceptions;
using MultiCriteriaCsharpApi;
using Microsoft.Extensions.Options;

namespace DecisionPlatformWeb.Service.Parser.MultiCriteriaSolving;

public class OneStepMethodsParser
{
    private const string lexicographicOptimization = "Метод лексикографической оптимизации";
    private const string criteriaAggregationMethod = "Метод свертки критериев";
    private const string smartMethod = "Smart";
    private const string prometheeMethod = "Promethee I - III";

    private readonly NormalizerParser _normalizerParser;
    private readonly AggregationMethodParser _aggregationMethodParser;
    private readonly MinMaxValuesParser _minMaxValuesParser;
    private readonly PrometheeCheckedMethodsParser _prometheeCheckedMethodsParser;
    private readonly CriteriaFunctionTypesParser _criteriaFunctionTypesParser;
    private readonly ScaleFactorParser _scaleFactorParser;

    public OneStepMethodsParser(IOptions<MultiCriteriaSolvingConfig> config)
    {
        var cfg = config.Value;
        _normalizerParser = new NormalizerParser(cfg.OptionalMethods);
        _aggregationMethodParser = new AggregationMethodParser(cfg.OptionalMethods);
        _minMaxValuesParser = new MinMaxValuesParser(cfg.OptionalMethods);
        _prometheeCheckedMethodsParser = new PrometheeCheckedMethodsParser(cfg.OptionalMethods);
        _criteriaFunctionTypesParser = new CriteriaFunctionTypesParser(cfg.OptionalMethods);
        _scaleFactorParser = new ScaleFactorParser(cfg.OptionalMethods);

        var supported = new[]
        {
            lexicographicOptimization, criteriaAggregationMethod, smartMethod, prometheeMethod
        };

        var configMethods = cfg.OneStepMethods.Select(m => m.Method).ToArray();

        if (ConfigChecker.IsValid(supported, configMethods))
        {
            throw new ConfigException("unsupported methods on OneStepMethods section config");
        }
    }

    public OneStepMethods Parse(MathModel model, MultiCriteriaCsharpApi.CriteriaRelation relation, MethodInfo methodInfo)
    {
        OneStepMethods methods = new OneStepMethods();

        foreach (var info in methodInfo.Methods)
        {
            methods.Add(parse(model, relation, info));
        }

        return methods;
    }

    private OneStepMethod parse(MathModel model, MultiCriteriaCsharpApi.CriteriaRelation relation, Method methodInfo)
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
            case smartMethod:
                method = parseSmartMethod(methodInfo);

                break;
            case prometheeMethod:
                method = parsePrometheeMethod(methodInfo);

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

    private Smart parseSmartMethod(Method methodInfo)
    {
        var minMaxCriteriaValues = _minMaxValuesParser.Parse(methodInfo.AdditionalMethods);

        Smart method = new Smart();

        method.setMinMaxCriteriaValues(minMaxCriteriaValues);

        return method;
    }

    private PrometheeAllVersions parsePrometheeMethod(Method methodInfo)
    {
        PrometheeAllVersions method = new PrometheeAllVersions();

        var list = _criteriaFunctionTypesParser.Parse(methodInfo.AdditionalMethods);
        var prometheeCheckedMethods = _prometheeCheckedMethodsParser.Parse(methodInfo.AdditionalMethods);
        var scaleFactor = _scaleFactorParser.Parse(methodInfo.AdditionalMethods);

        method.setCriterionTypes(new CriterionTypes(list));
        method.useMethodVersions(
            prometheeCheckedMethods.usePrometheeI,
            prometheeCheckedMethods.usePrometheeII,
            prometheeCheckedMethods.usePrometheeIII);
        method.setScaleFactor(scaleFactor);

        return method;
    }
}