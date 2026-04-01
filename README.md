# ProjectForge — Backend Foundation

A clean, intentionally incomplete .NET 10 Web API backend for managing **Projects, Tasks, Notes, and Incidents**. Built as a starting point to be extended iteratively.

---

## Tech Stack

| Layer | Technology |
|---|---|
| API | ASP.NET Core 10 Web API |
| Database | PostgreSQL 16 (via Docker) |
| ORM | Entity Framework Core 10 |
| Auth | JWT Bearer tokens |
| Docs | Swagger / OpenAPI (Swashbuckle) |
| Password hashing | BCrypt.Net-Next |

---

## Architecture

```
src/ProjectForge.Api/
├── Controllers/          # HTTP layer — thin, delegates to services
├── Application/
│   ├── DTOs/             # Request/response contracts (by feature)
│   ├── Interfaces/       # Service contracts
│   └── Services/         # Business logic implementations
├── Domain/
│   └── Entities/         # EF Core entity models
└── Infrastructure/
    └── Data/
        ├── AppDbContext.cs
        ├── Configurations/   # IEntityTypeConfiguration per entity
        ├── DataSeeder.cs     # Startup dev seeder
        └── Migrations/       # EF Core generated migrations
```

---

## Local Setup

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (for PostgreSQL)
- [EF Core CLI](https://learn.microsoft.com/en-us/ef/core/cli/dotnet): `dotnet tool install --global dotnet-ef`

---

### 1. Start PostgreSQL

```bash
docker compose up -d
```

This starts a PostgreSQL 16 container on port `5432` with:

| Setting | Value |
|---|---|
| Host | `localhost` |
| Port | `5432` |
| Database | `projectforge` |
| Username | `projectforge` |
| Password | `projectforge_dev` |

---

### 2. Apply Migrations

From the solution root:

```bash
dotnet ef database update --project src/ProjectForge.Api
```

Alternatively, migrations run automatically on API startup (via `DataSeeder.SeedAsync`).

---

### 3. Run the API

```bash
dotnet run --project src/ProjectForge.Api
```

Swagger UI is available at: **http://localhost:5000**

---

### 4. Generate a New Migration (after model changes)

```bash
dotnet ef migrations add <MigrationName> --project src/ProjectForge.Api
```

---

## Demo Login Credentials

| Role | Email | Password |
|---|---|---|
| Admin | `admin@projectforge.local` | `Admin1234!` |
| Member | `member@projectforge.local` | `Member1234!` |

To test authenticated endpoints in Swagger:
1. `POST /api/auth/login` with the credentials above
2. Copy the returned `token`
3. Click **Authorize** (top right in Swagger UI)
4. Enter `Bearer <your-token>`

---

## API Endpoints

| Method | Route | Auth | Role |
|---|---|---|---|
| `POST` | `/api/auth/login` | ❌ | — |
| `GET` | `/api/projects` | ✅ | Any |
| `GET` | `/api/projects/{id}` | ✅ | Any |
| `POST` | `/api/projects` | ✅ | Admin only |
| `GET` | `/api/projects/{id}/tasks` | ✅ | Any |
| `POST` | `/api/projects/{id}/tasks` | ✅ | Any |
| `GET` | `/api/projects/{id}/notes` | ✅ | Any |
| `POST` | `/api/projects/{id}/notes` | ✅ | Any |
| `GET` | `/api/projects/{id}/incidents` | ✅ | Any |
| `POST` | `/api/projects/{id}/incidents` | ✅ | Any |

---

## Configuration

Configuration is in `src/ProjectForge.Api/appsettings.json`.

| Key | Description |
|---|---|
| `ConnectionStrings:DefaultConnection` | PostgreSQL connection string |
| `Jwt:Key` | HMAC signing key (change for production) |
| `Jwt:Issuer` | JWT issuer |
| `Jwt:Audience` | JWT audience |
| `Jwt:ExpiresInHours` | Token lifetime in hours (default: 8) |

> **Note:** For production, move secrets to environment variables or a secrets manager. Never commit real credentials.

---

## Intentionally Left Incomplete

This project is a **foundation**, not a finished product. The following are deliberately absent:

- Full CRUD for all entities (no `PUT`, `DELETE`, `PATCH` endpoints)
- Refresh tokens and token revocation
- User registration endpoint
- Password reset / forgot password flow
- Email verification or MFA
- ACL / fine-grained permissions model
- Audit logging / change history
- Search, filtering, sorting (beyond minimal ordering)
- Pagination
- Background jobs / queued tasks
- Notifications system
- AI or agent integration
- Full automated test suite (unit + integration)
- Production hardening (rate limiting, CORS policy, health checks, metrics)
- Soft delete pattern
- User-project ownership and task assignment

Strategic `// TODO:` comments are placed throughout the codebase at every extension point.

---

## Suggested Next Steps (with Claude Code)

The following are natural extensions in priority order:

1. **Add remaining CRUD endpoints** — `PUT`, `DELETE`, `PATCH /status` for each resource
2. **Implement pagination** — Add `page` / `pageSize` to list endpoints
3. **Validate project existence** in task/note/incident creation (return 404)
4. **Add user ownership** — Link projects and tasks to the creating user
5. **Task assignment** — Assign tasks to users; add `AssigneeId` to Task
6. **POST /api/auth/register** — Implement user self-registration
7. **Refresh tokens** — Long-lived tokens + `/api/auth/refresh`
8. **Replace string enums** with proper C# `enum` types
9. **Add integration tests** using EF Core in-memory or Testcontainers
10. **Add global exception handler middleware** for consistent error responses
11. **CORS policy** for a future frontend client
12. **Health check endpoint** (`/health`)
13. **Soft delete** with `IsDeleted` filters on entities
14. **Audit fields** — `UpdatedAt`, `UpdatedBy`

---

## Running Tests

No tests yet — this is a planned extension point.

```bash
# Future:
dotnet test
```

---
