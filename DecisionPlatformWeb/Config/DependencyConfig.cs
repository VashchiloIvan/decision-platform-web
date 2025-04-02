using DecisionPlatformWeb.Service.Cache;

namespace DecisionPlatformWeb.Config;

public static class DependencyConfig
{
    private const string MultiCriteriaSolvingSection = "MultiCriteriaSolving";
    private const string NaturalUncertaintySection = "NaturalUncertainty";
    
    public static void Configure(WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        var cfgSection = configuration.GetRequiredSection(MultiCriteriaSolvingSection);
        services.Configure<MultiCriteriaSolvingConfig>(cfgSection);
        
        cfgSection = configuration.GetRequiredSection(NaturalUncertaintySection);
        services.Configure<NaturalUncertaintyConfig>(cfgSection);

        services.AddSingleton(new Cache(cfgSection.GetValue<double>("CacheTimeout")));
    }
}