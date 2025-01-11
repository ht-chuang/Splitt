using System.ComponentModel.DataAnnotations;

namespace SplittLib.Models
{
    public class Check
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = "New Check";
        [Required]
        public int OwnerId { get; set; }
        [Required]
        public required User Owner { get; set; }
        public DateTime Date { get; set; }
        public ICollection<CheckItem> CheckItems { get; set; } = new List<CheckItem>();
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Tip { get; set; }
        public decimal Total { get; set; }
    }
}
