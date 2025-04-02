namespace DecisionPlatformWeb.Entity.NaturalUncertainty;

public class MathModel
{
    public List<string> Uncertainties { get; set; } // Список неопределенностей
    public Dictionary<string, List<double>> Alternatives { get; set; } // Словарь альтернатив
}