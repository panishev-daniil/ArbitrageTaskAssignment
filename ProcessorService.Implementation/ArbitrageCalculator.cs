using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using ProcessorService.Abstraction.Models.DTO;
using ProcessorService.Abstraction.Services;
using Serilog;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace ProcessorService.Implementation
{
    public class ArbitrageCalculator : IArbitrageCalculator
    {
        private readonly HttpClient _httpClient;
        private readonly IDistributedCache _cache;
        private readonly string _fetcherBaseUrl;
        private readonly string _storageBaseUrl;
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);

        public ArbitrageCalculator(HttpClient httpClient, IDistributedCache cache, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _cache = cache;
            _fetcherBaseUrl = configuration["FetcherApi:BaseUrl"]
                ?? throw new ArgumentNullException("FetcherApi:BaseUrl is not configured");
            _storageBaseUrl = configuration["StorageApi:BaseUrl"]
                ?? throw new ArgumentNullException("StorageApi:BaseUrl is not configured");
        }

        private string BuildUrl(string baseUrl, string symbol)
        {
            var sb = new StringBuilder(baseUrl);
            if (!baseUrl.EndsWith("/"))
                sb.Append("/");
            sb.Append(symbol);

            string finalUrl = sb.ToString();
            Log.Information("Generated API URL: {FinalUrl}",
                finalUrl);
            return finalUrl;
        }

        private string BuildFetcherUrl(string symbol) => BuildUrl(_fetcherBaseUrl, symbol);
        private string BuildStorageUrl(string symbol) => BuildUrl(_storageBaseUrl, symbol);

        private async Task<decimal?> GetCachedPriceAsync(string symbol)
        {
            var cachedPrice = await _cache.GetStringAsync(symbol);
            if (!string.IsNullOrEmpty(cachedPrice))
            {
                Log.Information("Cache hit for {Symbol}: {Price}", symbol, cachedPrice);
                return decimal.Parse(cachedPrice);
            }
            return null;
        }

        private async Task SetCachedPriceAsync(string symbol, decimal price)
        {
            await _cache.SetStringAsync(symbol, price.ToString(), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _cacheExpiration
            });
            Log.Information("Cached price for {Symbol}: {Price}", symbol, price);
        }

        private async Task<PriceResponseDto?> GetPriceResponseAsync(string symbol)
        {
            var url = BuildFetcherUrl(symbol);
            try
            {
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<PriceResponseDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                else
                {
                    Log.Warning("Failed to fetch price for {Symbol} from FetcherService. Status code: {StatusCode}",
                        symbol, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while fetching data for {Symbol} from FetcherService.",
                    symbol);
            }
            return null;
        }

        private async Task<decimal?> GetCachedFallbackPriceAsync(string symbol)
        {
            var cachedPrice = await GetCachedPriceAsync(symbol);
            if (cachedPrice.HasValue)
                return cachedPrice.Value;

            var priceResponse = await GetPriceResponseAsync(symbol);
            if (priceResponse != null)
            {
                await SetCachedPriceAsync(symbol, priceResponse.Price);
                return priceResponse.Price;
            }

            var fallbackPrice = await GetFallbackPriceAsync(symbol);
            if (fallbackPrice.HasValue)
            {
                await SetCachedPriceAsync(symbol, fallbackPrice.Value);
                return fallbackPrice.Value;
            }

            Log.Warning("No price data available for {Symbol}", symbol);
            return null;
        }

        private async Task<decimal?> GetFallbackPriceAsync(string symbol)
        {
            var url = BuildStorageUrl(symbol);
            try
            {
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var storedResponse = JsonSerializer.Deserialize<PriceResponseDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (storedResponse != null)
                        return storedResponse.Price;
                }
                else
                {
                    Log.Warning("Failed to fetch fallback price for {Symbol} from StorageAPI. Status code: {StatusCode}",
                        symbol, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while fetching fallback price for {Symbol} from StorageAPI.",
                    symbol);
            }
            return null;
        }

        private async Task<PriceResponseDto?> GetPriceOrFallbackResponseAsync(string symbol)
        {
            var priceResponse = await GetPriceResponseAsync(symbol);
            if (priceResponse != null)
                return priceResponse;

            var fallbackPrice = await GetCachedFallbackPriceAsync(symbol);
            if (fallbackPrice.HasValue)
            {
                Log.Information("Using fallback price for {Symbol}: {FallbackPrice}",
                    symbol, fallbackPrice.Value);
                return new PriceResponseDto { Symbol = symbol, Price = fallbackPrice.Value, RetrievedAt = DateTime.UtcNow };
            }
            Log.Warning("No price data available for {Symbol}",
                symbol);
            return null;
        }

        public async Task<ArbitrageDifferenceDto?> CalculateDifferenceAsync(string symbol1, string symbol2)
        {
            var priceResponse1 = await GetPriceOrFallbackResponseAsync(symbol1);
            var priceResponse2 = await GetPriceOrFallbackResponseAsync(symbol2);

            if (priceResponse1 != null && priceResponse2 != null)
            {
                var difference = Math.Abs(priceResponse1.Price - priceResponse2.Price);
                Log.Information("Calculated price difference: {Difference}",
                    difference);
                return new ArbitrageDifferenceDto
                {
                    Symbol1 = symbol1,
                    Symbol2 = symbol2,
                    Price1 = priceResponse1.Price,
                    Price2 = priceResponse2.Price,
                    Difference = difference,
                    CalculatedAt = DateTime.UtcNow
                };
            }

            Log.Warning("Failed to obtain prices for both symbols even with fallback.");
            return null;
        }
    }
}
