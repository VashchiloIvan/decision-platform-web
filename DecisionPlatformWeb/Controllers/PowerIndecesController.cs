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

        [HttpPost("calculate")]
        public IActionResult CalculatePowerIndeces([FromBody] CalculateRequest request)
        {
            Players powerIndeces = Solver.calculatePowerIndeces(request.Players, request.Quota, request.QuotaType, request.CalculationType, request.MethodType);
            return Json(powerIndeces);
        }
    }
}
