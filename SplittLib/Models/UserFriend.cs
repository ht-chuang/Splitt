using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SplittLib.Models
{
    public class UserFriend
    {
        [Key]
        [Column(Order = 0)]
        [Required]
        public int? UserId { get; set; } // Foreign key to the User

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        [Key]
        [Column(Order = 1)]
        [Required]
        public int? FriendId { get; set; } // Foreign key to the "friend" User

        [ForeignKey(nameof(FriendId))]
        public User Friend { get; set; } = null!;
    }
}