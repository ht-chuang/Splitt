using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SplittLib.Models
{
    public class Check
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Title { get; set; } = "New Check";

        [Required]
        public int OwnerId { get; set; }

        [Required]
        public virtual User Owner { get; set; } = null!;

        public DateTime Date { get; set; }

        public ICollection<CheckItem> CheckItems { get; set; } = new List<CheckItem>();

        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; } = 0m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Tax { get; set; } = 0m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Tip { get; set; } = 0m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; } = 0m;
    }
}
