namespace DecisionPlatformWeb.Entity;

public enum StepType
{
    Unspecified,
    Text,
    Table
} 
public class ProcessStep
{
    public StepType Type { get; set; }
}

public class TableProcessStep : ProcessStep
{
    public List<List<string>> Table { get; set; }
    public bool WithHeaders { get; set; }
}

public class TextProcessStep : ProcessStep
{
    public string Content { get; set; }
    public bool WithHeaders { get; set; }
}

public class SolveProcess
{
    public Guid Guid { get; set; }
    public List<ProcessStep> Process { get; set; }
}