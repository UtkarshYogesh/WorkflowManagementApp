# Database Provider and Migration Guide

This API supports two database providers:

- Local development: SQLite
- Production: Azure SQL / SQL Server

Entity Framework Core migrations are separated by provider because SQLite and SQL Server generate different database schemas and migration SQL.

## Project Structure

The shared application model lives in:

```text
Data/AppDbContext.cs
```

Provider-specific contexts live in:

```text
Data/SqliteAppDbContext.cs
Data/SqlServerAppDbContext.cs
```

Provider-specific migrations live in:

```text
Migrations/Sqlite
Migrations/SqlServer
```

## Why There Are Two DbContexts

`AppDbContext` contains the actual model:

- DbSets
- relationships
- indexes
- seed data
- delete behavior rules

`SqliteAppDbContext` and `SqlServerAppDbContext` inherit from `AppDbContext`.

This lets the application use `AppDbContext` everywhere, while EF Core can maintain separate migration histories:

```text
SqliteAppDbContext    -> Migrations/Sqlite
SqlServerAppDbContext -> Migrations/SqlServer
```

Using only `--output-dir` with one context is not enough, because EF migrations are tied to a `DbContext` type, not only to a folder.

## Runtime Provider Selection

The provider is selected in `Program.cs` using configuration:

```csharp
var provider = builder.Configuration["DatabaseProvider"] ?? "Sqlite";
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
```

If `DatabaseProvider` is `SqlServer`, the app uses SQL Server:

```csharp
options.UseSqlServer(connectionString)
```

Otherwise, the app uses SQLite:

```csharp
options.UseSqlite(connectionString)
```

Services and controllers can still depend on `AppDbContext`. Dependency injection provides the correct provider-specific context at runtime.

## Configuration

Local/default configuration:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=taskmanager.db"
  },
  "DatabaseProvider": "Sqlite"
}
```

Production configuration:

```json
{
  "DatabaseProvider": "SqlServer"
}
```

In Azure App Service, configure these application settings:

```text
DatabaseProvider=SqlServer
ConnectionStrings__DefaultConnection=<Azure SQL connection string>
```

Do not commit production database passwords or Azure SQL connection strings into `appsettings.json`.

## Design-Time DbContext Factories

The file below contains EF Core design-time factories:

```text
Data/DesignTimeDbContextFactory.cs
```

These factories are used by commands such as:

```powershell
dotnet ef migrations add ...
dotnet ef database update ...
```

They tell EF Core how to create each provider-specific context without needing to fully start the API.

This keeps migration commands stable and ensures:

- SQLite migrations use the SQLite provider.
- SQL Server migrations use the SQL Server provider.
- EF commands do not depend on API startup behavior.
- Connection strings can be supplied through environment variables.

## Add a SQLite Migration

Use this command for local SQLite migrations:

```powershell
dotnet ef migrations add <MigrationName> --context SqliteAppDbContext --output-dir Migrations\Sqlite
```

Example:

```powershell
dotnet ef migrations add AddProjectDueDate --context SqliteAppDbContext --output-dir Migrations\Sqlite
```

Apply SQLite migrations locally:

```powershell
dotnet ef database update --context SqliteAppDbContext
```

## Add a SQL Server Migration

Use this command for production SQL Server migrations:

```powershell
dotnet ef migrations add <MigrationName> --context SqlServerAppDbContext --output-dir Migrations\SqlServer
```

Example:

```powershell
dotnet ef migrations add AddProjectDueDate --context SqlServerAppDbContext --output-dir Migrations\SqlServer
```

Apply SQL Server migrations:

```powershell
dotnet ef database update --context SqlServerAppDbContext
```

For production, prefer generating an idempotent SQL script and applying it through a controlled deployment step:

```powershell
dotnet ef migrations script --context SqlServerAppDbContext --idempotent -o sqlserver-migration.sql
```

## Existing SQLite Database Warning

The existing `taskmanager.db` file may have been created using the old root `Migrations` history.

The new SQLite migration folder contains a fresh initial migration. Running it against an existing database that already has tables may fail because EF will try to create tables that already exist.

For local development, choose one of these approaches:

1. Fresh local database:

   Delete or rename `taskmanager.db`, then run:

   ```powershell
   dotnet ef database update --context SqliteAppDbContext
   ```

2. Preserve existing local data:

   Do not run the new initial migration directly against the existing DB. First create a baseline strategy or manually align the `__EFMigrationsHistory` table.

## Deployment Strategy

Current target deployment:

```text
Frontend Vue app -> Netlify
Backend .NET API -> Azure App Service
Local DB         -> SQLite
Production DB    -> Azure SQL
```

Azure App Service should run with:

```text
ASPNETCORE_ENVIRONMENT=Production
DatabaseProvider=SqlServer
ConnectionStrings__DefaultConnection=<Azure SQL connection string>
```

Local development should run with:

```text
DatabaseProvider=Sqlite
ConnectionStrings__DefaultConnection=Data Source=taskmanager.db
```
