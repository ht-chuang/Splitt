using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SplittLib.Models;

namespace SplittLib.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        [Required]
        public DbSet<User> User { get; set; } = null!;
        [Required]
        public DbSet<UserFriend> UserFriend { get; set; } = null!;
        [Required]
        public DbSet<Check> Check { get; set; } = null!;
        [Required]
        public DbSet<CheckItem> CheckItem { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new UserFriendConfiguration());
            modelBuilder.ApplyConfiguration(new CheckConfiguration());
            modelBuilder.ApplyConfiguration(new CheckItemConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }

    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("User");
            builder.HasKey(u => u.Id);
            builder.HasIndex(u => new { u.Username, u.UsernameTag }).IsUnique();
        }
    }

    internal class UserFriendConfiguration : IEntityTypeConfiguration<UserFriend>
    {
        public void Configure(EntityTypeBuilder<UserFriend> builder)
        {
            builder.ToTable("UserFriend");
            builder.HasKey(uf => new { uf.UserId, uf.FriendId });

            // User -> UserFriend relationship
            builder.HasOne(uf => uf.User)
                   .WithMany(u => u.Friends)
                   .HasForeignKey(uf => uf.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Friend -> UserFriend relationship
            builder.HasOne(uf => uf.Friend)
                   .WithMany()
                   .HasForeignKey(uf => uf.FriendId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

    internal class CheckConfiguration : IEntityTypeConfiguration<Check>
    {
        public void Configure(EntityTypeBuilder<Check> builder)
        {
            builder.ToTable("Check");
            builder.HasKey(c => c.Id);

            // User -> Check relationship
            builder.HasOne(c => c.Owner)
                   .WithMany(u => u.Checks)
                   .HasForeignKey(c => c.OwnerId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Check -> CheckItem relationship
            builder.HasMany(c => c.CheckItems)
                   .WithOne(ci => ci.Check)
                   .HasForeignKey(ci => ci.CheckId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

    internal class CheckItemConfiguration : IEntityTypeConfiguration<CheckItem>
    {
        public void Configure(EntityTypeBuilder<CheckItem> builder)
        {
            builder.ToTable("CheckItem");
            builder.HasKey(ci => ci.Id);

            // CheckItem -> Check relationship
            builder.HasOne(ci => ci.Check)
                   .WithMany(c => c.CheckItems)
                   .HasForeignKey(ci => ci.CheckId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
