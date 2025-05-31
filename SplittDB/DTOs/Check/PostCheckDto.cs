using System.ComponentModel.DataAnnotations;

namespace SplittDB.DTOs.Check
{
    public class PostCheckDto
    {
        [StringLength(50, MinimumLength = 1)]
        public string? Title { get; set; }

        public DateTime? Date { get; set; }

        [Required(ErrorMessage = "Owner ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Owner ID must be a positive integer")]
        public int? OwnerId { get; set; }
    }
}

