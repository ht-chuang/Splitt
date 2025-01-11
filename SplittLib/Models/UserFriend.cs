using System.ComponentModel.DataAnnotations;

namespace SplittLib.Models
{
    public class UserFriend
    {
        [Key]
        [Required]
        public int UserId { get; set; } // Foreign key to the User
        [Required]
        public required User User { get; set; }

        [Key]
        [Required]
        public int FriendId { get; set; } // Foreign key to the "friend" User
        [Required]
        public required User Friend { get; set; }
    }
}