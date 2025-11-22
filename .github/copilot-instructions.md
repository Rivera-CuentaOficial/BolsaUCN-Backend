# BolsaFE UCN Backend - AI Agent Instructions

## Architecture Overview

**Clean Architecture + ASP.NET Core 9.0** with strict layer separation:
- **Domain** (`src/Domain/Models/`): Entity models, enums. No dependencies on other layers.
- **Infrastructure** (`src/Infrastructure/`): `AppDbContext`, Repository implementations, `DataSeeder`.
- **Application** (`src/Application/`): Services (business logic), DTOs, Mappers (Mapster), Validators.
- **API** (`src/API/`): Controllers, Middlewares. Thin layer - delegates to Services.

**Key Pattern**: Repository → Service → Controller. Services contain ALL business logic, Controllers are routing only.

## Identity & Authentication

Uses **ASP.NET Core Identity** with custom `GeneralUser : IdentityUser<int>`:
- `UserType` enum: `Estudiante`, `Empresa`, `Particular`, `Administrador`
- One-to-one discriminated inheritance: `GeneralUser` links to `Student`, `Company`, `Individual`, or `Admin`
- JWT Bearer authentication (configured in `Program.cs`)
- Role-based authorization: `Applicant` (estudiantes), `Offerent` (empresas/particulares), `Admin`, `SuperAdmin`

**Authorization Pattern**: Use `[Authorize(Roles = "Applicant,Offerent")]` on controller actions. Extract `userId` from JWT:
```csharp
var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
int.TryParse(userIdClaim, out int currentUserId)
```

## Database & Migrations

**PostgreSQL 16** via EF Core. Connection string in `appsettings.json`.

**Critical Commands** (run from `bolsafeucn_back/`):
```bash
dotnet ef migrations add MigrationName
dotnet ef database update
dotnet ef database drop --force  # Drop DB completely
```

**Makefile shortcuts** (run from project root):
```bash
make db-restart    # Drop DB, migrate, run with watch
make watch         # dotnet watch --no-hot-reload
make docker-create # Start PostgreSQL container
```

**Data Seeding**: `DataSeeder.Initialize()` runs on startup (see `Program.cs`). Creates test users with password `Test123!`:
- `estudiante@alumnos.ucn.cl` (Applicant role)
- `empresa@techcorp.cl` (Offerent role)
- `particular@ucn.cl` (Offerent role)
- `admin@ucn.cl` (Admin role)

Seed includes fake offers, job applications, and reviews using **Bogus** library.

## Mapster Configuration

**NOT AutoMapper**. Uses **Mapster** with explicit configuration classes in `src/Application/Mappers/`:
- Each entity type has a dedicated Mapper class (e.g., `StudentMapper`, `OfferMapper`)
- Call `ConfigureAllMappings()` in mapper classes
- Registered via `MapperExtensions.ConfigureMapster()` in startup

**Pattern**: Services inject mappers as dependencies or use `TypeAdapter.Adapt<TDestination>(source)` directly.

## Service Layer Patterns

**Dependency Injection**: All services registered in `Program.cs` as Scoped:
```csharp
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IPdfGeneratorService, PdfGeneratorService>();
```

**Common Service Responsibilities**:
- **Validation**: Throw `InvalidOperationException`, `KeyNotFoundException`, `UnauthorizedAccessException` with descriptive messages
- **Repository calls**: Always async (`await`)
- **Rating updates**: `GeneralUser.Rating` is auto-calculated. Access directly, don't recalculate in services.
- **Transactions**: EF Core tracks changes automatically. Call `context.SaveChanges()` in repositories.

## Review System (Bidirectional)

`Review` model is **bidirectional**: Offeror rates Student AND Student rates Offeror.
- `RatingForStudent` / `CommentForStudent` (from Offeror)
- `RatingForOfferor` / `CommentForOfferor` (from Student)
- `AtTime`, `GoodPresentation` (boolean flags for students only)
- `PublicationId` links to the offer
- Window closes 14 days after creation (`ReviewWindowEndDate`)

