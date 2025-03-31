using StorageService.Shared.DAL;

namespace StorageService.Abstraction.Models
{
    public class ArbitrageDifference : Base
    {
        public string Symbol1 { get; set; }
        public string Symbol2 { get; set; }
        public decimal Price1 { get; set; }
        public decimal Price2 { get; set; }
        public decimal Difference { get; set; }
        public DateTime CalculatedAt { get; set; }
    }
}
