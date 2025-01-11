using System.ComponentModel.DataAnnotations;

namespace SplittLib.Models
{
    public class CheckItem
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = "New Item";
        public string Description { get; set; } = "";
        [Required]
        public int CheckId { get; set; }
        [Required]
        public required Check Check { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}