using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SplittDB.Attributes;

namespace SplittDB.DTOs.CheckMember
{
    public class PatchCheckMemberDto : PatchRequestDto
    {
        [StringLength(50, MinimumLength = 1)]
        public string? Name { get; set; } = null!;

        private int? _userId;
        [Range(1, int.MaxValue, ErrorMessage = "User ID must be a positive integer")]
        public int? UserId
        {
            get => _userId;
            set
            {
                _userId = value;
                SetHasProperty(nameof(UserId));
            }
        }

        [Column(TypeName = "decimal(18,2)")]
        [DecimalPrecision(2)]
        public decimal? AmountOwed { get; set; } = null!;
    }
}

