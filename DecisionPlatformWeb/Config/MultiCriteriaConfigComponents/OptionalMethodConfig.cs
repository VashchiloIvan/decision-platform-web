namespace DecisionPlatformWeb.Config.MultiCriteriaConfigComponents;

public class OptionalMethodConfig
{
    public string Key { get; set; }
    public string Name { get; set; }
    
    public string Type { get; set; }
    public List<Option>? Methods { get; set; }
}