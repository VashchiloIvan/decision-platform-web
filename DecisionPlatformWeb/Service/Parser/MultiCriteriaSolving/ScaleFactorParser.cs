using DecisionPlatformWeb.Config.MultiCriteriaConfigComponents;
using DecisionPlatformWeb.Entity.Inner;
using DecisionPlatformWeb.Exceptions;
using MultiCriteriaCsharpApi;

namespace DecisionPlatformWeb.Service.Parser.MultiCriteriaSolving;

public class ScaleFactorParser
{
    private readonly string configKey = "Масштабный фактор (для PROMETHEE III)";

    public ScaleFactorParser(List<OptionalMethodConfig> config)
    {
        OptionalMethodConfig? currentConfig = config.FirstOrDefault(methodConfig => methodConfig.Name == configKey);
        if (currentConfig == null)
        {
            throw new ConfigException($"failed to parse {configKey} section of config");
        }
    }
    
    public double Parse(AdditionalMethod[] info)
    {
        AdditionalMethod? section = info.FirstOrDefault(additionalMethod => additionalMethod.Name == configKey);
        if (section == null)
        {
            throw new FailedToParseException("failed to parse criteria function types");
        }

        if (section.Value == "")
        {
            return 0;
        }

        var factor = double.Parse(section.Value.Replace(".", ","));

        return factor;
    }
}