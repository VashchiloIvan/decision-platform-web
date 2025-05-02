using DecisionPlatformWeb.Entity.Inner;
using MultiCriteriaCsharpApi;

namespace DecisionPlatformWeb.Entity;

public class IAdditionalInfoControl
{
    public string Name { get; set; }
}

public class InputInfoControl : IAdditionalInfoControl
{
    public string Name { get; set; }
    public string InputType { get; set; } // например, "number", "text", "checkbox"
}

public class CriteriaTableInputsInfoControl : IAdditionalInfoControl
{
    public string Name { get; set; } // Например "Параметры критериев"

    public List<CriterionThresholdInput> CriteriaInputs { get; set; }
}
public class CriterionThresholdInput
{
    public string criterion { get; set; }
    public Thresholds thresholds { get; set; }
}

public class Thresholds
{
    public double indifference { get; set; }
    public double preference { get; set; }
    public double veto { get; set; }
}

public class MultiSolveProcess
{
    public Guid Guid { get; set; }
    public Method Method { get; set; }
    public TaskProcess Process { get; set; }
    public bool IsFinished { get; set; }
    public bool IsValidModel { get; set; }
    public IAdditionalInfoControl[] Controls { get; set; }
    public MathModel MathModel { get; set; }
}