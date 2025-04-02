using NaturalUncertaintyCsharpApi;

namespace DecisionPlatformWeb.Entity.NaturalUncertainty;

public class Protocol
{
    public Criteria Criteria { get; set; }
    public TaskProcess SolvingProtocol { get; set; }
}

public class Protocols
{
    public List<Protocol> SolvingProtocols { get; set; } = new();
}