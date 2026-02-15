# Active Context

## Current Phase: UserProfile DDD Implementation Complete ✅

The UserProfile Aggregated Root with PII encryption has been successfully implemented, moving user profile data to a separate domain entity with automatic encryption.

## Completed

### UserProfile Aggregate Root Implementation
- ✅ IAggregatedRoot marker interface in Core
- ✅ Gender enum (Male, Female, Other, PreferNotToSay)
- ✅ IEncryptionService interface in Core
- ✅ EncryptionService implementation using ASP.NET Core DataProtection (AES)
- ✅ EF Core ValueConverter for automatic encrypt/decrypt on read/write
- ✅ UserProfile aggregate root with:
  - FirstName, LastName (encrypted PII)
  - Gender, DateOfBirth
  - Value Objects: Consent, Contact, EmergencyContact, Address, Identification, Biometrics
- ✅ IUserProfileRepository interface
- ✅ IUserProfileService interface and implementation
- ✅ UserCreatedEventConsumer creates UserProfile on registration

### Login Endpoint Enhancement
- ✅ LoginEndpoint now fetches UserProfile to return FirstName/LastName

### Configuration
- ✅ appsettings.Development.json created with:
  - ConnectionStrings
  - DataProtection encryption key
  - AdminUser credentials (encrypted password)
- ✅ launch.json configured for VSCode debugging
- ✅ tasks.json build task created

### SeedData Enhancement
- ✅ SeedData reads AdminUser from configuration
- ✅ Decrypts password using IEncryptionService before creating admin user

## Running Services
- API: http://localhost:8080 (Docker)
- PostgreSQL: localhost:5432

## Project Structure (Updated)
```
backend/
├── src/
│   ├── API/              # Web API + SignalR Hub + OpenAPI
│   │   ├── Endpoints/
│   │   │   └── Auth/
│   │   │       ├── LoginEndpoint.cs
│   │   │       └── RegisterEndpoint.cs
│   │   ├── DTOs/
│   │   └── Hubs/
│   ├── Core/             # Domain entities and business logic
│   │   ├── Entities/
│   │   │   ├── UserProfiles/
│   │   │   │   └── UserProfile.cs
│   │   │   ├── ValueObjects/
│   │   │   │   ├── Consent.cs
│   │   │   │   ├── Contact.cs
│   │   │   │   ├── EmergencyContact.cs
│   │   │   │   ├── Address.cs
│   │   │   │   ├── Identification.cs
│   │   │   │   └── Biometrics.cs
│   │   │   ├── IAggregatedRoot.cs
│   │   │   ├── EncryptAttribute.cs
│   │   │   └── BaseEntity.cs
│   │   ├── Enums/
│   │   │   └── Gender.cs
│   │   ├── Services/
│   │   │   ├── IAuthService.cs
│   │   │   ├── IEncryptionService.cs
│   │   │   ├── IUserProfileService.cs
│   │   │   └── JwtOptions.cs
│   │   └── Repositories/
│   │       ├── IUserRepository.cs
│   │       └── IUserProfileRepository.cs
│   └── Infrastructure/   # Data access, external services
│       ├── Services/
│       │   ├── AuthService.cs
│       │   └── EncryptionService.cs
│       ├── Data/
│       │   ├── ApplicationDbContext.cs
│       │   ├── SeedData.cs
│       │   └── Encryption/
│       │       └── EncryptionConverter.cs
│       └── PostgreSQL/
│           └── Repositories/
│               ├── UserRepository.cs
│               └── UserProfileRepository.cs
└── .vscode/
    ├── launch.json
    └── tasks.json
```

## Current Decisions

### Encryption Architecture
- **IDataProtectionProvider** for AES-256 encryption
- ValueConverter for automatic encrypt/decrypt in EF Core
- EncryptAttribute for marking PII fields
- Separate encryption key in appsettings.Development.json

### Value Object Relationships
| Value Object | Relationship | PII |
|--------------|-------------|-----|
| Consent | 1:N | No |
| EmergencyContact | 1:N | Yes (encrypted) |
| Contact | 1:1 | Yes (encrypted) |
| Address | 1:N | No |
| Identification | 1:N | Yes (encrypted) |
| Biometrics | 1:1 | Yes (Base64, encrypted) |

### Admin Password Management
- Encrypted password stored in appsettings.Development.json
- Decrypted at runtime using IEncryptionService
- Allows secure configuration without exposing passwords in source

## VSCode Debugging
- Launch configuration in .vscode/launch.json
- Build task in .vscode/tasks.json
- Press F5 to debug the API locally

## Next Steps

1. **Complete UserProfile Creation for Admin**
   - Trigger UserCreatedEvent for admin user during seeding
   - Or create UserProfile directly in SeedData

2. **Question Bank Domain**
   - Question entity with answer options
   - QuestionRepository

3. **Session Management**
   - Session entity with lifecycle states
   - SessionRepository

4. **Frontend Development**
   - React + Vite setup
   - Authentication flow
   - Dashboard pages
