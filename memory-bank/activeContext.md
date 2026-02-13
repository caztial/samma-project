# Active Context

## Current Phase: Backend Foundation Complete ✅

The backend API project structure has been successfully created and is running in Docker Compose.

## Completed

### Backend Infrastructure
- ✅ Clean Architecture project structure (4 layers: Domain, Application, Infrastructure, API)
- ✅ .NET 10 release version configured
- ✅ SignalR Hub (SessionHub) with group management
- ✅ OpenAPI 3.1.1 with Scalar UI documentation
- ✅ Scalar server configured to localhost:5001
- ✅ Central Package Management for NuGet packages
- ✅ Docker Compose with PostgreSQL 16
- ✅ CORS configured for SignalR WebSocket connections

### Running Services
- API: http://localhost:5001
- Scalar UI: http://localhost:5001/scalar/v1
- OpenAPI JSON: http://localhost:5001/openapi/v1.json
- SignalR Hub: ws://localhost:5001/hub/session
- PostgreSQL: localhost:5432

## Project Structure
```
backend/
├── src/
│   ├── API/              # Web API + SignalR Hub + OpenAPI
│   ├── Application/      # Application layer (empty, ready for use)
│   ├── Domain/           # Domain layer (empty, ready for use)
│   └── Infrastructure/   # Infrastructure layer (empty, ready for use)
├── Directory.Packages.props  # Central Package Management
├── DhammaSession.sln
└── Dockerfile
```

## Next Steps

1. **Week 1 Continuation**
   - Implement Authentication (ASP.NET Identity with JWT)
   - Create database entities and EF Core migrations
   - Set up repository implementations

2. **Week 2-4 Implementation**
   - Question bank domain and application logic
   - Session management
   - Real-time features enhancement
   - Frontend development (React + Vite)

## Recent Changes
- Added Central Package Management for Scalar and all NuGet packages
- Configured Scalar server URL to localhost:5001
- Added PostgreSQL to Docker Compose
- Updated to .NET 10 release version

## Current Decisions

### Package Management
- Using Central Package Management (Directory.Packages.props)
- All package versions managed in single location

### OpenAPI/Scalar Configuration
- Server URL explicitly set to http://localhost:5001
- Mars theme with dark mode
- Curl as default HTTP client

### Database
- PostgreSQL 16 running in Docker
- Connection string configured for Docker network
