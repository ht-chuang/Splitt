using Bogus;
using Bogus.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using SplittLib.Models;

namespace SplittLib.Data
{
    public class SeedConfiguration
    {
        public int NumberOfUsers { get; set; } = 10;
        public int NumberOfChecks { get; set; } = 15;
        public int MaxFriendsPerUser { get; set; } = 3;
        public int MaxItemsPerCheck { get; set; } = 10;
        public int MaxMembersPerCheck { get; set; } = 5;
    }

    public class DbInitializer
    {
        private readonly SeedConfiguration _config;
        private readonly ILogger<DbInitializer> _logger;

        public IReadOnlyCollection<User> Users { get; }
        public IReadOnlyCollection<UserFriend> UserFriends { get; }
        public IReadOnlyCollection<Check> Checks { get; }
        public IReadOnlyCollection<CheckItem> CheckItems { get; }
        public IReadOnlyCollection<CheckMember> CheckMembers { get; }

        public DbInitializer(int seed, SeedConfiguration config, ILogger<DbInitializer> logger)
        {
            _config = config;
            _logger = logger;

            _logger.LogInformation("Initializing database seeder with seed: {Seed}", seed);
            Randomizer.Seed = new Random(seed); // Ensures consistent seeding

            _logger.LogInformation("Generating entities...");
            Users = GenerateUsers(_config.NumberOfUsers);
            UserFriends = GenerateUserFriends(Users, _config.MaxFriendsPerUser);
            Checks = GenerateChecks(_config.NumberOfChecks, Users);
            CheckItems = GenerateCheckItems(Checks, _config.MaxItemsPerCheck);
            CheckMembers = GenerateCheckMembers(Checks, _config.MaxMembersPerCheck);
        }

        public async Task SeedAsync(AppDbContext context)
        {
            _logger.LogInformation("Starting database seeding process");
            try
            {
                _logger.LogInformation("Ensuring clean database state");
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();

                _logger.LogInformation("Adding entities to the database...");
                await context.User.AddRangeAsync(Users);
                await context.UserFriend.AddRangeAsync(UserFriends);
                await context.Check.AddRangeAsync(Checks);
                await context.CheckItem.AddRangeAsync(CheckItems);
                await context.CheckMember.AddRangeAsync(CheckMembers);

                _logger.LogInformation("Saving changes to database");
                await context.SaveChangesAsync();

                _logger.LogInformation("Resetting database sequences");
                await ResetSequencesAsync(context);

                _logger.LogInformation("Database seeding completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database");
                throw;
            }
        }

        private async Task ResetSequencesAsync(AppDbContext context)
        {
            foreach (var entityType in context.Model.GetEntityTypes())
            {
                var primaryKey = entityType.FindPrimaryKey();
                if (primaryKey?.Properties == null || !primaryKey.Properties.Any())
                {
                    _logger.LogWarning("Skipping sequence reset for entity {EntityType} - no primary key found",
                entityType.Name);
                    continue;
                }

                var tableName = entityType.GetTableName();
                var idProperty = primaryKey.Properties.First();

                if (idProperty.ValueGenerated == ValueGenerated.OnAdd)
                {
                    _logger.LogDebug("Resetting sequence for table {TableName}", tableName);
#pragma warning disable EF1002
                    await context.Database.ExecuteSqlRawAsync(
                        $"SELECT setval(pg_get_serial_sequence('\"{tableName}\"', '{idProperty.Name}'), (SELECT MAX(\"{idProperty.Name}\") FROM \"{tableName}\"));");
#pragma warning restore EF1002
                }
            }
        }

        private static IReadOnlyCollection<User> GenerateUsers(int amount)
        {
            int userId = 1;
            var userFaker = new Faker<User>()
                .RuleFor(x => x.Id, f => userId++)
                .RuleFor(x => x.Name, f => f.Name.FullName().ClampLength(2, 32))
                .RuleFor(x => x.Username, (f, u) => f.Internet.UserName(u.Name).ClampLength(3, 32))
                .RuleFor(x => x.UsernameTag, f => f.Random.AlphaNumeric(4))
                .RuleFor(x => x.Email, (f, u) => f.Internet.Email(u.Name, provider: "splitt.com"))
                .RuleFor(x => x.Phone, f => f.Phone.PhoneNumber("###-###-####"));

            return userFaker.Generate(amount);
        }