**Hangfire Job**: `CloseExpiredReviewsAsync()` runs hourly (configured in `Program.cs`).

## PDF Generation

Uses **QuestPDF** (Community license for educational use).
- Service: `PdfGeneratorService` implements `IPdfGeneratorService`
- Endpoint: `GET /api/review/my-reviews/pdf` (requires `[Authorize]`)
- Pattern: Fetch data → Build DTO → Generate with QuestPDF's fluent API
- Color scheme: Dynamic based on rating (5.5+ = green, <3.0 = red)

## Testing Credentials

Four pre-seeded users (all use `Test123!`):
| Email | Role | UserType |
|-------|------|----------|
| `estudiante@alumnos.ucn.cl` | Applicant | Estudiante |
| `empresa@techcorp.cl` | Offerent | Empresa |
| `particular@ucn.cl` | Offerent | Particular |
| `admin@ucn.cl` | Admin | Administrador |

**Quick Test Flow**:
1. `POST /api/auth/login` with email/password
2. Copy JWT token from response
3. Use `Authorization: Bearer {token}` in subsequent requests

## Controllers Convention

All controllers inherit from `BaseController`:
```csharp
[ApiController]
[Route("api/[controller]")]
public class BaseController : ControllerBase { }
```

**Standard Pattern**:
- Inject services via constructor
- Extract `userId` from JWT claims for auth checks
- Delegate ALL logic to services
- Return `Ok()`, `NotFound()`, `Unauthorized()`, `BadRequest()` with meaningful messages

## Publication System

`Publication` is abstract base class. Derived types:
- **Offer**: Job offers (has `Remuneration`, `Requirements`, etc.)
- **BuySell**: Marketplace items (has `Price`, `Condition`, etc.)

Both support:
- `StatusValidation` enum: `Published`, `InProcess`, `Rejected` (admin workflow)
- `IsActive` boolean
- Image attachments via `ICollection<Image>`

## Error Handling

Global middleware: `ErrorHandlingMiddleware` catches exceptions and returns structured JSON:
```json
{
  "statusCode": 500,
  "message": "Error description",
  "details": "Stack trace in dev mode"
}
```

**Service Layer**: Throw specific exceptions (`KeyNotFoundException`, `UnauthorizedAccessException`, `InvalidOperationException`). Middleware handles HTTP status codes.

## Logging

**Serilog** configured in `appsettings.json`. Logs to:
- Console (colored output)
- File: `logs/log-{Date}.txt` and `logs/log-{Date}.json`

**Usage**: Inject `ILogger<T>` or use static `Log.Information()`, `Log.Error()`, etc.

## Development Workflow

**Start Development**:
```bash
cd bolsafeucn_back
dotnet watch --no-hot-reload  # Auto-recompile on file changes
```

**Database Reset** (when models change):
```bash
make db-restart  # Drops DB, applies migrations, starts watch
```

**Swagger**: Available at `https://localhost:5001/swagger` in Development mode.

**Hangfire Dashboard**: `http://localhost:5185/hangfire` (background jobs monitoring).

## Common Patterns to Follow

1. **DTOs everywhere**: Never expose domain models directly in API responses
2. **Async/await**: All data access must be async
3. **Repository pattern**: Services call repositories, never DbContext directly
4. **JWT claims**: Always validate user identity from token, never trust route parameters for userId
5. **Rating field**: Read `GeneralUser.Rating` directly - it's maintained automatically by review system
6. **Mapster config**: Create dedicated mapper class for new entities, register in `MapperExtensions`
7. **NO emojis**: Never use emojis in code comments, documentation, commit messages, or any technical documentation

## File Naming Conventions

- DTOs: `{Entity}DTO.cs` in `src/Application/DTOs/{EntityName}DTO/`
- Services: `I{Entity}Service.cs` (interface) and `{Entity}Service.cs` (impl)
- Repositories: `I{Entity}Repository.cs` and `{Entity}Repository.cs`
- Controllers: `{Entity}Controller.cs` (no "Api" prefix)
- Models: `{Entity}.cs` in `src/Domain/Models/`
