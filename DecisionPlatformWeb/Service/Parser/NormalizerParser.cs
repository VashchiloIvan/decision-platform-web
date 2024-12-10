using DecisionPlatformWeb.Config.MultiCriteriaConfigComponents;
using DecisionPlatformWeb.Entity.Inner;
using DecisionPlatformWeb.Exceptions;

namespace DecisionPlatformWeb.Service.Parser;

public class NormalizerParser
{
    private const string configKey = "Нормировщик мат. модели";

    private const string minMaxNormalizerName = "Минимаксный нормировщик";
    public NormalizerParser(List<OptionalMethodConfig> config)
    {
        OptionalMethodConfig? currentConfig = config.FirstOrDefault(methodConfig => methodConfig.Name == configKey);
        if (currentConfig == null)
        {
            throw new ConfigException($"failed to parse {configKey} section of config");
        }

        var supportedMethods = new[]
        {
            minMaxNormalizerName
        };

        var configMethods = currentConfig.Methods.Select(m => m.Name).ToArray();

        if (!ConfigChecker.IsValid(supportedMethods, configMethods)) 
        {
            throw new ConfigException($"config of {configKey} section contains unsupported methods");
        }
    }

    public Normalizer Parse(AdditionalMethod[] info)
    {
        AdditionalMethod? normalizerSection = info.FirstOrDefault(additionalMethod => additionalMethod.Name == configKey);
        if (normalizerSection == null)
        {
            throw new FailedToParseException("failed to parse normalizer");    
        }

        switch (normalizerSection.Value)
        {
            case minMaxNormalizerName:
                return new MinMaxNormalizer();
            default:
                throw new FailedToParseException($"unsupported normalizer method {normalizerSection.Value}");
        }
    }
}