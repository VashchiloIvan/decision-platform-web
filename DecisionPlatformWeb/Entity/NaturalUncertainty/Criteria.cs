namespace DecisionPlatformWeb.Entity.NaturalUncertainty;

public class Criteria
{
    public string CriteriaName { get; set; } // Название критерия
    public List<CriteriaParameter> Parameters { get; set; } // Список параметров
}