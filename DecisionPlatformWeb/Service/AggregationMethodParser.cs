using DecisionPlatformWeb.Config.MultiCriteriaConfigComponents;
using DecisionPlatformWeb.Entity.Inner;
using DecisionPlatformWeb.Exceptions;

namespace DecisionPlatformWeb.Service;

public class AggregationMethodParser
{
    private const string configKey = "Метод свёртки";

    private const string addictiveAggregationOperator = "Аддитивная свёртка";
    private const string multiplicativeAggregationOperator = "Мультипликативная свёртка";
    private const string idealDistanceAggregationOperator = "Свёртка расстояния до идеала";

    public AggregationMethodParser(List<OptionalMethodConfig> config)
    {
        OptionalMethodConfig? currentConfig = config.FirstOrDefault(methodConfig => methodConfig.Name == configKey);
        if (currentConfig == null)
        {
            throw new ConfigException($"failed to parse {configKey} section of config");
        }

        var supportedMethods = new[]
        {
            addictiveAggregationOperator,
            multiplicativeAggregationOperator,
            idealDistanceAggregationOperator
        };

        var configMethods = currentConfig.Methods.Select(m => m.Name).ToArray();

        if (!ConfigChecker.IsValid(supportedMethods, configMethods))
        {
            throw new ConfigException($"config of {configKey} section contains unsupported methods");
        }
    }

    public AggregationOperator Parse(AdditionalMethod[] info)
    {
        AdditionalMethod? section =
            info.FirstOrDefault(additionalMethod => additionalMethod.Name == configKey);
        if (section == null)
        {
            throw new FailedToParseException("failed to parse AggregationOperator");
        }

        switch (section.Value)
        {
            case addictiveAggregationOperator:
                return new AddictiveAggregationOperator();
            case multiplicativeAggregationOperator:
                return new MultiplicativeAggregationOperator();
            case idealDistanceAggregationOperator:
                return new IdealDistanceAggregationOperator();
            default:
                throw new FailedToParseException(
                    $"unsupported AggregationMethod method {section.Value}");
        }
    }
}