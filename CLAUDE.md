# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**ProjectForge** is an ASP.NET Core 10 Web API for managing Projects, Tasks, Notes, and Incidents. It is intentionally incomplete — strategic `TODO` comments throughout mark planned extension points.

## Commands

```bash
# Start PostgreSQL (required before running the API)
docker compose up -d

# Run the API
dotnet run --project src/ProjectForge.Api

# Apply EF Core migrations
dotnet ef database update --project src/ProjectForge.Api

# Create a new migration after model changes
dotnet ef migrations add <MigrationName> --project src/ProjectForge.Api

# Build
dotnet build

# Run tests (no test project yet — this is a TODO)
dotnet test
```

**Prerequisites:** .NET 10 SDK, Docker Desktop, `dotnet-ef` global tool (`dotnet tool install --global dotnet-ef`)

## Architecture

Layered architecture with clear separation:

- **Controllers** (`Controllers/`) — Thin HTTP layer; route binding and auth attributes only
- **Application/Services/** — Business logic; implement interfaces in `Application/Interfaces/`
- **Application/DTOs/** — Request/response contracts using C# `record` types, organized by feature
- **Domain/Entities/** — EF Core domain models (`User`, `Project`, `ProjectTask`, `Note`, `Incident`)
- **Infrastructure/Data/** — `AppDbContext`, Fluent API configurations in `EntityConfigurations.cs`, migrations, and `DataSeeder`

All services are registered as `AddScoped` in `Program.cs`. `DataSeeder.SeedAsync()` auto-migrates and seeds dev data on startup.

## Key Conventions

- **Entity naming:** The task entity is `ProjectTask` (mapped to "Tasks" table) to avoid collision with `System.Threading.Tasks.Task`
- **String enums:** Status, Priority, Severity, and Role values are plain `string` properties — a TODO exists to migrate these to C# enum types
- **DTOs:** All DTOs are C# `record` types in `Application/DTOs/<Feature>/`
- **EF configuration:** All entity configurations use `IEntityTypeConfiguration<T>` in a single file: `Infrastructure/Data/Configurations/EntityConfigurations.cs`
- **Cascade deletes:** All child entities (Tasks, Notes, Incidents) cascade delete from Project

## Development Seed Data

The API auto-seeds two users on first run:

| Role | Email | Password |
|------|-------|----------|
| Admin | `admin@projectforge.local` | `Admin1234!` |
| Member | `member@projectforge.local` | `Member1234!` |

Swagger UI is available at `http://localhost:5000` in development. Authenticate via `POST /api/auth/login`, then use "Authorize" with `Bearer <token>`.

## Database

PostgreSQL 16 via Docker (`docker-compose.yml`). Connection string: `Host=localhost;Port=5432;Database=projectforge;Username=projectforge;Password=projectforge_dev`

JWT config in `appsettings.json` under `"Jwt"` — the dev key is intentionally hardcoded; change it for any real deployment.
