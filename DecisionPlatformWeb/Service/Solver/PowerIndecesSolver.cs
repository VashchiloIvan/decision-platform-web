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

        public PowerIndexs CalcSSDirectEquiles(List<Player> players, double quota)
        {
            return DirectCalcService.powerIndexShapleyShubick(new Players(players), (int)QuotaType.EQUALITY, quota);
        }

        public PowerIndexs CalcSSDirectExceeding(List<Player> players, double quota)
        {
            return DirectCalcService.powerIndexShapleyShubick(new Players(players), (int)QuotaType.EXCEEDING, quota);
        }

        public PowerIndexs CalcBDirectEquiles(List<Player> players, double quota)
        {
            return DirectCalcService.powerIndexBanzhaf(new Players(players), (int)QuotaType.EQUALITY, quota);
        }

        public PowerIndexs CalcBDirectExceeding(List<Player> players, double quota)
        {
            return DirectCalcService.powerIndexBanzhaf(new Players(players), (int)QuotaType.EXCEEDING, quota);
        }

        public PowerIndexs CalcSSGenerEquiles(List<Player> players, double quota)
        {
            return GenerCalcService.powerIndexShapleyShubick(new Players(players), (int)QuotaType.EQUALITY, quota);
        }

        public PowerIndexs CalcSSGenerExceeding(List<Player> players, double quota)
        {
            return GenerCalcService.powerIndexShapleyShubick(new Players(players), (int)QuotaType.EXCEEDING, quota);
        }

        public PowerIndexs CalcBGenerEquiles(List<Player> players, double quota)
        {
            return GenerCalcService.powerIndexBanzhaf(new Players(players), (int)QuotaType.EQUALITY, quota); 
        }

        public PowerIndexs CalcBGenerExceeding(List<Player> players, double quota)
        {
            return GenerCalcService.powerIndexBanzhaf(new Players(players), (int)QuotaType.EXCEEDING, quota);
        }
    }
}
