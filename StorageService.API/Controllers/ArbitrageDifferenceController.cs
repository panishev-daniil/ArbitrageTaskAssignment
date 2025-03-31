using Microsoft.AspNetCore.Mvc;
using StorageService.Abstraction.Models.Dtos;
using StorageService.Abstraction.Repositories;

namespace StorageService.API.Controllers
{
    [ApiController]
    [Route("api/arbitrage-difference")]
    public class ArbitrageDifferenceController : ControllerBase
    {
        private readonly IArbitrageDifferenceRepository _arbitrageRepository;

        public ArbitrageDifferenceController(IArbitrageDifferenceRepository arbitrageRepository)
        {
            _arbitrageRepository = arbitrageRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateArbitrageDiffernce(ArbitrageDifferenceDto arbitrageDifferenceDto)
        {
            if (arbitrageDifferenceDto == null)
                return NotFound("");

            await _arbitrageRepository.CreateArbitrageDifferenceAsync(arbitrageDifferenceDto);

            return Ok(arbitrageDifferenceDto);
        }
    }
}
