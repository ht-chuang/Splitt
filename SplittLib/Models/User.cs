using System.ComponentModel.DataAnnotations;

namespace SplittLib.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public required string Name { get; set; }
        [Required]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "The field must be alphanumeric.")]
        public required string Username { get; set; }
        [Required]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "The field must be exactly 4 characters.")]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "The field must be alphanumeric.")]
        public required string UsernameTag { get; set; }
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public ICollection<UserFriend> Friends { get; set; } = new List<UserFriend>();
        public ICollection<Check> Checks { get; set; } = new List<Check>();
    }
}