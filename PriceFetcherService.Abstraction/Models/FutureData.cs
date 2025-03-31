namespace PriceFetcherService.Abstraction.Models
{
    public class FutureData
    {
        public string Symbol { get; set; }
        public decimal? Price { get; set; }
        public DateTime RetrievedAt { get; set; }

        public FutureData(string symbol,
            decimal? price)
        {
            Symbol = symbol;
            Price = price;
            RetrievedAt = DateTime.UtcNow;
        }
    }
}
