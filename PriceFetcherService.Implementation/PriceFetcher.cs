using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using PriceFetcherService.Abstraction.Services;
using Serilog;
using System.Text;

namespace PriceFetcherService.Implementation
{
    public class PriceFetcher : IPriceFetcher
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public PriceFetcher(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _baseUrl = configuration["BinanceApi:BaseUrl"] 
                ?? throw new ArgumentNullException("BinanceApi:BaseUrl is not configured");
        }

        public async Task<decimal?> GetFuturePriceAsync(string symbol)
        {
            try
            {
                var urlBuilder = new StringBuilder(_baseUrl).Append(symbol);
                HttpResponseMessage response = await _httpClient.GetAsync(urlBuilder.ToString());

                if (!response.IsSuccessStatusCode)
                {
                    Log.Warning($"Request error: {response.StatusCode}");
                    return null;
                }

                string jsonResponse = await response.Content.ReadAsStringAsync();
                JObject data = JObject.Parse(jsonResponse);
                return data["price"]?.Value<decimal>();
            }
            catch (Exception ex)
            {
                Log.Error($"Error when receiving data from Binance: {ex.Message}");
                return null;
            }
        }
    }
}
