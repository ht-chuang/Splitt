using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SplittLib.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(32, MinimumLength = 2, ErrorMessage = "The field must be between 2 and 32 characters.")]
        public required string Name { get; set; }

        [Required]
        [StringLength(32, MinimumLength = 3, ErrorMessage = "The field must be between 3 and 32 characters.")]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "The field must be alphanumeric.")]
        public required string Username { get; set; }

        [Required]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "The field must be exactly 4 characters.")]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "The field must be alphanumeric.")]
        public required string UsernameTag { get; set; }

        [EmailAddress]
        public string Email { get; set; } = "";

        [Phone]
        public string Phone { get; set; } = "";

        public ICollection<UserFriend> Friends { get; set; } = new List<UserFriend>();

        public ICollection<Check> Checks { get; set; } = new List<Check>();

        public ICollection<CheckMember> CheckMembers { get; set; } = new List<CheckMember>();
    }
}