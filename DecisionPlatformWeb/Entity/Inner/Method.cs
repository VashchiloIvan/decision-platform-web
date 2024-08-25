using DecisionPlatformWeb.Entity.Inner;

namespace DecisionPlatformWeb.Entity.Inner;

public class Method
{
    public string Name { get; set; }
    public AdditionalMethod[] AdditionalMethods { get; set; }
}