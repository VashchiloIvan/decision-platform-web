namespace DecisionPlatformWeb.Entity.NaturalUncertainty;

public class RequestData
{
    public MathModel MathModel { get; set; } // Объект mathModel
    public List<Criteria> Criterias { get; set; } // Список критериев
}