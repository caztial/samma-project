# Active Context

## Current Phase: Education & BankAccount Endpoints Complete ✅

Added full CRUD endpoints for Education and BankAccount entities, and updated ProfileResponse to include both new collections.

## Recent Changes (Feb 22, 2026)

### Education CRUD Endpoints Added
| Endpoint | Method | Route |
|----------|--------|-------|
| AddEducationEndpoint | POST | /profile/{id}/educations |
| GetEducationsEndpoint | GET | /profile/{id}/educations |
| UpdateEducationEndpoint | PUT | /profile/{id}/educations/{educationId} |
| RemoveEducationEndpoint | DELETE | /profile/{id}/educations/{educationId} |

### BankAccount CRUD Endpoints Added
| Endpoint | Method | Route |
|----------|--------|-------|
| AddBankAccountEndpoint | POST | /profile/{id}/bank-accounts |
| GetBankAccountsEndpoint | GET | /profile/{id}/bank-accounts |
| UpdateBankAccountEndpoint | PUT | /profile/{id}/bank-accounts/{bankAccountId} |
| RemoveBankAccountEndpoint | DELETE | /profile/{id}/bank-accounts/{bankAccountId} |

### ProfileResponse Updated
- Added `List<EducationResponse> Educations` collection
- Added `List<BankAccountResponse> BankAccounts` collection
- ProfileMapper updated to map both new collections

### Mappers Added
- `EducationMapper` - Maps EducationRequest/EducationResponse to/from Education entity
- `BankAccountMapper` - Maps BankAccountRequest/BankAccountResponse to/from BankAccount entity

### DTOs Created
- `EducationRequest`, `EducationResponse`
- `AddEducationRequest`, `UpdateEducationRequest`, `RemoveEducationRequest`
- `BankAccountRequest`, `BankAccountResponse`
- `AddBankAccountRequest`, `UpdateBankAccountRequest`, `RemoveBankAccountRequest`

## Entity/Value Object Architecture

```
UserProfile (Aggregate Root)
├── Contact (Value Object, 1:1)
├── Biometrics (Value Object, 1:1)
├── Addresses (1:N)
│   └── UserAddress (Entity)
│       └── Address (Value Object)
├── EmergencyContacts (1:N)
│   └── EmergencyContact (Entity)
│       └── Contact (Value Object)
├── Identifications (1:N)
│   └── Identification (Entity)
├── Consents (1:N)
│   └── UserConsent (Entity)
│       └── Consent (Value Object)
├── Educations (1:N)
│   └── Education (Entity)
└── BankAccounts (1:N)
    └── BankAccount (Entity)
```

## Running Services
- API: http://localhost:8080 (Docker)
- PostgreSQL: localhost:5432

## Current Decisions

### Entity Pattern for 1:N Relationships
- 1:N relationships are modeled as **Entities** (inherit from `BaseEntity`)
- Entities have `Id`, `CreatedAt`, `UpdatedAt` for audit trail
- Value objects inside entities are stored as owned types via EF Core `OwnsOne`

### Value Object Pattern
- True value objects (1:1, immutable, no identity) remain in `ValueObjects/`
- `Contact`, `Biometrics` are true value objects (no Id)
- Reusable value objects: `Address`, `Consent`, `Contact`

### PII Encryption
- Fields marked with `[Encrypt]` attribute are encrypted at database level
- Encrypted fields: `AccountHolderName`, `AccountNumber`, identification `Value`, etc.

## Next Steps
1. Run database migration
2. Testing
3. Frontend integration
4. Question Bank domain