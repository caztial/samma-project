# Active Context

## Current Phase: UserProfile API Enhanced ✅

The UserProfile API has been enhanced with improved claim-based authorization and validation.

## Recent Changes (Feb 18, 2026)

### Enhancement: GetProfileEndpoint & UpdateProfileEndpoint
- Added `[FromClaim("sub")]` attribute to request DTOs for automatic JWT claim binding
- Made route ID optional - falls back to deriving ProfileId from UserId in claims
- Route ID takes precedence over claims when both are provided
- Updated validation documentation in FASTENDPOINTS_GUIDE.md

### Enhancement: UpdateProfileRequest Validation
- Flattened ContactNumber and Email to top-level fields
- Added UpdateProfileRequestValidator with FluentValidation
- Uses built-in `.EmailAddress()` validator
- Added validation examples for string-to-enum and date parsing

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

## Running Services
- API: http://localhost:8080 (Docker)
- PostgreSQL: localhost:5432

## Current Decisions

### Authorization Model
- **AdminOwnerRequirement**: Admins can access all profiles, owners can access their own
- **Roles**: Participant, Moderator, Admin
- **ValueFetchFrom**: Determines where to fetch PII (Database vs JWT)
- **[FromClaim]**: FastEndpoints attribute for automatic JWT claim binding

### Endpoint Pattern
- Use `[FromClaim("sub")]` on request DTO properties to bind UserId from JWT
- Route ID takes precedence: route → claims → error
- Flattened DTOs for simpler JSON payloads

### Validation
- FastEndpoints validators inherit from `Validator<TRequest>`
- Use built-in FluentValidation rules where possible (e.g., `.EmailAddress()`)
- Custom validators for string-to-enum and date parsing

## Project Structure
```
backend/src/
├── API/
│   ├── Endpoints/UserProfile/   # 15+ endpoints
│   ├── DTOs/UserProfile/       # Request/Response DTOs
│   ├── Validators/              # FluentValidation validators
│   ├── Security/                # AdminOwnerAuthorizationHandler
│   └── Mappers/                 # UserProfileMappers
├── Core/
│   ├── Entities/UserProfiles/   # UserProfile aggregate
│   ├── Entities/ValueObjects/  # Contact, Address, etc.
│   ├── Services/               # IUserProfileService, IEncryptionService
│   └── Authorization/          # AdminOwnerRequirement
└── Infrastructure/
    ├── Services/                # EncryptionService, AuthService
    └── PostgreSQL/             # UserProfileRepository
```

## Next Steps
1. Frontend integration with UserProfile endpoints
2. Testing
3. Question Bank domain
4. Session Management
