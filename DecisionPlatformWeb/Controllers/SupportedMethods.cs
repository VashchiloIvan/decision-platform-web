namespace DecisionPlatformWeb.Controllers;

public class SupportedMethods
{
    public Dictionary<string, Dictionary<string, List<string>>> OneStepMethodInfo { get; set; }
    public Dictionary<string, Dictionary<string, List<string>>> MultiStepMethodInfo { get; set; }

    public SupportedMethods()
    {
        this.OneStepMethodInfo = new Dictionary<string, Dictionary<string, List<string>>>();
        this.MultiStepMethodInfo = new Dictionary<string, Dictionary<string, List<string>>>();
    }
}