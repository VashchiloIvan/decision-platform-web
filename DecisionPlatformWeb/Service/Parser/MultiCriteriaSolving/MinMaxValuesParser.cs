using DecisionPlatformWeb.Config.MultiCriteriaConfigComponents;
using DecisionPlatformWeb.Entity.Inner;
using DecisionPlatformWeb.Exceptions;
using MultiCriteriaCsharpApi;

namespace DecisionPlatformWeb.Service.Parser.MultiCriteriaSolving;

public class MinMaxValuesParser
{
    private readonly string configKey = "Максимальные и минимальные значения по критериям";
    
    public MinMaxValuesParser(List<OptionalMethodConfig> config)
    {
        OptionalMethodConfig? currentConfig = config.FirstOrDefault(methodConfig => methodConfig.Name == configKey);
        if (currentConfig == null)
        {
            throw new ConfigException($"failed to parse {configKey} section of config");
        }
    }
    
    public MinMaxCriteriaValues Parse(AdditionalMethod[] info)
    {
        AdditionalMethod? section = info.FirstOrDefault(additionalMethod => additionalMethod.Name == configKey);
        if (section == null)
        {
            throw new FailedToParseException("failed to parse min-max criteria values");    
        }

        var values = new MinMaxValues();
        foreach (var criteriaWithMinMax in section.Value.Split("; "))
        {
            var trimEnd = criteriaWithMinMax.TrimEnd(']');
            var minMax = trimEnd.Split("[")[1];
            var splitted = minMax.Split(", ");

            var min = double.Parse(splitted[0]);
            var max = double.Parse(splitted[1]);
            
            values.Add(new MinMax(min, max));
        }

        return new MinMaxCriteriaValues(values);
    }
}