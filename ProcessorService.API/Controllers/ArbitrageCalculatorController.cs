using Microsoft.AspNetCore.Mvc;
using ProcessorService.Abstraction.Services;

namespace ProcessorService.API.Controllers
{
    [ApiController]
    [Route("api/arbitrage-calculator")]
    public class ArbitrageCalculatorController : ControllerBase
    {
        private readonly IArbitrageCalculator _arbitrageCalculator;

        public ArbitrageCalculatorController(IArbitrageCalculator arbitrageCalculator)
        {
            _arbitrageCalculator = arbitrageCalculator;
        }

        [HttpGet]
        public async Task<IActionResult> CalculateSymbolsDifference(string symbol1, string symbol2)
        {
            var arbitrageDifferenceDto = await _arbitrageCalculator.CalculateDifferenceAsync(symbol1, symbol2);
            if (arbitrageDifferenceDto == null)
                return NotFound("Failed to obtain prices for both symbols even with fallback.");

            return Ok(arbitrageDifferenceDto);
        }
    }
}
