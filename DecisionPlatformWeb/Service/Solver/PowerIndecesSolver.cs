using DecisionPlatformWeb.Entity.PowerIndeces;
using PowerIndecesDecisionWrapperCsharp;

namespace DecisionPlatformWeb.Service.Solver
{
    public class PowerIndecesSolver
    {
        private DirectCalc DirectCalcService { get; set; }
        private GenerCalc GenerCalcService { get; set; }


        public PowerIndecesSolver()
        {
            DirectCalcService = new DirectCalc();
            GenerCalcService = new GenerCalc();
        }

        private Players calculateDirection(List<Player> players, double quota, CalculateType calculateType, QuotaType quotaType)
        {
            Players result = new Players();
            switch (calculateType)
            {
                case CalculateType.ShapleyShupic:
                    result = DirectCalcService.powerIndexShapleyShubick(new Players(players), (int)quotaType, quota);
                    break;
                case CalculateType.Banzhaf:
                    result = DirectCalcService.powerIndexBanzhaf(new Players(players), (int)quotaType, quota);
                    break;
                case CalculateType.Johnson:
                    result = DirectCalcService.powerIndexJohnson(new Players(players), (int)quotaType, quota);
                    break;
                default:
                    break;
            }
            return result;
        }

        private Players calculateGeneration(List<Player> players, double quota, CalculateType calculateType, QuotaType quotaType)
        {
            Players result = new Players();
            switch (calculateType)
            {
                case CalculateType.ShapleyShupic:
                    result = GenerCalcService.powerIndexShapleyShubick(new Players(players), (int)quotaType, quota);
                    break;
                case CalculateType.Banzhaf:
                    result = GenerCalcService.powerIndexBanzhaf(new Players(players), (int)quotaType, quota);
                    break;
                default:
                    break;
            }
            return result;
        }

        public Players calculatePowerIndeces(List<Player> players, double quota, QuotaType quotaType, CalculateType calculateType, MethodType methodType)
        {
            switch (methodType)
            {
                case MethodType.Direction:
                    return this.calculateDirection(players, quota, calculateType, quotaType);
                case MethodType.Generation:
                    return this.calculateGeneration(players, quota, calculateType, quotaType);
                default:
                    return new Players();
            }
        }
    }
}
