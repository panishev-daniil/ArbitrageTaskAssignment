using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Serilog;
using StorageService.Abstraction.Models;
using StorageService.Abstraction.Models.Dtos;
using StorageService.Abstraction.Repositories;
using StorageService.Shared.DAL;

namespace StorageService.DAL.Repositories
{
    public class ArbitrageDifferenceRepository : Repository<ArbitrageDifference>, IArbitrageDifferenceRepository
    {
        private readonly DbContext _context;
        private readonly IMapper _mapper;

        public ArbitrageDifferenceRepository(AppDbContext context, IMapper mapper) : base(context)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ArbitrageDifference> CreateArbitrageDifferenceAsync(ArbitrageDifferenceDto arbitrageDifference)
        {
            var existingArbitrageDifference = await GetAsync(arbitrageDifference.Id);

            if (existingArbitrageDifference is not null)
            {
                Log.Warning("Attempt to create an arbitrage difference that already exists: {arbitrageDifferenceId}",
                    arbitrageDifference.Id);
            }

            var arbitrageDifferenceDto = _mapper.Map<ArbitrageDifference>(arbitrageDifference);
            var createdArbitrageDifference = await InsertAsync(arbitrageDifferenceDto);
            _context.SaveChanges();

            Log.Information("Arbitrage difference created successfully: {arbitrageDifferenceId}",
                createdArbitrageDifference.Id);
            return createdArbitrageDifference;
        }

        public async Task<ArbitrageDifference> DeleteArbitrageDifferenceByIdAsync(Guid id)
        {
            var arbitrageDifference = await GetAsync(id);

            if (arbitrageDifference is not null)
            {
                Log.Warning("Attempt to create an arbitrage difference that already exists: {arbitrageDifferenceId}",
                    arbitrageDifference.Id);
            }

            await DeleteAsync(arbitrageDifference);
            _context.SaveChanges();

            Log.Information("Arbitrage difference created successfully: {arbitrageDifferenceId}",
                arbitrageDifference.Id);
            return arbitrageDifference;
        }

        public async Task<IEnumerable<ArbitrageDifference?>> GetAllArbitrageDifferenceAsync()
        {
            var arbitrageDifferences = await GetAsync();

            if (!arbitrageDifferences.Any())
            {
                Log.Information("No arbitrage difference found in the table.");
                throw new ArgumentException("ARBITRAGE_DIFFERENCES_IS_NOT_FOUND");
            }

            return arbitrageDifferences;
        }

        public async Task<ArbitrageDifference?> GetArbitrageDifferenceByIdAsync(Guid id)
        {
            var arbitrageDifference = await GetAsync(id);

            if (arbitrageDifference is null)
            {
                Log.Information("Arbitrage difference with Id {arbitrageDifferenceId} is not found.",
                    arbitrageDifference.Id);
                throw new ArgumentException("ARBITRAGE_DIFFERENCE_IS_NOT_FOUND");
            }

            return arbitrageDifference;
        }

        public async Task<ArbitrageDifference> UpdateArbitrageDifferenceAsync(ArbitrageDifferenceDto arbitrageDifference)
        {
            var oldArbitrageDifference = await GetAsync(arbitrageDifference.Id);

            if (oldArbitrageDifference is null)
            {
                Log.Warning("Arbitrage difference with Id {arbitrageDifferenceId} is not found.",
                    arbitrageDifference.Id);
                throw new ArgumentException("ARBITRAGE_DIFFERENCE_IS_NOT_FOUND");
            }

            _mapper.Map(arbitrageDifference, oldArbitrageDifference);

            await UpdateAsync(oldArbitrageDifference);
            _context.SaveChanges();

            return null;
        }
    }
}
