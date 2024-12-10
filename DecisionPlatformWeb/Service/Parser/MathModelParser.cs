using DecisionPlatformWeb.Entity.Inner;

namespace DecisionPlatformWeb.Service.Parser;

public class MathModelParser
{
    private readonly CriteriaRelationParser _parser;

    public MathModelParser(CriteriaRelationParser parser)
    {
        _parser = parser;
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
        return _parser.ProduceCriteriaRelation(model, relation);
    }
}