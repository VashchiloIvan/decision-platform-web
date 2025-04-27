using PowerIndecesDecisionWrapperCsharp;

namespace DecisionPlatformWeb.Entity.PowerIndeces
{
    public class CalculateRequest
    {
        public double Quota { get; set; }
        public int QuotaType { get; set; }
        public int CalcType { get; set; }
        public List<Player> Players { get; set; }
    }
}
