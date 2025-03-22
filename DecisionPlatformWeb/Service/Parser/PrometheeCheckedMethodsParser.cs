using DecisionPlatformWeb.Config.MultiCriteriaConfigComponents;
using DecisionPlatformWeb.Entity.Inner;
using DecisionPlatformWeb.Exceptions;

namespace DecisionPlatformWeb.Service.Parser;

public class PrometheeCheckedMethods
{
    public bool usePrometheeI, usePrometheeII, usePrometheeIII;
}

public class PrometheeCheckedMethodsParser
{
    private readonly string configKey = "Версии метода Promethee";
    private readonly Dictionary<string, int> availableMethods = new();
    
    public PrometheeCheckedMethodsParser(List<OptionalMethodConfig> config)
    {
        OptionalMethodConfig? currentConfig = config.FirstOrDefault(methodConfig => methodConfig.Name == configKey);
        if (currentConfig == null)
        {
            throw new ConfigException($"failed to parse {configKey} section of config");
        }
        
        foreach (var currentConfigMethod in currentConfig.Methods)
        {
            availableMethods[currentConfigMethod.Name] = 1;
        }
    }

    public PrometheeCheckedMethods Parse(AdditionalMethod[] info)
    {
        AdditionalMethod? section = info.FirstOrDefault(additionalMethod => additionalMethod.Name == configKey);
        if (section == null)
        {
            throw new FailedToParseException("failed to parse promethee checked methods");    
        }

        var values = new PrometheeCheckedMethods();
        foreach (var version in section.Value.Split(", "))
        {
            if (availableMethods[version] != 1)
            {
                continue;
            }

            switch (version)
            {
                case "Promethee I":
                    values.usePrometheeI = true;
                    break;
                case "Promethee II":
                    values.usePrometheeII = true;
                    break;
                case "Promethee III":
                    values.usePrometheeIII = true;
                    break;
            }
        }

        return values;
    }
}