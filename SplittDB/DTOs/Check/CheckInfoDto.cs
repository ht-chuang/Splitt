namespace SplittDB.DTOs.Check
{
    public class CheckInfoDto
    {
        public int? Id { get; set; } = null!;

        public string? Title { get; set; } = null!;

        public int? OwnerId { get; set; } = null!;

        public DateTime? Date { get; set; } = null!;

        public decimal? Subtotal { get; set; } = null!;

        public decimal? Tax { get; set; } = null!;

        public decimal? Tip { get; set; } = null!;

        public decimal? Total { get; set; } = null!;
    }
}