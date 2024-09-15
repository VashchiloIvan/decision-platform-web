using DecisionPlatformWeb.Config;
using DecisionPlatformWeb.Entity.Inner;

namespace DecisionPlatformWeb.Service.Factory;

public class CriteriaRelationFactory
{
    private readonly MultiCriteriaSolvingConfig _config;

    public CriteriaRelationFactory(MultiCriteriaSolvingConfig config)
    {
        _config = config;
    }

    public CriteriaRelation ProduceCriteriaRelation(MathModel model, Entity.Inner.CriteriaRelation relation)
    {
        var method = "";

        foreach (var cfg in _config.CriteriaRelations)
        {
            if (cfg.Name == relation.Name)
            {
                method = cfg.Method;
            }
        }

        if (method == "AllCriteriaRelation")
        {
            return produceAllCriteriaRelation(model, relation);
        }

        if (method == "SimpleRankingMethod")
        {
            return produceSimpleRankingMethod(model, relation);
        }

        if (method == "ProportionalMethod")
        {
            return produceProportionalMethod(model, relation);
        }

        if (method == "SimpleCriteriaRelation")
        {
            return produceSimpleCriteriaRelation(model, relation);
        }

        throw new ArgumentException("Информация о важности критериев не поддерживается!");
    }


    private CriteriaRelation produceAllCriteriaRelation(MathModel model, Entity.Inner.CriteriaRelation relation)
    {
        var criterias = getCriteriaMap(model);
        
        IntList result = new IntList();
        CriteriaInfo[] criteriaInfos = relation.Info.OrderBy(cR => cR.Value).ToArray();
        foreach (CriteriaInfo criteriaInfo in criteriaInfos)
        {
            result.Add(criterias[criteriaInfo.CriteriaName]);
        }
        
        return new AllCriteriaRelation(result);
    }

    private CriteriaRelation produceSimpleRankingMethod(MathModel model, Entity.Inner.CriteriaRelation relation)
    {
        var criterias = getCriteriaMap(model);

        var map = getWeightMap(criterias, relation);
        
        return new SimpleRankingMethod(model.getCriteriaList().Count, map);
    }

    private CriteriaRelation produceProportionalMethod(MathModel model, Entity.Inner.CriteriaRelation relation)
    {
        var criterias = getCriteriaMap(model);

        var map = getWeightMap(criterias, relation);
        
        return new ProportionalMethod(model.getCriteriaList().Count, map);
    }

    private CriteriaRelation produceSimpleCriteriaRelation(MathModel model, Entity.Inner.CriteriaRelation relation)
    {
        TwoCriteriaRelationList relationList = new TwoCriteriaRelationList();

        var criterias = getCriteriaMap(model);
        
        foreach (var criteriaInfo in relation.Info)
        {
            relationList.Add(new TwoCriteriaRelation(
                criterias[criteriaInfo.First],
                criteriaConstraint(criteriaInfo.Constraint),
                criterias[criteriaInfo.Second]
            ));
        }

        return new SimpleCriteriaRelation(relationList, model.getCriteriaList().Count());
    }

    private WeightMap getWeightMap(Dictionary<string, int> criterias, Entity.Inner.CriteriaRelation relation)
    {
        WeightMap map = new WeightMap();
        
        foreach (var info in relation.Info)
        {
            map.Add(criterias[info.CriteriaName], (double)info.Value);
        }

        return map;
    }
    
    private Dictionary<string, int> getCriteriaMap(MathModel model)
    {
        var idx = 0;
        Dictionary<string, int> criterias = new Dictionary<string, int>();
        
        foreach (var criteria in model.getCriteriaList())
        {
            criterias[criteria.getName()] = idx++;
        }

        return criterias;
    }

    private CriteriaConstraint criteriaConstraint(string criteriaConstraint)
    {
        switch (criteriaConstraint)
        {
            case ">":
                return CriteriaConstraint.More;
            case ">=":
                return CriteriaConstraint.MoreOrEquivalent;
            case "<":
                return CriteriaConstraint.Less;
            case "<=":
                return CriteriaConstraint.LessOrEquivalent;
            default:
                return CriteriaConstraint.Equivalent;
        }
    }
}