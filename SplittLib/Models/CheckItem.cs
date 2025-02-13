using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SplittLib.Models
{
    public class CheckItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Name { get; set; } = "New Item";

        [StringLength(255)]
        public string? Description { get; set; } = string.Empty;

        [Required]
        public int CheckId { get; set; }

        [Required]
        public Check Check { get; set; } = null!;

        [Range(0, int.MaxValue, ErrorMessage = "The field must be a valid positive integer.")]
        public int Quantity { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; } = 0m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; } = 0m;
    }
}