using DecisionPlatformWeb.Config.MultiCriteriaConfigComponents;

namespace DecisionPlatformWeb.Config;

public class MultiCriteriaSolvingConfig
{
    public int CacheTimeout { get; set; }
    public List<Option> CriteriaRelations { get; set; }
    public List<OptionalMethodConfig> OptionalMethods { get; set; }
    public List<DecisionMethodConfig> OneStepMethods { get; set; }
    public List<DecisionMethodConfig> MultiStepMethods { get; set; }
}