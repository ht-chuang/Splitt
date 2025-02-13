public class CheckDto
{
    public required string Title { get; set; }
    public required int OwnerId { get; set; }  // This replaces the Owner navigation property
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Tip { get; set; }
    public decimal Total { get; set; }
    public DateTime Date { get; set; }
    // Add any other properties needed for creation, but exclude navigation properties
}