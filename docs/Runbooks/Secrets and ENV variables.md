#### User Secrets
User Secrets are designed for development purposes, allowing devs to store sensitive data outside the project tree. This prevents it from being checked into source control.

##### Adding User Secrets
To initialize user secrets in the project, run:
```
dotnet user-secrets init
```
This adds a `UserSecretsId` element to your `.csproj` file, linking the project to a unique secret storage.

To add a secret, run:
```
dotnet user-secrets set "YourSecretKey" "YourSecretValue"
```
For hierarchical keys, use a colon `:` for sections:
```
dotnet user-secrets set "Section:Subsection:Key" "Value"
```

##### Modifying User Secrets
To modify an existing secret, set it again with a new value:
```
dotnet user-secrets set "YourSecretKey" "NewSecretValue"
```

##### Removing User Secrets
Removing user secrets is simply running:
```
dotnet user-secrets remove "YourSecretKey"
```

##### Showing User Secrets
To list all secrets associated with the project:
```
dotnet user-secrets list
```

##### Using User Secrets in Code
Ensure that a reference to the `Microsoft.Extensions.Configuration.UserSecrets` package is added if not already included:
```
dotnet add package Microsoft.Extensions.Configuration.UserSecrets
```
Then load user secrets in your application's configuration:
```c#
var builder = new ConfigurationBuilder()
	.AddUserSecrets<Program>();

IConfiguration configuration = builder.Build();
```

*\* Note that ASP.NET Core does this automatically when in the "Development" environment*

To use it in code:
```c#
string secretValue = builder.configuration["YourSecretKey"];
```

#### Environment Variables
Environment variables can be integrated with the `.NET` configuration system using the `appsettings.json` file.

##### Setting up `appsettings.json`
An example file could look like:
```json
{
	"YourApp": {
		"YourSetting": "DefaultValue"
	}
}
```

Then you can load the environment variables in your application's configuration like so:
```c#
var builder = new ConfigurationBuilder()
	.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
	.AddEnvironmentVariables(); 
	
IConfiguration configuration = builder.Build();
```

Accessing is simply:
```c#
string settingValue = configuration["YourApp:YourSetting"];
```

##### Environment-specific `appsettings.json`
For environments like `development`, `staging`, or `production`, we can specify the environment on `appsettings.json` by renaming it:
```
appsettings.{Environment}.json

e.g.
appsettings.Development.json
appsettings.Staging.json
appsettings.Production.json
```

To set the environment, go to Properties > `launchsettings.json` and change the `ASPNETCORE_ENVIRONMENT` variable.