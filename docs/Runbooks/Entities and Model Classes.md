When creating new models, there are multiple things to consider in order to properly configure attributes. This runbook covers common scenarios that may occur while architecting entities.
#### Attribute Validations
Entity Framework Core provides two approaches for configuring entities.
##### 1. Data Annotations
Data Annotations are attributes placed directly in the model class file. Here are examples of common annotations:
```c#
public class Product
{
	[Key] // Marks this as the primary key
	public int Id { get; set; }

	[Required] // Makes the column non-nullable
	[StringLength(100, MinimumLength = 3)] // Sets string length constraints
	public string Name { get; set; }
	
	[Column("product_code", TypeName = "varchar(20)")] // Specifies column name and type
	public string Code { get; set; }
	
	[Range(0.01, 10000.00)] // Sets numeric range constraints 
	[Column(TypeName = "decimal(18,2)")]
	public decimal Price { get; set; } 
	
	[MaxLength(500)] // Sets maximum length for strings or arrays 
	public string Description { get; set; } 
	
	[Index(IsUnique = true)] // Creates a unique index 
	[EmailAddress] // Adds validation for email format 
	public string Email { get; set; } 
	
	[Timestamp] // Adds concurrency control 
	public byte[] RowVersion { get; set; }

	[DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Computed column by database on insert
	public DateTime CreatedAt { get; set; }

	[DatabaseGenerated(DatabaseGeneratedOption.Computed)] // Computed column by database on every update
	public DateTime UpdatedAt { get; set; }
	
	[NotMapped] // Excludes property from database mapping 
	public string CalculatedField { get; set; } 
	
	[Comment("Stores the product's current inventory level")] // Adds column comment 
	public int StockLevel { get; set; } 
	
	[ConcurrencyCheck] // Adds concurrency checking 
	public string Version { get; set; }
}
```
It is also possible to create custom data annotations for more complex validations. These are public classes that live in the `/Models/Validations/` directory and inherit the `ValidationAttribute` class.
##### 2. Fluent API
Fluent API configurations are defined in the DbContext `OnModelCreating` method and provide more powerful and flexible configurations:
```c#
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
	modelBuilder.Entity<Product>(entity =>
    {
        // Basic column configuration
        entity.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("product_name")
            .HasColumnType("varchar(100)")
            .HasComment("Product's display name");

        // Computed column
        entity.Property(e => e.UpdatedAt)
            .HasComputedColumnSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAddOrUpdate();

        // Check constraint
        entity.HasCheckConstraint("CK_Price_Range", "\"Price\" BETWEEN 0.01 AND 10000.00");

        // Complex unique constraint
        entity.HasIndex(e => new { e.Code, e.Name })
            .IsUnique()
            .HasDatabaseName("IX_Product_Code_Name");

        // Default value
        entity.Property(e => e.StockLevel)
            .HasDefaultValue(0);

        // Custom column collation
        entity.Property(e => e.Name)
            .UseCollation("case_insensitive");
    });
}
```
#### Entity Relationships
Foreign key relationships can be established through both data annotations and Fluent API, but the recommendation is to use Fluent API for more advanced configurations.

One-to-One Relationships:
```c#
public class User
{
    public int Id { get; set; }
    [Required]
    public string Username { get; set; }
    public UserProfile Profile { get; set; }
}

public class UserProfile
{
    public int Id { get; set; }
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
}

// Using Fluent API
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<User>()
        .HasOne(u => u.Profile)
        .WithOne(p => p.User)
        .HasForeignKey<UserProfile>(p => p.UserId)
        .OnDelete(DeleteBehavior.Cascade);  // Optional: specify delete behavior (when User is deleted, its UserProfile is also deleted)
}
```

One-to-Many Relationships:
```c#
public class Customer
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    public ICollection<Order> Orders { get; set; }
}
public class Order
{
    public int Id { get; set; }
    [Required]
    public DateTime OrderDate { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; }
}

// Using Fluent API
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
	modelBuilder.Entity<Customer>()
		.HasMany(c => c.Orders)
		.WithOne(o => o.Customer)
		.HasForeignKey(o => o.CustomerId)
		.OnDelete(DeleteBehavior.Restrict);  // Prevent cascade deletes
}
```

