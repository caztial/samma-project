# Active Context

## Current Phase: UserProfile API Complete ✅

The UserProfile API with authorization has been completed. All CRUD endpoints are now operational with role-based and owner-based authorization.

## Recent Changes (Feb 17, 2026)

### Commit: 8b18adb - refactor encryption service
- Refactored IEncryptionService interface
- Updated appsettings configuration
- Added Infrastructure project dependencies
- Removed appsettings.Production.json

### Commit: 8705ea3 - Add Profile endpoints with Auth
- Added Update endpoints for: Address, Consent, EmergencyContact, Identification
- New AdminOwnerAuthorizationHandler (replaced ProfileOwnerAuthorizationHandler)
- New ApplicationRoles enum (Participant, Moderator, Admin)
- New ValueFetchFrom enum for PII retrieval
- New IUserProfileService and UserProfileService
- Updated all UserProfile endpoints with proper authorization
- Added UserProfileRepository implementation

### Commit: 2f4b614 - Add REST endpoints for UserProfile
- Created all CRUD endpoints for UserProfile
- New ProfileOwnerAuthorizationHandler
- New UserProfileMappers (196 lines)
- New IUserProfileService and UserProfileService

### Commit: d51dbd2 - add UserCreatedEventHandler and PII data
- Updated UserCreatedEventConsumer
- Updated ApplicationDbContext
- New EncryptionConverter for PII
- Added event flow documentation

### Commit: bc105dc - Add UserProfile AggregatedRoot
- New IAggregatedRoot interface
- New UserProfile entity with all Value Objects
- New EncryptAttribute for PII
- New IEncryptionService with ASP.NET DataProtection
- New EncryptionConverter for EF Core

## Running Services
- API: http://localhost:8080 (Docker)
- PostgreSQL: localhost:5432

## Current Decisions

### Authorization Model
- **AdminOwnerRequirement**: Admins can access all profiles, owners can access their own
- **Roles**: Participant, Moderator, Admin
- **ValueFetchFrom**: Determines where to fetch PII (Database vs JWT)

### Encryption Architecture
- IDataProtectionProvider for AES-256 encryption
- ValueConverter for automatic encrypt/decrypt
- EncryptAttribute marks PII fields

### Endpoint Authorization
- Role-based: `Roles("Admin", "Moderator")`
- Policy-based: `Policies("ProfileOwner")`
- Owner check via AdminOwnerAuthorizationHandler

## Project Structure
```
backend/src/
├── API/
│   ├── Endpoints/UserProfile/   # 15+ endpoints
│   ├── DTOs/UserProfile/       # Request/Response DTOs
│   ├── Security/               # AdminOwnerAuthorizationHandler
│   └── Mappers/                # UserProfileMappers
├── Core/
│   ├── Entities/UserProfiles/  # UserProfile aggregate
│   ├── Entities/ValueObjects/  # Contact, Address, etc.
│   ├── Services/               # IUserProfileService, IEncryptionService
│   └── Authorization/          # AdminOwnerRequirement
└── Infrastructure/
    ├── Services/               # EncryptionService, AuthService
    └── PostgreSQL/             # UserProfileRepository
```

## Next Steps
1. Frontend integration with UserProfile endpoints
2. Testing
3. Question Bank domain
4. Session Management
