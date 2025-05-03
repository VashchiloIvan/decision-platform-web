using NaturalUncertaintyCsharpApi;

namespace DecisionPlatformWeb.Service.Parser.NaturalUncertainty;

public class ProbabilisticModelParser
{
    public ProbabilisticModel Parse(Entity.NaturalUncertainty.MathModel condition)
    {
        UncertaintyList uncertainties = new UncertaintyList();
        foreach (var conditionUncertainty in condition.Uncertainties)
        {
            uncertainties.Add(new Uncertainty(conditionUncertainty));
        }

        ProbabilisticAlternativeList alternatives = new ProbabilisticAlternativeList();
        foreach (var (alternativeName, paramsArray) in condition.Alternatives)
        {
            var marks = new DoubleList();
            var probabilities = new DoubleList();

            foreach (var alternativeParams in paramsArray)
            {
                marks.Add(alternativeParams.Mark);
                probabilities.Add(alternativeParams.Probability);
            }

            alternatives.Add(new ProbabilisticAlternative(alternativeName, marks, probabilities));
        }

        return new ProbabilisticModel(alternatives, uncertainties);
    }
}