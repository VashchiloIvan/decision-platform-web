using DecisionPlatformWeb.Entity.Inner;
using DecisionPlatformWeb.Service.Factory;

namespace DecisionPlatformWeb.Service;

public class MathModelParser
{
    private readonly CriteriaRelationFactory _factory;

    public MathModelParser(CriteriaRelationFactory factory)
    {
        _factory = factory;
    }

    public MathModel Parse(Entity.Inner.Criteria[] conditionCriteriaList, Alternative[] conditionAlternativeList)
    {
        var cType = (string type) =>
        {
            return type == "max" ? CriteriaType.MAXIMIZATION : CriteriaType.MINIMIZATION;
        };
        
        Criterias criteriaList = new Criterias();
        foreach (var criteria in conditionCriteriaList)
        {
            criteriaList.Add(new Criteria(criteria.Name, cType(criteria.Type)));
        }

        EstimateVectors vectors = new EstimateVectors();
        foreach (var alternative in conditionAlternativeList)
        {
            vectors.Add(new EstimateVector(alternative.Name, new Marks(alternative.Marks)));
        }

        return new MathModel(criteriaList, vectors);
    }

    public CriteriaRelation Parse(MathModel model, Entity.Inner.CriteriaRelation relation)
    {
        return _factory.ProduceCriteriaRelation(model, relation);
    }
}