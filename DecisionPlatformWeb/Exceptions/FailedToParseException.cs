namespace DecisionPlatformWeb.Exceptions
{
    public class FailedToParseException : Exception
    {
        public string message { get; }

        public FailedToParseException(string message)
            : base($"Failed to parse: {message}")
        {
            this.message = message;
        }

        public FailedToParseException(string message, Exception innerException)
            : base($"Failed to parse: {message}", innerException)
        {
            this.message = message;
        }
    }
}