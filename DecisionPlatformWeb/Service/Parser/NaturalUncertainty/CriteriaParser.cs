using DecisionPlatformWeb.Config;
using DecisionPlatformWeb.Entity.NaturalUncertainty;
using DecisionPlatformWeb.Exceptions;
using NaturalUncertaintyCsharpApi;
using Criteria = DecisionPlatformWeb.Config.NaturalUncertaintyConfigComponents.Criteria;
using MathModel = NaturalUncertaintyCsharpApi.MathModel;

namespace DecisionPlatformWeb.Service.Parser.NaturalUncertainty;

public class CriteriaParser
{
    private readonly Criteria[] _criteriaArray;

    private const string _waldCriterion = "Wald's criterion";
    private const string _hurwiczCriterion = "Hurwicz criterion";
    private const string _bernoulliLaplaceCriterion = "Bernoulli-Laplace criterion";

    private const string _optimismCoefficient = "OptimismCoefficient";

    public CriteriaParser(NaturalUncertaintyConfig config)
    {
        _criteriaArray = config.Criterias;

        foreach (var criteria in _criteriaArray)
        {
            if (criteria.Method != _waldCriterion &&
                criteria.Method != _hurwiczCriterion &&
                criteria.Method != _bernoulliLaplaceCriterion)
            {
                throw new ConfigException("wrong configuration");
            }
        }
    }

    public List<Criterion> Parse(List<Entity.NaturalUncertainty.Criteria> condition, MathModel mathModel)
    {
        List<Criterion> result = new List<Criterion>();

        foreach (var criteria in condition)
        {
            switch (criteria.CriteriaName)
            {
                case _waldCriterion:
                    result.Add(parseWaldCriterion(criteria, mathModel));
                    
                    break;
                case _hurwiczCriterion:
                    result.Add(parseHurwiczCriterion(criteria, mathModel));
                    
                    break;
                case _bernoulliLaplaceCriterion:
                    result.Add(parseBernoulliLaplaceCriterion(criteria, mathModel));
                    
                    break;
                default:
                    throw new NotSupportedException($"criteria {criteria.CriteriaName} not supported");
            }
        }

        return result;
    }

    private Criterion parseBernoulliLaplaceCriterion(Entity.NaturalUncertainty.Criteria criteria, MathModel mathModel)
    {
        return new PrincipleOfInsufficientReason(mathModel);
    }

    private OptimismPessimismCriterion parseHurwiczCriterion(Entity.NaturalUncertainty.Criteria criteria, MathModel mathModel)
    {
        CriteriaParameter? parameter = criteria.Parameters.FirstOrDefault(parameter => parameter.Key == _optimismCoefficient);

        if (parameter == null)
        {
            throw new InvalidDataException($"parameter {_optimismCoefficient} not specified");
        }

        parameter.Value = parameter.Value.Replace(".", ",");
        if (!double.TryParse(parameter.Value, out var param))
        {
            throw new InvalidDataException($"parameter {_optimismCoefficient} not number");
        }

        return new OptimismPessimismCriterion(mathModel, param);
    }

    private MaximinCriterion parseWaldCriterion(Entity.NaturalUncertainty.Criteria criteria, MathModel mathModel)
    {
        return new MaximinCriterion(mathModel);
    }
}