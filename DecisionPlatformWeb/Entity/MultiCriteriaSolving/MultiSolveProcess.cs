using DecisionPlatformWeb.Entity.Inner;
using MultiCriteriaCsharpApi;

namespace DecisionPlatformWeb.Entity;

public class AdditionalInfoControl
{
    public string Name;
    public string InputType; // number
}

public class MultiSolveProcess
{
    public Guid Guid { get; set; }
    public Method Method { get; set; }
    public TaskProcess Process { get; set; }
    public bool IsFinished { get; set; }
    public bool IsValidModel { get; set; }
    public AdditionalInfoControl[] Controls { get; set; }
}