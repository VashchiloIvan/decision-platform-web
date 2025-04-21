using DecisionPlatformWeb.Config;
using DecisionPlatformWeb.Entity.Inner;
using MultiCriteriaCsharpApi;
using CriteriaRelation = DecisionPlatformWeb.Entity.Inner.CriteriaRelation;

namespace DecisionPlatformWeb.Service.Parser.MultiCriteriaSolving;

public class CriteriaRelationParser
{
    private readonly MultiCriteriaSolvingConfig _config;

    public CriteriaRelationParser(MultiCriteriaSolvingConfig config)
    {
        _config = config;
    }

    public MultiCriteriaCsharpApi.CriteriaRelation ProduceCriteriaRelation(MathModel model, CriteriaRelation? relation)
    {
        var method = "";

        if (relation == null)
        {
            return null;
        }

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

        return null;
    }


    private MultiCriteriaCsharpApi.CriteriaRelation produceAllCriteriaRelation(MathModel model, CriteriaRelation relation)
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

    private MultiCriteriaCsharpApi.CriteriaRelation produceSimpleRankingMethod(MathModel model, Entity.Inner.CriteriaRelation relation)
    {
        var criterias = getCriteriaMap(model);

        var map = getWeightMap(criterias, relation);
        
        return new SimpleRankingMethod(model.getCriteriaList().Count, map);
    }

    private MultiCriteriaCsharpApi.CriteriaRelation produceProportionalMethod(MathModel model, Entity.Inner.CriteriaRelation relation)
    {
        var criterias = getCriteriaMap(model);

        var map = getWeightMap(criterias, relation);
        
        return new ProportionalMethod(model.getCriteriaList().Count, map);
    }

    private MultiCriteriaCsharpApi.CriteriaRelation produceSimpleCriteriaRelation(MathModel model, Entity.Inner.CriteriaRelation relation)
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