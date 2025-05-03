using DecisionPlatformWeb.Config.NaturalUncertaintyConfigComponents;

namespace DecisionPlatformWeb.Config;

public class NaturalUncertaintyConfig
{
    public Parameter[] Parameters { get; set; }
    public Criteria[] WithoutProbabilityCriterias { get; set; }
    public Criteria[] WithProbabilityCriterias { get; set; }
}