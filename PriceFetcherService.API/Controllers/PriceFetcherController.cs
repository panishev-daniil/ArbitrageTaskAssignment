using Microsoft.AspNetCore.Mvc;
using PriceFetcherService.Abstraction.Services;

namespace PriceFetcherService.API.Controllers
{
    [ApiController]
    [Route("api/price-fetcher")]
    public class PriceFetcherController : ControllerBase
    {
        private readonly IPriceFetcher _fetcher;

        public PriceFetcherController(IPriceFetcher fetcher)
        {
            _fetcher = fetcher;
        }

        [HttpGet("{symbol}")]
        public async Task<IActionResult> GetPrice(string symbol)
        {
            var price = await _fetcher.GetFuturePriceAsync(symbol);
            if (price == null)
                return NotFound("Price not found");

            return Ok(new { Symbol = symbol, Price = price });
        }
    }
}
