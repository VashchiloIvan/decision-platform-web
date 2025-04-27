using DecisionPlatformWeb.Entity.PowerIndeces;
using DecisionPlatformWeb.Service.Solver;
using Microsoft.AspNetCore.Mvc;
using PowerIndecesDecisionWrapperCsharp;

namespace DecisionPlatformWeb.Controllers
{

    [Route("[controller]")]
    public class PowerIndecesController : Controller
    {
        private readonly PowerIndecesSolver Solver;

        public PowerIndecesController()
        {
            Solver = new PowerIndecesSolver();
        }

        [HttpPost("calculate/ss")]
        public IActionResult CalculateSS([FromBody] CalculateRequest request)
        {
            if (request.QuotaType == (int)QuotaType.EQUALITY)
            {
                if (request.CalcType == 0) {
                    PowerIndexs powerIndeces = Solver.CalcSSDirectEquiles(request.Players, request.Quota);
                    return Json(powerIndeces);
                }
                else
                {
                    PowerIndexs powerIndeces = Solver.CalcSSGenerEquiles(request.Players, request.Quota);
                    return Json(powerIndeces);
                }
            } else
            {
                if (request.CalcType == 0)
                {
                    PowerIndexs powerIndeces = Solver.CalcSSDirectExceeding(request.Players, request.Quota);
                    return Json(powerIndeces);
                }
                else
                {
                    PowerIndexs powerIndeces = Solver.CalcSSGenerExceeding(request.Players, request.Quota);
                    return Json(powerIndeces);
                }
            }
        }

        [HttpPost("calculate/banz")]
        public IActionResult CalculateBanz([FromBody] CalculateRequest request)
        {
            if (request.QuotaType == (int)QuotaType.EQUALITY)
            {
                if (request.CalcType == 0)
                {
                    PowerIndexs powerIndeces = Solver.CalcBDirectEquiles(request.Players, request.Quota);
                    return Json(powerIndeces);
                }
                else
                {
                    PowerIndexs powerIndeces = Solver.CalcBGenerEquiles(request.Players, request.Quota);
                    return Json(powerIndeces);
                }
            }
            else
            {
                if (request.CalcType == 0)
                {
                    PowerIndexs powerIndeces = Solver.CalcBDirectExceeding(request.Players, request.Quota);
                    return Json(powerIndeces);
                }
                else
                {
                    PowerIndexs powerIndeces = Solver.CalcBGenerExceeding(request.Players, request.Quota);
                    return Json(powerIndeces);
                }
            }
        }
    }
}
