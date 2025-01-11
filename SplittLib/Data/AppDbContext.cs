using Microsoft.EntityFrameworkCore;
using SplittLib.Models;

namespace SplittLib.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public required DbSet<User> User { get; set; }
        public required DbSet<UserFriend> UserFriend { get; set; }
        public required DbSet<Check> Check { get; set; }
        public required DbSet<CheckItem> CheckItem { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User Entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.HasIndex(u => new { u.Username, u.UsernameTag }).IsUnique();
            });

            modelBuilder.Entity<UserFriend>(entity =>
            {
                entity.HasKey(uf => new { uf.UserId, uf.FriendId });

                // User -> UserFriend relationship
                entity.HasOne(uf => uf.User)
                    .WithMany(u => u.Friends)
                    .HasForeignKey(uf => uf.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Friend -> UserFriend relationship
                entity.HasOne(uf => uf.Friend)
                    .WithMany()
                    .HasForeignKey(uf => uf.FriendId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Check>(entity =>
            {
                entity.HasKey(c => c.Id);

                // User -> Check relationship
                entity.HasOne(c => c.Owner)
                      .WithMany(u => u.Checks)
                      .HasForeignKey(c => c.OwnerId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Check -> CheckItem relationship
                entity.HasMany(c => c.CheckItems)
                      .WithOne(ci => ci.Check)
                      .HasForeignKey(ci => ci.CheckId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
