using DecisionPlatformWeb.Config;
using DecisionPlatformWeb.Entity.NaturalUncertainty;
using DecisionPlatformWeb.Exceptions;
using NaturalUncertaintyCsharpApi;

namespace DecisionPlatformWeb.Service.Parser.NaturalUncertainty;

public class ProbabilityCriteriaParser
{
    private readonly DecisionPlatformWeb.Config.NaturalUncertaintyConfigComponents.Criteria[] _criteriaArray;

    private const string _averageResultCriterion = "Average result criterion";

    public ProbabilityCriteriaParser(NaturalUncertaintyConfig config)
    {
        _criteriaArray = config.WithProbabilityCriterias;

        foreach (var criteria in _criteriaArray)
        {
            if (criteria.Method != _averageResultCriterion)
            {
                throw new ConfigException("wrong configuration");
            }
        }
    }

    public List<Criterion> Parse(List<Criteria> condition, ProbabilisticModel mathModel)
    {
        List<Criterion> result = new List<Criterion>();

        foreach (var criteria in condition)
        {
            switch (criteria.CriteriaName)
            {
                case _averageResultCriterion:
                    result.Add(parseAaverageResultCriterion(criteria, mathModel));

                    break;
                default:
                    throw new NotSupportedException($"criteria {criteria.CriteriaName} not supported");
            }
        }

        return result;
    }

    private Criterion parseAaverageResultCriterion(Entity.NaturalUncertainty.Criteria criteria, ProbabilisticModel mathModel)
    {
        return new AverageResultCriterion(mathModel);
    }
}