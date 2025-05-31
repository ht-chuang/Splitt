using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SplittDB.Attributes;

namespace SplittDB.DTOs.CheckItem
{
    public class PostCheckItemDto
    {
        [StringLength(50, MinimumLength = 1)]
        public string? Name { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Check ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Check ID must be a positive integer")]
        public int? CheckId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a valid positive integer.")]
        public int? Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [DecimalPrecision(2)]
        public decimal? UnitPrice { get; set; }
    }
}