        private static IReadOnlyCollection<UserFriend> GenerateUserFriends(IEnumerable<User> users, int maxFriendsPerUser)
        {
            var userFriends = new List<UserFriend>();

            if (users.Count() < 2)
                return userFriends;

            maxFriendsPerUser = Math.Min(users.Count() - 1, maxFriendsPerUser);
            foreach (var user in users)
            {
                var faker = new Faker();
                var numberOfFriends = faker.Random.Number(0, maxFriendsPerUser);
                var friends = faker.PickRandom(users.Where(u => u.Id != user.Id), numberOfFriends);

                userFriends.AddRange(friends.Select(friend => new UserFriend
                {
                    User = user,
                    Friend = friend,
                }));
            }

            return userFriends;
        }

        private static IReadOnlyCollection<Check> GenerateChecks(int amount, IEnumerable<User> users)
        {
            int checkId = 1;
            var checkFaker = new Faker<Check>()
                .RuleFor(x => x.Id, f => checkId++)
                .RuleFor(x => x.Title, f => f.Lorem.Sentence().ClampLength(1, 50))
                .RuleFor(x => x.Owner, f => f.PickRandom(users))
                .RuleFor(x => x.Date, f => f.Date.Past().ToUniversalTime());

            return checkFaker.Generate(amount);
        }

        private static IReadOnlyCollection<CheckItem> GenerateCheckItems(IEnumerable<Check> checks, int maxItemsPerCheck)
        {
            var checkItems = new List<CheckItem>();
            if (!checks.Any())
                return checkItems;

            int checkItemId = 1;
            foreach (var check in checks)
            {
                var checkItemFaker = new Faker<CheckItem>()
                    .RuleFor(x => x.Id, f => checkItemId++)
                    .RuleFor(x => x.Name, f => f.Commerce.ProductName().ClampLength(1, 50))
                    .RuleFor(x => x.Description, f => f.Lorem.Sentence().ClampLength(0, 255))
                    .RuleFor(x => x.Check, f => check)
                    .RuleFor(x => x.Quantity, f => f.Random.Number(1, 6))
                    .RuleFor(x => x.UnitPrice, f => Math.Round(f.Random.Decimal(1, 100), 2))
                    .RuleFor(x => x.TotalPrice, (f, ci) => Math.Round(ci.Quantity * ci.UnitPrice, 2));

                var faker = new Faker();
                var numberOfItems = faker.Random.Number(0, maxItemsPerCheck);
                var subCheckItems = checkItemFaker.Generate(numberOfItems);

                check.Subtotal = Math.Round(subCheckItems.Sum(ci => ci.TotalPrice), 2);
                check.Tax = Math.Round(faker.Random.Decimal(0m, 0.07m) * check.Subtotal, 2);
                check.Tip = Math.Round(faker.Random.Decimal(0m, 0.30m) * check.Subtotal, 2);
                check.Total = Math.Round(check.Subtotal + check.Tax + check.Tip, 2);

                checkItems.AddRange(subCheckItems);
            }

            return checkItems;
        }

        private static IReadOnlyCollection<CheckMember> GenerateCheckMembers(IEnumerable<Check> checks, int maxMembersPerCheck)
        {
            var checkMembers = new List<CheckMember>();
            if (!checks.Any())
                return checkMembers;

            int checkMemberId = 1;
            foreach (var check in checks)
            {
                var faker = new Faker();
                var numberOfMembers = faker.Random.Number(1, maxMembersPerCheck);

                // Ensure that the owner is always included in the members
                var owner = new CheckMember
                {
                    Id = checkMemberId++,
                    Name = check.Owner.Name,
                    Check = check,
                    User = check.Owner,
                    AmountOwed = check.Total
                };
                checkMembers.Add(owner);

                var checkMemberFaker = new Faker<CheckMember>()
                    .RuleFor(x => x.Id, f => checkMemberId++)
                    .RuleFor(x => x.Name, f => f.Name.FullName().ClampLength(2, 32))
                    .RuleFor(x => x.Check, f => check);
                var subCheckMembers = checkMemberFaker.Generate(numberOfMembers - 1);

                checkMembers.AddRange(subCheckMembers);
            }

            return checkMembers;
        }
    }
}
