using DecisionPlatformWeb.Config.MultiCriteriaConfigComponents;

namespace DecisionPlatformWeb.Controllers;

public class SupportedMethodData
{
    public string Type { get; set; }
    
    public List<Option> SelectList { get; set; } = new();
}

public class SupportedMethods
{
    public Dictionary<string, Dictionary<string, SupportedMethodData>> OneStepMethodInfo { get; set; } = new();
    public Dictionary<string, Dictionary<string, SupportedMethodData>> MultiStepMethodInfo { get; set; } = new();
}