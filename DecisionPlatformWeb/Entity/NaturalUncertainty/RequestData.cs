namespace DecisionPlatformWeb.Entity.NaturalUncertainty;

public class RequestData
{
    public bool WithProbabilities { get; set; } // В mathModel известны вероятности
    public MathModel MathModel { get; set; } // Объект mathModel
    public List<Criteria> Criterias { get; set; } // Список критериев
}