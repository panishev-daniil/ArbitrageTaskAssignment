using StorageService.Abstraction.Models;
using StorageService.Abstraction.Models.Dtos;

namespace StorageService.Abstraction.Repositories
{
    public interface IArbitrageDifferenceRepository
    {
        Task<ArbitrageDifference?> GetArbitrageDifferenceByIdAsync(Guid id);
        Task<IEnumerable<ArbitrageDifference?>> GetAllArbitrageDifferenceAsync();
        Task<ArbitrageDifference> CreateArbitrageDifferenceAsync(ArbitrageDifferenceDto arbitrageDifference);
        Task<ArbitrageDifference> UpdateArbitrageDifferenceAsync(ArbitrageDifferenceDto arbitrageDifference);
        Task<ArbitrageDifference> DeleteArbitrageDifferenceByIdAsync(Guid id);
    }
}
