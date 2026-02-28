# Technical Context

## Technology Stack

### Backend
| Component | Technology | Version |
|-----------|-----------|---------|
| Framework | .NET | 10.0 |
| Web API | ASP.NET Core | 10.0 |
| OpenAPI | Microsoft.AspNetCore.OpenApi | 10.0.0-preview.1.25120.3 |
| OpenAPI Models | Microsoft.OpenApi | 2.0.0-preview5 |
| Documentation | Scalar.AspNetCore | 2.0.18 |
| Authentication | ASP.NET Identity | - |
| JWT | FastEndpoints.Security | 7.2.0 |
| Real-time | SignalR | - |
| ORM | Entity Framework Core | - |
| Database | PostgreSQL | 16 |
| Validation | FluentValidation | - |
| Mediator | MediatR | - |
| Messaging | MassTransit | 9.0.1 |

### Frontend
| Component | Technology | Version |
|-----------|-----------|---------|
| Framework | React | 18 |
| Build Tool | Vite | 7+ |
| Language | JavaScript (JSX) | - |
| UI Components | @react-spectrum/s2 | Latest |
| State Management | React Context | - |
| HTTP Client | Axios | - |
| i18n | Custom (LocaleContext) | - |
| Build Plugin | unplugin-parcel-macros | - |
| Locale Optimization | @react-aria/optimize-locales-plugin | - |

### Supported Languages
- English (en-US)
- Sinhala (si-LK)

## Frontend Development Rules

### Development Workflow
- **Do NOT run `npm run dev`** - Ask the user to start the dev server instead
- Verify builds using `npm run build` with full logs to catch and fix errors

### Styling Approach
- **Always use inline styles via the S2 `style` macro** (not CSS files)
- All style values must be **static** - no dynamic values allowed in style macro
- Define styles at module level as `const` for S2 style macro compatibility

### S2 Style Macro Quirks
- Use `'end'` not `'flex-end'`
- Use `paddingLeft`/`paddingRight` not `paddingStart`/`paddingEnd`
- Responsive breakpoints: `sm: { display: 'flex' }` for screens ≥640px

### Component Development
- **Always check component options and props using the React Spectrum S2 MCP server** before implementing
- Use semantic color tokens (e.g., `backgroundColor: 'base'`) for automatic light/dark mode support

### Mobile-First Approach
- Design for mobile screens first, then adapt for desktop
- Use S2 responsive breakpoints (`sm:`) for larger screens

### Design System Documentation
- **React Spectrum S2 MCP Server** - Available for looking up component documentation
- **Server**: `React Spectrum (S2)` 
- **Guide**: See `memory-bank/react-spectrum-s2-mcp.md`

### Infrastructure
| Component | Technology |
|-----------|-----------|
| Container | Docker |
| Orchestration | Docker Compose |
| Web Server | Nginx |
| Database | PostgreSQL |

## Architecture Patterns

### Backend Architecture: Clean Architecture (3 layers)
```
├── API Layer (FastEndpoints, SignalR Hubs, DTOs, OpenAPI)
├── Core Layer (Domain + Application: Entities, Services interfaces, Repository interfaces)
└── Infrastructure Layer (Service implementations, Data Access, External Services)
```

### Key Patterns
- **CQRS**: Separate read/write operations
- **Repository Pattern**: Data access abstraction
- **Service Pattern**: IAuthService interface in Core, implementation in Infrastructure
- **Unit of Work**: Transaction management
- **Domain Events**: Loose coupling via MassTransit
- **SignalR Groups**: Session-based communication
- **Central Package Management**: Single location for package versions

### Frontend Architecture
```
├── apps/
│   ├── participant/     # Mobile-optimized
│   ├── admin/          # Tablet-optimized
│   └── presenter/      # Projector display
├── shared/
│   ├── components/
│   ├── hooks/
│   ├── services/
│   └── types/
```

## Development Environment

### Prerequisites
- .NET 10 SDK
- Node.js 20+
- Docker & Docker Compose
- PostgreSQL (for local development)

### Local Development
```bash
# Backend (Docker)
docker-compose up --build

# Or local development
cd backend/src/API
dotnet run
```

### Docker Deployment
```bash
docker-compose up --build
```

## Environment Variables

### Backend (.env)
```
ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__DefaultConnection=Host=postgres;Database=dhamma_db;Username=dhamma_user;Password=dhamma_pass
Jwt__Secret=your-secret-key
```

### Frontend (.env)
```
VITE_API_URL=http://localhost:5001
VITE_SIGNALR_URL=http://localhost:5001/hub
```

## Package Management

### Central Package Management
Using `Directory.Packages.props` in backend root:
```xml
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>
  <ItemGroup>
    <PackageVersion Include="Microsoft.AspNetCore.OpenApi" Version="10.0.0-preview.1.25120.3" />
    <PackageVersion Include="Microsoft.OpenApi" Version="2.0.0-preview5" />
    <PackageVersion Include="Scalar.AspNetCore" Version="2.0.18" />
    <PackageVersion Include="FastEndpoints" Version="7.2.0" />
    <PackageVersion Include="FastEndpoints.Security" Version="7.2.0" />
  </ItemGroup>
</Project>
```

### Infrastructure Project Dependencies
The Infrastructure project requires `FastEndpoints.Security` for JWT token generation:
```xml
<PackageReference Include="FastEndpoints.Security" />
```

## OpenAPI/Scalar Configuration

### Server Configuration
```csharp
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Servers = new List<OpenApiServer>
        {
            new OpenApiServer { Url = "http://localhost:5001" }
        };
        return Task.CompletedTask;
    });
});
```

### Scalar UI Configuration
```csharp
app.MapScalarApiReference(options =>
{
    options.Title = "Dhamma Session API";
    options.Theme = ScalarTheme.Mars;
    options.DefaultHttpClient = new(ScalarTarget.Shell, ScalarClient.Curl);
    options.ShowSidebar = true;
    options.DarkMode = true;
});
```

## Security Considerations
- JWT tokens for authentication via FastEndpoints.Security
- Role-based authorization
- Input validation
- SQL injection protection (EF Core)
- XSS protection (React escapes by default)
- CORS configured for SignalR WebSocket connections

## Performance Considerations
- SignalR in-memory (single instance for LAN)
- No Redis backplane needed
- Direct database access (no caching layer for MVP)
- Optimized for local network speed
- Central Package Management for consistent dependencies
