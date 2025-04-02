namespace DecisionPlatformWeb.Entity;

using DecisionPlatformWeb.Entity.Inner;

public class TaskCondition
{
    public Criteria[] CriteriaList { get; set; }
    public MultiCriteriaSolving.Inner.Alternative[] AlternativeList { get; set; }
    public CriteriaRelation CriteriaRelation { get; set; }
    public MethodInfo MethodInfo { get; set; }

    public TaskCondition()
    {
        CriteriaList = Array.Empty<Criteria>();
        AlternativeList = Array.Empty<MultiCriteriaSolving.Inner.Alternative>();
    }
}