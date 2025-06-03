namespace SplittDB.DTOs.CheckItem
{
    public class CheckItemInfoDto
    {
        public int? Id { get; set; } = null!;

        public string? Name { get; set; } = null!;

        public string? Description { get; set; } = null!;

        public int? CheckId { get; set; } = null!;

        public int? Quantity { get; set; } = null!;

        public decimal? UnitPrice { get; set; } = null;

        public decimal? TotalPrice { get; set; } = null!;
    }
}