Many-to-Many Relationships:
```c#
public class Student 
{ 
	public int Id { get; set; } 
	public string Name { get; set; } 
	public ICollection<Course> Courses { get; set; } 
} 
public class Course 
{ 
	public int Id { get; set; } 
	public string Title { get; set; } 
	public ICollection<Student> Students { get; set; } 
}

// Using Fluent API
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
	modelBuilder.Entity<Student>()
		.HasMany(s => s.Courses)
		.WithMany(c => c.Students)
		.Map(m =>
		{
			m.ToTable("StudentCourses");
			m.MapLeftKey("StudentId");
			m.MapRightKey("CourseId");
		});
}
```

#### ORM Loading Strategies
There are three common patterns Entity Framework Core supports to load related data. For the rest of this section, suppose we have the two entities:
```c#
public class User
{
	public int Id { get; set; }
	public string Name { get; set; }
	public ICollection<Order> Orders { get; set; }
}

public class Order
{
	public int Id { get; set; }
	public int CustomerId { get; set; }
	public User Customer { get; set; }
	...
}
```

##### 1. Lazy Loading
Lazy loading loads related data only when you access the navigation property.
```c#
// First, enable lazy loading in your DbContext
public class YourDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseLazyLoadingProxies()  // Enable lazy loading
            .UseNpgsql("your_connection_string");
    }
}

// Must have `virtual` on navigation property:
public class User
{
	public int Id { get; set; }
	public string Name { get; set; }
	public virtual ICollection<Order> Orders { get; set; } // Added virtual
}

// Usage example
public async Task ShowUserOrders(int userId)
{
    var user = await _context.Users.FindAsync(userId);
    
    // Orders aren't loaded yet
    
    // This will trigger a separate database query
    foreach (var order in user.Orders)  // Lazy loading happens here
    {
        Console.WriteLine($"Order: {order.Id}");
    }
}
```
##### 2. Eager Loading 
Eager loading loads related data along with the main entity using `Include()`.
```c#
// Usage example
public async Task ShowUserOrders(int userId)
{
	// Single query that loads both User and Orders
	var user = await _context.Users
		.Include(u => u.Orders) // Eager load orders
		.FirstOrDefaultAsync(u => u.Id == userId);

	foreach (var order in user.Orders) // No additional queries
	{
		Console.WriteLine($"Order: {order.Id}");
	}
}
```
##### 3. Explicit Loading
Explicit loading allows loading related data on demand using `Load()`.
```c#
public async Task ShowUserOrders(int userId)
{
    var user = await _context.Users.FindAsync(userId);
    
    // Explicitly load orders when needed
    await _context.Entry(user)
        .Collection(u => u.Orders)
        .LoadAsync();
        
    foreach (var order in user.Orders)
    {
        Console.WriteLine($"Order: {order.Id}");
    }
}
```

##### Choosing a Loading Strategy
In general, this example encapsulate when to use each strategy:
```c#
public class UserService
{
    // Use eager loading for known related data needs
    public async Task<UserViewModel> GetUserProfile(int userId)
    {
        return await _context.Users
            .Include(u => u.Profile)
            .Include(u => u.Settings)
            .Select(u => new UserViewModel(u))
            .FirstOrDefaultAsync(u => u.Id == userId);
    }
    
    // Use lazy loading for optional/occasional access
    public async Task<bool> HasRecentOrders(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        return user.Orders.Any(o => o.CreatedAt >= DateTime.UtcNow.AddDays(-30));
    }
    
    // Use explicit loading for conditional loading
    public async Task<UserDetailsViewModel> GetUserDetails(int userId, bool includeOrders)
    {
        var user = await _context.Users.FindAsync(userId);
        
        if (includeOrders)
        {
            await _context.Entry(user)
                .Collection(u => u.Orders)
                .LoadAsync();
        }
        
        return new UserDetailsViewModel(user);
    }
}
```
