namespace DecisionPlatformWeb.Config;

public static class DependencyConfig
{
    private const string MultiCriteriaSolvingSection = "MultiCriteriaSolving";
    private const string NaturalUncertaintySection = "NaturalUncertainty";
    
    public static void Configure(WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        services.AddMemoryCache(_ => { });

        var cfgSection = configuration.GetRequiredSection(MultiCriteriaSolvingSection);
        services.Configure<MultiCriteriaSolvingConfig>(cfgSection);
        
        cfgSection = configuration.GetRequiredSection(NaturalUncertaintySection);
        services.Configure<NaturalUncertaintyConfig>(cfgSection);
    }
}