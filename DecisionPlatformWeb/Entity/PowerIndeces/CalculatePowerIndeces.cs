using PowerIndecesDecisionWrapperCsharp;
using System.Text.Json.Serialization;

namespace DecisionPlatformWeb.Entity.PowerIndeces
{
    public class CalculateRequest
    {
        [JsonPropertyName("quota")]
        public double Quota { get; set; }
        [JsonPropertyName("quotaType")]
        public QuotaType QuotaType { get; set; }
        [JsonPropertyName("calculationType")]
        public CalculateType CalculationType { get; set; }
        [JsonPropertyName("methodType")]
        public MethodType MethodType { get; set; }
        [JsonPropertyName("players")]
        public List<Player> Players { get; set; }
    }
}
