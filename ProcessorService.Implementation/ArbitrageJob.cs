using Microsoft.Extensions.Configuration;
using ProcessorService.Abstraction.Services;
using Serilog;
using System.Text;
using System.Text.Json;

namespace ProcessorService.Implementation
{
    public class ArbitrageJob : IArbitrageJob
    {
        private readonly IArbitrageCalculator _calculator;
        private readonly HttpClient _httpClient;
        private readonly string _storagePostUrl;

        public ArbitrageJob(IArbitrageCalculator calculator, HttpClient httpClient, IConfiguration configuration)
        {
            _calculator = calculator;
            _httpClient = httpClient;
            _storagePostUrl = configuration["StorageApi:BaseUrl"]
                ?? throw new ArgumentNullException("StorageApi:BaseUrl is not configured");
        }

        public async Task ExecuteAsync()
        {
            // Для демонстрации, можно вынести в appsettings.json
            var symbol1 = "BTCUSDT_250328";
            var symbol2 = "BTCUSDT_250627";

            var result = await _calculator.CalculateDifferenceAsync(symbol1, symbol2);
            if (result != null)
            {
                Log.Information("Arbitrage difference calculated successfully. Sending result to StorageAPI.");

                var json = JsonSerializer.Serialize(result);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(_storagePostUrl, content);
                if (response.IsSuccessStatusCode)
                {
                    Log.Information("Arbitrage difference successfully stored.");
                }
                else
                {
                    Log.Warning("Failed to store arbitrage difference. Status code: {StatusCode}",
                        response.StatusCode);
                }
            }
            else
            {
                Log.Warning("Arbitrage difference calculation returned null.");
            }
        }
    }

}
