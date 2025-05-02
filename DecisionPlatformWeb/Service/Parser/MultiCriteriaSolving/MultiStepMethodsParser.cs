using System.Text.Json;
using DecisionPlatformWeb.Config;
using DecisionPlatformWeb.Entity;
using DecisionPlatformWeb.Entity.Inner;
using DecisionPlatformWeb.Exceptions;
using MultiCriteriaCsharpApi;
using Microsoft.Extensions.Options;

namespace DecisionPlatformWeb.Service.Parser.MultiCriteriaSolving;

public class MultiStepMethodsParser
{
    private const string successiveConcessionsMethod = "Метод последовательных уступок";
    private const string electreMethod = "Метод ELECTRE";
    private const string electreIIMethod = "Метод ELECTRE II";
    private const string electreIIIMethod = "Метод ELECTRE III";

    public MultiStepMethodsParser(IOptions<MultiCriteriaSolvingConfig> config)
    {
        var cfg = config.Value;

        var supported = new[]
        {
            successiveConcessionsMethod, electreMethod, electreIIMethod, electreIIIMethod
        };

        var configMethods = cfg.MultiStepMethods.Select(m => m.Name).ToArray();

        if (!ConfigChecker.IsValid(supported, configMethods))
        {
            throw new ConfigException("unsupported methods on MultiStepMethods section config");
        }
    }

    public MultiStepMethod Parse(MathModel model, MultiCriteriaCsharpApi.CriteriaRelation relation, Method methodInfo)
    {
        MultiStepMethod method;

        switch (methodInfo.Name)
        {
            case successiveConcessionsMethod:
                method = parseSuccessiveConcessionsMethod();
                break;
            case electreMethod:
                method = parseElectreMethod();
                break;
            case electreIIMethod:
                method = parseElectreIIMethod();
                break;
            case electreIIIMethod:
                method = parseElectreIIIMethod();
                break;
            default:
                throw new InvalidDataException($"unsupported method: {methodInfo.Name}");
        }

        method.setMathModel(model);
        method.setCriteriaRelation(relation);
        method.withSolveProcessLog();

        return method;
    }

    private SuccessiveConcessionsMethod parseSuccessiveConcessionsMethod()
    {
        return new SuccessiveConcessionsMethod();
    }

    private ElectreMethod parseElectreMethod()
    {
        return new ElectreMethod();
    }

    private ElectreIIMethod parseElectreIIMethod()
    {
        return new ElectreIIMethod();
    }

    private ElectreIIIMethod parseElectreIIIMethod()
    {
        return new ElectreIIIMethod();
    }

    public IAdditionalInfoControl[] GetControls(Method methodInfo)
    {
        switch (methodInfo.Name)
        {
            case successiveConcessionsMethod:
                return new IAdditionalInfoControl[]
                {
                    new InputInfoControl()
                    {
                        Name = "Уступка",
                        InputType = "number" // double
                    }
                };
            case electreMethod:
                return new IAdditionalInfoControl[]
                {
                    new InputInfoControl()
                    {
                        Name = "Alfa",
                        InputType = "number" // double
                    },
                    new InputInfoControl()
                    {
                        Name = "Beta",
                        InputType = "number" // double
                    },
                };
            case electreIIMethod:
                return new IAdditionalInfoControl[]
                {
                    new InputInfoControl()
                    {
                        Name = "Порог сильной согласованности",
                        InputType = "number" // double
                    },
                    new InputInfoControl()
                    {
                        Name = "Порог средней согласованности",
                        InputType = "number" // double
                    },
                    new InputInfoControl()
                    {
                        Name = "Порог слабой согласованности",
                        InputType = "number" // double
                    },
                    new InputInfoControl()
                    {
                        Name = "Порог сильной несогласованности",
                        InputType = "number" // double
                    },
                    new InputInfoControl()
                    {
                        Name = "Порог слабой несогласованности",
                        InputType = "number" // double
                    },
                };
            case electreIIIMethod:
                return new IAdditionalInfoControl[]
                {
                    new CriteriaTableInputsInfoControl
                    {
                        Name = "Пороговые значения критериев",
                        CriteriaInputs = new List<CriterionThresholdInput>() 
                    }
                };
            default:
                throw new InvalidDataException($"unsupported method: {methodInfo.Name}");
        }
    }

