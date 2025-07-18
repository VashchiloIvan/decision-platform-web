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
        foreach (var (alternativeName, paramsArray) in condition.Alternatives)
        {
            var marks = new DoubleList();
            
            foreach (var alternativeParams in paramsArray)
            {
                marks.Add(alternativeParams.Mark);
            }

            alternatives.Add(new Alternative(alternativeName, marks));
        }

        return new MathModel(alternatives, uncertainties);
    }
}