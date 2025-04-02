using NaturalUncertaintyCsharpApi;

namespace DecisionPlatformWeb.Service.Parser.NaturalUncertainty;

public class MathModelParser
{
    public MathModel Parse(Entity.NaturalUncertainty.MathModel condition)
    {
        UncertaintyList uncertainties = new UncertaintyList();
        foreach (var conditionUncertainty in condition.Uncertainties)
        {
            uncertainties.Add(new Uncertainty(conditionUncertainty));
        }

        AlternativeList alternatives = new AlternativeList();
        foreach (var (alternativeName, marks) in condition.Alternatives)
        {
            alternatives.Add(new Alternative(alternativeName, new DoubleList(marks)));
        }

        return new MathModel(alternatives, uncertainties);
    }
}