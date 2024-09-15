namespace DecisionPlatformWeb.Service;

public class ConfigChecker
{
    public static bool IsValid(string[] supportedMethods, string[] configMethods)
    {
        if (configMethods.Any(configMethod => !supportedMethods.Contains(configMethod)))
        {
            return false;
        }

        if (configMethods.Length > supportedMethods.Length)
        {
            return false;
        }

        return true;
    }
}