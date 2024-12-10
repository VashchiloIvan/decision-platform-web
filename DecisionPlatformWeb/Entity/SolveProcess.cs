using DecisionPlatformWeb.Entity.Inner;

namespace DecisionPlatformWeb.Entity;

public class MethodProcess
{
    public Method Method { get; set; }
    public TaskProcess Process { get; set; }
    public string Result { get; set; }
}

public class SolveProcess
{
    public Guid Guid { get; set; }
    public List<MethodProcess> Processes { get; set; }

    public SolveProcess()
    {
        Processes = new List<MethodProcess>();
    }
}