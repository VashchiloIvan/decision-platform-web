namespace DecisionPlatformWeb.Entity;

public class MakerParameter
{
    public string Name { get; set; }
    public string Value { get; set; }
}

public class MakerInfo
{
    public Guid Id { get; set; }
    public MakerParameter[] Parameters { get; set; }
}