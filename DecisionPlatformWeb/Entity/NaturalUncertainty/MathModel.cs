namespace DecisionPlatformWeb.Entity.NaturalUncertainty;

public class AlternativeParams
{
    public double Mark { get; set; }
    public double Probability { get; set; }
}

public class MathModel
{
    public List<string> Uncertainties { get; set; } // Список неопределенностей
    public Dictionary<string, List<AlternativeParams>> Alternatives { get; set; } // Словарь альтернатив
}