using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SplittDB.Attributes;

namespace SplittDB.DTOs.Check
{
    public class PatchCheckDto
    {
        [StringLength(50, MinimumLength = 1)]
        public string? Title { get; set; }

        public DateTime? Date { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [DecimalPrecision(2)]
        public decimal? Subtotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [DecimalPrecision(2)]
        public decimal? Tax { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [DecimalPrecision(2)]
        public decimal? Tip { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [DecimalPrecision(2)]
        public decimal? Total { get; set; }
    }
}

