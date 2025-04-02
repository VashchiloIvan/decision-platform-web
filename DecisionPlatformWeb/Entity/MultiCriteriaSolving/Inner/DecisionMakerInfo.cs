namespace DecisionPlatformWeb.Entity.Inner;

public class DecisionMakerInfo
{
    public Guid Guid { get; set; }
    public Dictionary<string, string> Parameters { get; set; }
}