using DecisionPlatformWeb.Config.MultiCriteriaConfigComponents;
using DecisionPlatformWeb.Entity.Inner;
using DecisionPlatformWeb.Exceptions;
using DecisionWrapperCsharp;

namespace DecisionPlatformWeb.Service.Parser.MultiCriteriaSolving;

public class CriteriaFunctionTypesParser
{
    private readonly string configKey = "Тип функции по критериям";

    public CriteriaFunctionTypesParser(List<OptionalMethodConfig> config)
    {
        OptionalMethodConfig? currentConfig = config.FirstOrDefault(methodConfig => methodConfig.Name == configKey);
        if (currentConfig == null)
        {
            throw new ConfigException($"failed to parse {configKey} section of config");
        }
    }

    // 1 - Уровневый критерий[Порог безразличия-1, Порог строгого предпочтения-2
    // 2 - Квази-критерий[Порог безразличия-2]
    public CriterionTypeList Parse(AdditionalMethod[] info)
    {
        AdditionalMethod? section = info.FirstOrDefault(additionalMethod => additionalMethod.Name == configKey);
        if (section == null)
        {
            throw new FailedToParseException("failed to parse criteria function types");
        }

        CriterionTypeList list = new CriterionTypeList();
        foreach (var criteriaSection in section.Value.Split("], "))
        {
            var type = criteriaSection.Split(" - ")[1];

            var q = 0.0;
            var r = 0.0;
            var sigma = 0.0;
            
            foreach (var se in type.Split("[")[1].Split(", "))
            {
                var sect = se.Split("]")[0];
                
                if (sect.Contains("Порог безразличия"))
                {
                    q = double.Parse(sect.Split("-")[1]);
                }
                
                if (sect.Contains("Порог строгого предпочтения"))
                {
                    r = double.Parse(sect.Split("-")[1]);
                }

                if (sect.Contains("Величина между порогом строгого предпочтения и базразличия"))
                {
                    sigma = double.Parse(sect.Split("-")[1]);
                }
            }

            switch (type.Split("[")[0])
            {
                case "Обычный критерий":
                    list.Add(new UsualCriterion());
                    break;
                case "Квази-критерий":
                    list.Add(new QuasiCriterion(q));
                    break;
                case "Критерий с линейным предпочтением":
                    list.Add(new VShapeCriterion(q));
                    break;
                case "Уровневый критерий":
                    list.Add(new LevelCriterion(q, r));
                    break;
                case "Критерий V-формы с областью безразличия":
                    list.Add(new LinearCriterion(q, r));
                    break;
                case "Гауссовский критерий":
                    list.Add(new GaussianCriterion(sigma));
                    break;
            }
        }

        return list;
    }
}