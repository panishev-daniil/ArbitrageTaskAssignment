namespace PriceFetcherService.Abstraction.Services
{
    public interface IPriceFetcher
    {
        Task<decimal?> GetFuturePriceAsync(string symbol);
    }
}
