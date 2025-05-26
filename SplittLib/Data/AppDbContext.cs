using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SplittLib.Models;

namespace SplittLib.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> User { get; set; } = null!;
        public DbSet<UserFriend> UserFriend { get; set; } = null!;
        public DbSet<Check> Check { get; set; } = null!;
        public DbSet<CheckItem> CheckItem { get; set; } = null!;
        public DbSet<CheckMember> CheckMember { get; set; } = null!;

        // Converts DateTime to UTC for all DateTime properties
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<DateTime>()
                .HaveConversion<UtcDateTimeConverter>();

            configurationBuilder.Properties<DateTime?>()
                .HaveConversion<UtcNullableDateTimeConverter>();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new UserFriendConfiguration());
            modelBuilder.ApplyConfiguration(new CheckConfiguration());
            modelBuilder.ApplyConfiguration(new CheckItemConfiguration());
            modelBuilder.ApplyConfiguration(new CheckMemberConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }

    public class UtcDateTimeConverter : ValueConverter<DateTime, DateTime>
    {
        public UtcDateTimeConverter() : base(
            v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
        { }
    }

    public class UtcNullableDateTimeConverter : ValueConverter<DateTime?, DateTime?>
    {
        public UtcNullableDateTimeConverter() : base(
            v => v.HasValue ? (v.Value.Kind == DateTimeKind.Utc ? v.Value : DateTime.SpecifyKind(v.Value, DateTimeKind.Utc)) : v,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v)
        { }
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
                   .OnDelete(DeleteBehavior.Restrict);

            // Friend -> UserFriend relationship
            builder.HasOne(uf => uf.Friend)
                   .WithMany()
                   .HasForeignKey(uf => uf.FriendId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(uf => new { uf.UserId, uf.FriendId }).IsUnique();
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

            // Check -> CheckMember relationship
            builder.HasMany(c => c.CheckMembers)
                   .WithOne(cm => cm.Check)
                   .HasForeignKey(cm => cm.CheckId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

    internal class CheckItemConfiguration : IEntityTypeConfiguration<CheckItem>
    {
        public void Configure(EntityTypeBuilder<CheckItem> builder)
        {
            builder.ToTable("CheckItem");
            builder.HasKey(ci => ci.Id);
        }
    }

    internal class CheckMemberConfiguration : IEntityTypeConfiguration<CheckMember>
    {
        public void Configure(EntityTypeBuilder<CheckMember> builder)
        {
            builder.ToTable("CheckMember");
            builder.HasKey(cm => cm.Id);

            // User -> CheckMember relationship
            builder.HasOne(cm => cm.User)
                .WithMany(u => u.CheckMembers)
                .HasForeignKey(cm => cm.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
