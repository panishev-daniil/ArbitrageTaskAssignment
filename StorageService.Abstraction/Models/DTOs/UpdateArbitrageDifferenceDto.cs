namespace StorageService.Abstraction.Models.Dtos
{
    public record UpdateArbitrageDifferenceDto(
        string Symbol1,
        string Symbol2,
        decimal Price1,
        decimal Price2,
        decimal Difference,
        DateTime CalculatedAt
    );
}