    public MultiCriteriaCsharpApi.DecisionMakerInfo Parse(Method methodInfo, MakerInfo info)
    {
        switch (methodInfo.Name)
        {
            case successiveConcessionsMethod:
                foreach (var makerParameter in info.Parameters)
                {
                    if (makerParameter.Name == "Уступка")
                    {
                        makerParameter.Value = makerParameter.Value == "" ? "0" : makerParameter.Value;
                        return new CedeValueInfo(Convert.ToDouble(makerParameter.Value.Replace(".", ",")));
                    }
                }

                throw new InvalidDataException($"invalid parameters: {info.Parameters}");

            case electreMethod:
                double alfa = 0, beta = 0;
                bool firstFilled = false;

                foreach (var makerParameter in info.Parameters)
                {
                    var value = makerParameter.Value.Replace(".", ",");
                    if (makerParameter.Name == "Alfa")
                    {
                        alfa = Convert.ToDouble(value);
                        if (firstFilled)
                            return new AlfaBetaInfo(alfa, beta);

                        firstFilled = true;
                    }
                    else if (makerParameter.Name == "Beta")
                    {
                        beta = Convert.ToDouble(value);
                        if (firstFilled)
                            return new AlfaBetaInfo(alfa, beta);

                        firstFilled = true;
                    }
                }

                throw new InvalidDataException($"invalid parameters: {info.Parameters}");

            case electreIIMethod:
                double concStrong = 0, concMean = 0, concWeak = 0, discStrong = 0, discWeak = 0;

                foreach (var makerParameter in info.Parameters)
                {
                    var value = makerParameter.Value.Replace(".", ",");
                    if (makerParameter.Name == "Порог сильной согласованности")
                        concStrong = Convert.ToDouble(value);
                    else if (makerParameter.Name == "Порог средней согласованности")
                        concMean = Convert.ToDouble(value);
                    else if (makerParameter.Name == "Порог слабой согласованности")
                        concWeak = Convert.ToDouble(value);
                    else if (makerParameter.Name == "Порог сильной несогласованности")
                        discStrong = Convert.ToDouble(value);
                    else if (makerParameter.Name == "Порог слабой несогласованности")
                        discWeak = Convert.ToDouble(value);
                }

                return new ConcordanceDisconcordanceThresholds(concStrong, concMean, concWeak, discStrong, discWeak);

            case electreIIIMethod:
            {
                var list = new IndifferencePreferenceVetoList();

                var parameter = info.Parameters[0];

                if (string.IsNullOrWhiteSpace(parameter.Value))
                {
                    throw new InvalidDataException("Пороговые значения критериев пустые.");
                }
                
                CriterionThresholdInput[] criteriaInputs;

                try
                {
                    criteriaInputs = JsonSerializer.Deserialize<CriterionThresholdInput[]>(parameter.Value);
                }
                catch (Exception ex)
                {
                    throw new InvalidDataException("Ошибка при разборе таблицы критериев ELECTRE III.", ex);
                }

                var i = 0;
                foreach (var criterion in criteriaInputs)
                {
                    list.Add(i, new IndifferencePreferenceVeto(
                            criterion.thresholds.indifference,
                            criterion.thresholds.preference,
                            criterion.thresholds.veto
                        )
                    );

                    i++;
                }

                return new IndifferencePreferenceVetoInfo(list);
            }
            default:
                throw new InvalidDataException($"unsupported method: {methodInfo.Name}");
        }
    }
}