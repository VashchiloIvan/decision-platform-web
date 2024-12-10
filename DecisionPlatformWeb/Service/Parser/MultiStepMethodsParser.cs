using DecisionPlatformWeb.Config;
using DecisionPlatformWeb.Entity;
using DecisionPlatformWeb.Entity.Inner;
using DecisionPlatformWeb.Exceptions;
using Microsoft.Extensions.Options;

namespace DecisionPlatformWeb.Service.Parser;

public class MultiStepMethodsParser
{
    private const string successiveConcessionsMethod = "Метод последовательных уступок";
    private const string electreMethod = "Метод ELECTRE";
    
    public MultiStepMethodsParser(IOptions<MultiCriteriaSolvingConfig> config)
    {
        var cfg = config.Value;

        var supported = new[]
        {
            successiveConcessionsMethod, electreMethod
        };

        var configMethods = cfg.MultiStepMethods.Select(m => m.Method).ToArray();

        if (ConfigChecker.IsValid(supported, configMethods))
        {
            throw new ConfigException("unsupported methods on MultiStepMethods section config");
        }
    }
    
    public MultiStepMethod Parse(MathModel model, CriteriaRelation relation, Method methodInfo)
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
        SuccessiveConcessionsMethod method = new SuccessiveConcessionsMethod();
        
        return method;
    }
    private ElectreMethod parseElectreMethod()
    {
        ElectreMethod method = new ElectreMethod();

        return method;
    }
    
    public AdditionalInfoControl[] GetControls(Method methodInfo)
    {
        MultiStepMethod method;
        
        switch (methodInfo.Name)
        {
            case successiveConcessionsMethod:
                return new AdditionalInfoControl[]
                {
                    new AdditionalInfoControl
                    {
                        Name = "Уступка",
                        InputType = "number" // double
                    }
                };
            case electreMethod:
                return new AdditionalInfoControl[]
                {
                    new AdditionalInfoControl
                    {
                        Name = "Alfa",
                        InputType = "number" // double
                    },
                    new AdditionalInfoControl
                    {
                        Name = "Beta",
                        InputType = "number" // double
                    },
                };
            default:
                throw new InvalidDataException($"unsupported method: {methodInfo.Name}");
        }
    }
    
    public DecisionMakerInfo Parse(Method methodInfo, MakerInfo info)
    {
        switch (methodInfo.Name)
        {
            case successiveConcessionsMethod:
                foreach (var makerParameter in info.Parameters)
                {
                    if (makerParameter.Name == "Уступка")
                    {
                        // TODO remove
                        makerParameter.Value = makerParameter.Value == "" ? "0" : makerParameter.Value; 
                        // TODO check convert to double
                        return new CedeValueInfo(Convert.ToDouble(makerParameter.Value));
                    }
                }
                
                throw new InvalidDataException($"invalid parameters: {info.Parameters}");
            case electreMethod:
                double alfa = 0, beta = 0;
                bool firstFilled = false;
                
                foreach (var makerParameter in info.Parameters)
                {
                    // TODO
                    makerParameter.Value = makerParameter.Value.Replace(".", ",");
                    if (makerParameter.Name == "Alfa")
                    {
                        // TODO check convert to double
                        alfa = Convert.ToDouble(makerParameter.Value);
                        
                        if (firstFilled)
                        {
                            return new AlfaBetaInfo(alfa, beta);
                        }
                        
                        firstFilled = true;
                    }
                    
                    if (makerParameter.Name == "Beta")
                    {
                        beta = Convert.ToDouble(makerParameter.Value);
                        
                        if (firstFilled)
                        {
                            return new AlfaBetaInfo(alfa, beta);
                        }
                        
                        firstFilled = true;
                    }
                }
                
                throw new InvalidDataException($"invalid parameters: {info.Parameters}");
            default:
                throw new InvalidDataException($"unsupported method: {methodInfo.Name}");
        }
    }
}