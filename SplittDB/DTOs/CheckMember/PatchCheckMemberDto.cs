using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SplittDB.Attributes;

namespace SplittDB.DTOs.CheckMember
{
    public class PatchCheckMemberDto
    {
        [StringLength(50, MinimumLength = 1)]
        public string? Name { get; set; } = null!;

        [Range(1, int.MaxValue, ErrorMessage = "User ID must be a positive integer")]
        public int? UserId { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        [DecimalPrecision(2)]
        public decimal? AmountOwed { get; set; } = null!;
    }
}

