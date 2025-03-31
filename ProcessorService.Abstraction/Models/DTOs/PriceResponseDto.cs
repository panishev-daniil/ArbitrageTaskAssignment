namespace ProcessorService.Abstraction.Models.DTO
{
    public class PriceResponseDto
    {
        public string Symbol { get; set; }
        public decimal Price { get; set; }
        public DateTime RetrievedAt { get; set; }
    }
}
