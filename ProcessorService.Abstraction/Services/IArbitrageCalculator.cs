using ProcessorService.Abstraction.Models.DTO;

namespace ProcessorService.Abstraction.Services
{
    public interface IArbitrageCalculator
    {
        Task<ArbitrageDifferenceDto?> CalculateDifferenceAsync(string symbol1, string symbol2);
    }
}
