namespace SplittDB.DTOs.Check
{
    public class CheckInfoDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public int? OwnerId { get; set; }

        public DateTime Date { get; set; }

        public decimal Subtotal { get; set; }

        public decimal Tax { get; set; }

        public decimal Tip { get; set; }

        public decimal Total { get; set; }
    }
}