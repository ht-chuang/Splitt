using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SplittLib.Models
{
    public class CheckMember
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Name { get; set; } = "New Friend";

        [Required]
        public int? CheckId { get; set; }

        [Required]
        public Check Check { get; set; } = null!;

        public int? UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        public decimal AmountOwed { get; set; } = 0m;
    }
}