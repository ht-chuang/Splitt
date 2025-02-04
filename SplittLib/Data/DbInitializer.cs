using Bogus;
using Microsoft.EntityFrameworkCore;
using SplittLib.Models;

namespace SplittLib.Data
{
    public class DbInitializer
    {
        public IReadOnlyCollection<User> Users { get; }
        public IReadOnlyCollection<UserFriend> UserFriends { get; }
        public IReadOnlyCollection<Check> Checks { get; }
        public IReadOnlyCollection<CheckItem> CheckItems { get; }

        public DbInitializer(int seed)
        {
            Randomizer.Seed = new Random(seed); // Ensures consistent seeding
            Users = GenerateUsers(10);
            UserFriends = GenerateUserFriends(Users);
            Checks = GenerateChecks(15, Users);
            CheckItems = GenerateCheckItems(Checks);
        }

        public void Seed(AppDbContext context)
        {
            context.Database.EnsureCreated(); // Ensure database is created

            context.User.AddRange(Users);
            context.UserFriend.AddRange(UserFriends);
            context.Check.AddRange(Checks);
            context.CheckItem.AddRange(CheckItems);
            context.SaveChanges();

            context.Database.ExecuteSqlRaw("SELECT setval(pg_get_serial_sequence('\"Users\"', 'Id'), COALESCE((SELECT MAX(\"Id\") FROM \"Users\"), 1), false);");
            context.Database.ExecuteSqlRaw("SELECT setval(pg_get_serial_sequence('\"UserFriends\"', 'Id'), COALESCE((SELECT MAX(\"Id\") FROM \"UserFriends\"), 1), false);");
            context.Database.ExecuteSqlRaw("SELECT setval(pg_get_serial_sequence('\"Checks\"', 'Id'), COALESCE((SELECT MAX(\"Id\") FROM \"Checks\"), 1), false);");
            context.Database.ExecuteSqlRaw("SELECT setval(pg_get_serial_sequence('\"CheckItems\"', 'Id'), COALESCE((SELECT MAX(\"Id\") FROM \"CheckItems\"), 1), false);");
        }

        private static IReadOnlyCollection<User> GenerateUsers(int amount)
        {
            int userId = 1;
            var userFaker = new Faker<User>()
                .RuleFor(x => x.Id, f => userId++)
                .RuleFor(x => x.Name, f => f.Name.FullName())
                .RuleFor(x => x.Username, (f, u) => f.Internet.UserName(u.Name))
                .RuleFor(x => x.UsernameTag, f => f.Random.AlphaNumeric(4))
                .RuleFor(x => x.Email, (f, u) => f.Internet.Email(u.Name, provider: "splitt.com"))
                .RuleFor(x => x.Phone, f => f.Phone.PhoneNumber("###-###-####"));

            return userFaker.Generate(amount);
        }

        private static IReadOnlyCollection<UserFriend> GenerateUserFriends(IEnumerable<User> users)
        {
            var userFriends = new List<UserFriend>();

            if (users.Count() < 2)
                return userFriends;

            foreach (var user in users)
            {
                var faker = new Faker();
                var numberOfFriends = faker.Random.Number(0, 3);
                var friends = faker.PickRandom(users.Where(u => u.Id != user.Id), numberOfFriends);

                userFriends.AddRange(friends.Select(friend => new UserFriend
                {
                    UserId = user.Id,
                    FriendId = friend.Id,
                }));
            }

            return userFriends;
        }

        private static IReadOnlyCollection<Check> GenerateChecks(int amount, IEnumerable<User> users)
        {
            int checkId = 1;
            var checkFaker = new Faker<Check>()
                .RuleFor(x => x.Id, f => checkId++)
                .RuleFor(x => x.Title, f => f.Lorem.Sentence())
                .RuleFor(x => x.OwnerId, f => f.PickRandom(users).Id)
                .RuleFor(x => x.Date, f => f.Date.Past().ToUniversalTime());

            return checkFaker.Generate(amount);
        }

        private static IReadOnlyCollection<CheckItem> GenerateCheckItems(IEnumerable<Check> checks)
        {
            var checkItems = new List<CheckItem>();
            if (!checks.Any())
                return checkItems;

            int checkItemId = 1;
            foreach (var check in checks)
            {
                var checkItemFaker = new Faker<CheckItem>()
                    .RuleFor(x => x.Id, f => checkItemId++)
                    .RuleFor(x => x.Name, f => f.Commerce.ProductName())
                    .RuleFor(x => x.Description, f => f.Lorem.Sentence())
                    .RuleFor(x => x.CheckId, f => check.Id)
                    .RuleFor(x => x.Quantity, f => f.Random.Number(1, 6))
                    .RuleFor(x => x.UnitPrice, f => f.Random.Decimal(1, 100))
                    .RuleFor(x => x.TotalPrice, (f, ci) => ci.Quantity * ci.UnitPrice);

                var faker = new Faker();
                var numberOfItems = faker.Random.Number(1, 10);
                var subCheckItems = checkItemFaker.Generate(numberOfItems);

                check.Subtotal = subCheckItems.Sum(ci => ci.TotalPrice);
                check.Tax = faker.Random.Decimal(0m, 0.07m) * check.Subtotal;
                check.Tip = faker.Random.Decimal(0m, 0.30m) * check.Subtotal;
                check.Total = check.Subtotal + check.Tax + check.Tip;

                checkItems.AddRange(subCheckItems);
            }

            return checkItems;
        }
    }
}
