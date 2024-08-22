namespace DecisionPlatformWeb.Config;

public static class DependencyConfig
{
    private const string MultiCriteriaSolvingSection = "MultiCriteriaSolving";
    
    public static void Configure(WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        services.Configure<MultiCriteriaSolvingConfig>(
            configuration.GetSection(MultiCriteriaSolvingSection));
    }
}