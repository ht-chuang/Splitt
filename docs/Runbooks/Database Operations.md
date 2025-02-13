#### Setting up
Install `PostgreSQL` with `brew` in your console:
```
brew install postgresql
```

Ensure you have postgresql installed with:
```
postgres --version
```

Start the service:
```
brew services start postgresql
```

To set up a PostgreSQL database locally,

Log in as the default user `postgres` in the console:
```
psql postgres
```

Then create the database with a username and password:
```sql
CREATE DATABASE splitt;
CREATE USER <username> WITH PASSWORD <password>;
GRANT ALL PRIVILEGES ON DATABASE splitt TO <username>;
```

Next, form your connection string with the following line below. Remember to replace your username and password. 
```
Host=localhost;Database=splitt;Username=<username>;Password=<password>;
```

Add this connection string to your `user-secrets`. If not already initialized, run in the console:
```
dotnet init user-secrets --project SplittDB
```

Then once you have `user-secrets` initialized, set the default connection string:
```
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=splitt;Username=<username>;Password=<password>;" --project SplittDB
```

Install EntityFrameworkCore as a global tool if haven't already:
```
dotnet tool install --global dotnet-ef
```

Then in your terminal, run:
```
dotnet ef database update --project SplittLib --startup-project SplittDB
```
This should populate your database with tables.

OPTIONAL: Install a UI client for the PostgreSQL database
Personally, I use pgAdmin, which can be installed [here](https://www.pgadmin.org/download/)
#### Migrations
##### Basic Commands
Since the DbContext is in `SplittLib` but migrations need to be run from `SplittDB`, we need to specify the startup project.

To generate a new migration:
```
dotnet ef migrations add MigrationName --project SplittLib --startup-project SplittDB
```

Applying migrations to the database:
```
dotnet ef database update --project SplittLib --startup-project SplittDB
```

Remove the latest migration (does not apply to the database):
```
dotnet ef migrations remove --project SplittLib --startup-project SplittDB
```

Reverting migrations in the database:
```bash
# Applying migrations
dotnet ef migrations add FirstMigration
dotnet ef migrations add MistakeMigration
dotnet ef database update # applies all migrations

# Reverting last migration (MistakeMigration)
dotnet ef database update FirstMigration

# To revert all the way back to an empty database
dotnet ef database update 0
```

Generating an SQL script for all migrations:
```
dotnet ef migrations script --project SplittLib --startup-project SplittDB
```
##### Adding New Tables
1. Create the model class in `SplittLib`
```c#
public class Customer
{
	public int Id { get; set; }
	public string Name { get; set; }
	public DateTime CreatedAt { get; set; }
}
```
2. Add `DbSet` to the `DbContext`
```c#
public DbSet<Customer> Customers { get; set; }
```
3. Generate the migration
```
dotnet ef migrations add AddCustomerTable --project SplittLib --startup-project SplittDB
```

More information on creating entities can be found in [[Entities and Model Classes]].
#### Seed Data