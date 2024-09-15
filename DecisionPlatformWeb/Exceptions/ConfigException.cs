namespace DecisionPlatformWeb.Exceptions
{
    public class ConfigException : Exception
    {
        public string message { get; }

        public ConfigException(string message)
            : base($"Failed to parse: {message}")
        {
            this.message = message;
        }
        
        public ConfigException(string message, Exception innerException)
            : base($"Failed to parse: {message}", innerException)
        {
            this.message = message;
        }
    }
}