# Progress

## Build Status
✅ Build succeeded

## What's Implemented

### Core Layer
- ✅ IUserProfileService with full CRUD operations for profile and collections
- ✅ UserProfileService implementation with update methods
- ✅ IUserProfileRepository with collection-specific methods
- ✅ **ValueObject base class** - Abstract base for DDD value object pattern
- ✅ **True Value Objects** (1:1, no identity): `Contact`, `Biometrics`
- ✅ **Nested Value Objects** (embedded in entities): `Address`, `Consent`
- ✅ **Entities** (1:N, inherit from `BaseEntity`):
  - `UserAddress` - wraps `Address` value object, adds `Type` field
  - `UserConsent` - wraps `Consent` value object
  - `EmergencyContact` - contains `Contact` value object
  - `Identification` - `Type` and `Value` fields (encrypted)
  - `Education` - Education qualifications
  - `BankAccount` - Bank account details (encrypted)
- ✅ IAggregatedRoot marker interface
- ✅ IEncryptionService interface
- ✅ AdminOwnerRequirement for authorization
- ✅ ApplicationRoles enum (Participant, Moderator, Admin)
- ✅ ValueFetchFrom enum

### Infrastructure Layer
- ✅ UserProfileRepository with EF Core implementations for all collection operations
- ✅ Supports add, remove, update, and get operations for:
  - EmergencyContacts
  - Addresses (UserAddress)
  - Identifications
  - Consents (UserConsent)
  - Educations
  - BankAccounts
- ✅ EncryptionService implementation using ASP.NET DataProtection
- ✅ UserCreatedEventConsumer

### API Layer - Security
- ✅ AdminOwnerAuthorizationHandler - Custom authorization for admin/owner checks
- ✅ AuthorizationExtensions for policy registration

### API Layer - DTOs
- ✅ ProfileResponse - Full aggregate response with all collections (includes Educations, BankAccounts)
- ✅ UpdateProfileRequest - Profile update request
- ✅ Collection requests:
  - AddAddressRequest, UpdateAddressRequest, RemoveAddressRequest (includes `Type` field)
  - AddConsentRequest, UpdateConsentRequest, RemoveConsentRequest
  - AddEmergencyContactRequest, UpdateEmergencyContactRequest, RemoveEmergencyContactRequest
  - AddIdentificationRequest, UpdateIdentificationRequest, RemoveIdentificationRequest
  - BiometricsRequest
  - AddEducationRequest, UpdateEducationRequest, RemoveEducationRequest
  - AddBankAccountRequest, UpdateBankAccountRequest, RemoveBankAccountRequest
- ✅ Collection responses (EducationResponse, BankAccountResponse)

### API Layer - Mappers
- ✅ ProfileMapper - Maps full aggregate to ProfileResponse (includes Educations, BankAccounts)
- ✅ EmergencyContactMapper - Maps to/from EmergencyContact (uses `Contact` value object)
- ✅ AddressMapper - Maps to/from UserAddress (uses `Address` value object)
- ✅ IdentificationMapper - Maps to/from Identification
- ✅ ConsentMapper - Maps to/from UserConsent (uses `Consent` value object)
- ✅ BiometricsMapper
- ✅ EducationMapper - Maps to/from Education
- ✅ BankAccountMapper - Maps to/from BankAccount

### API Layer - Validators
- ✅ UpdateProfileRequestValidator - FluentValidation for UpdateProfileRequest
  - FirstName: Required, min 3 chars
  - Gender: Required, valid enum values
  - DateOfBirth: Required, valid date format
  - Email: Optional, built-in .EmailAddress() validator

### API Layer - Endpoints
All endpoints use:
- AdminOwnerRequirement for authorization
- Mapper pattern for DTO/Entity mapping

| Endpoint | Method | Route |
|----------|--------|-------|
| GetProfileEndpoint | GET | /api/profile/{id} |
| UpdateProfileEndpoint | PUT | /api/profile/{id} |
| AddEmergencyContactEndpoint | POST | /api/profile/{id}/emergency-contacts |
| GetEmergencyContactsEndpoint | GET | /api/profile/{id}/emergency-contacts |
| UpdateEmergencyContactEndpoint | PUT | /api/profile/{id}/emergency-contacts/{contactId} |
| RemoveEmergencyContactEndpoint | DELETE | /api/profile/{id}/emergency-contacts/{contactId} |
| AddAddressEndpoint | POST | /api/profile/{id}/addresses |
| GetAddressesEndpoint | GET | /api/profile/{id}/addresses |
| UpdateAddressEndpoint | PUT | /api/profile/{id}/addresses/{addressId} |
| RemoveAddressEndpoint | DELETE | /api/profile/{id}/addresses/{addressId} |
| AddIdentificationEndpoint | POST | /api/profile/{id}/identifications |
| GetIdentificationsEndpoint | GET | /api/profile/{id}/identifications |
| UpdateIdentificationEndpoint | PUT | /api/profile/{id}/identifications/{identificationId} |
| RemoveIdentificationEndpoint | DELETE | /api/profile/{id}/identifications/{identificationId} |
| AddConsentEndpoint | POST | /api/profile/{id}/consents |
| GetConsentsEndpoint | GET | /api/profile/{id}/consents |
| UpdateConsentEndpoint | PUT | /api/profile/{id}/consents/{consentId} |
| RemoveConsentEndpoint | DELETE | /api/profile/{id}/consents/{consentId} |
| GetBiometricsEndpoint | GET | /api/profile/{id}/biometrics |
| UpdateBiometricsEndpoint | PUT | /api/profile/{id}/biometrics |
| AddEducationEndpoint | POST | /api/profile/{id}/educations |
| GetEducationsEndpoint | GET | /api/profile/{id}/educations |
| UpdateEducationEndpoint | PUT | /api/profile/{id}/educations/{educationId} |
| RemoveEducationEndpoint | DELETE | /api/profile/{id}/educations/{educationId} |
| AddBankAccountEndpoint | POST | /api/profile/{id}/bank-accounts |
| GetBankAccountsEndpoint | GET | /api/profile/{id}/bank-accounts |
| UpdateBankAccountEndpoint | PUT | /api/profile/{id}/bank-accounts/{bankAccountId} |
| RemoveBankAccountEndpoint | DELETE | /api/profile/{id}/bank-accounts/{bankAccountId} |

### Question Aggregate (Feb 24, 2026)
- ✅ **Question** - Aggregate Root with MCQ validation
- ✅ **AnswerOption** - Entity for MCQ options with points
- ✅ **Tag** - Entity for question tags with normalized name
- ✅ **MediaMetadata** - Value Object for audio/video attachments
- ✅ **QuestionType** enum - MCQ type
- ✅ **MediaType** enum - Audio/Video types
- ✅ IQuestionRepository with search and pagination
- ✅ IQuestionService with full CRUD and tag operations
- ✅ QuestionRepository implementation
- ✅ QuestionService implementation with MCQ validation

### Question API Layer
- ✅ QuestionDto - Create/Update/Response DTOs
- ✅ AnswerOptionDto - Answer option DTOs
- ✅ TagDto - Tag DTOs
- ✅ MediaMetadataDto - Media metadata DTO
- ✅ QuestionMappers - Mappers for Question entity
- ✅ QuestionRequestValidator - FluentValidation validators

### Question Endpoints
| Endpoint | Method | Route | Roles |
|----------|--------|-------|-------|
| CreateMCQQuestionEndpoint | POST | /questions/mcq | Admin, Moderator |
| GetMCQQuestionEndpoint | GET | /questions/mcq/{id} | Admin, Moderator, Presenter |
| ListQuestionsEndpoint | GET | /questions | Anonymous |
| UpdateMCQQuestionEndpoint | PUT | /questions/mcq/{id} | Admin, Moderator |
| DeleteQuestionEndpoint | DELETE | /questions/{id} | Admin, Moderator |
| AddTagEndpoint | POST | /questions/{QuestionId}/tags | Admin, Moderator |
| RemoveTagEndpoint | DELETE | /questions/{QuestionId}/tags/{TagId} | Admin, Moderator |
| SearchTagsEndpoint | GET | /tags | Anonymous |
| AddAnswerOptionEndpoint | POST | /questions/{QuestionId}/answer-options | Admin, Moderator |
| UpdateAnswerOptionEndpoint | PUT | /questions/{QuestionId}/answer-options/{OptionId} | Admin, Moderator |
| DeleteAnswerOptionEndpoint | DELETE | /questions/{QuestionId}/answer-options/{OptionId} | Admin, Moderator |
| AddMediaEndpoint | POST | /questions/{QuestionId}/media | Admin, Moderator |
| UpdateMediaEndpoint | PUT | /questions/{QuestionId}/media/{MediaId} | Admin, Moderator |
| DeleteMediaEndpoint | DELETE | /questions/{QuestionId}/media/{MediaId} | Admin, Moderator |

### Authentication
- ✅ LoginEndpoint - Returns FirstName/LastName from UserProfile
- ✅ RegisterEndpoint - Creates ApplicationUser and publishes UserCreatedEvent
- ✅ JWT authentication via FastEndpoints.Security

### Events
- ✅ UserCreatedEvent - Published on user registration
- ✅ UserCreatedEventConsumer - Creates UserProfile on user creation

### Key Patterns Used
- ✅ `[FromClaim]` attribute for JWT claim binding in FastEndpoints
- ✅ Optional route parameters with fallback to claims
- ✅ Flattened DTOs for simpler JSON payloads
- ✅ FluentValidation with built-in validators and custom rules
- ✅ **Entity wrapping Value Object pattern** - 1:N entities wrap value objects for reusable data

## What's Left
- ❌ Database migration
- ❌ Frontend integration with these endpoints
- ❌ Testing
- ✅ Question Bank domain (Feb 24, 2026)
- ❌ Session Management
- ❌ SignalR Hub implementation
- ❌ Presentation Mode

## Recent Changes Summary
| Change | Date | Description |
|--------|------|-------------|
| Question Number Field | Feb 25, 2026 | Added mandatory `Number` field (string, max 50 chars) to Question aggregate |
| Search Enhancement | Feb 25, 2026 | SearchText now searches both Text and Number fields |
| Tag M:N Refactoring | Feb 24, 2026 | Tags now reusable via QuestionTag join table |
| Delete Question Endpoint | Feb 24, 2026 | DELETE /questions/{id} for Admin/Moderator |
| ITagService/ITagRepository | Feb 24, 2026 | New services for tag management with typeahead search |
| Question Aggregate | Feb 24, 2026 | Full Question aggregate with MCQ support |
| Question Endpoints | Feb 24, 2026 | CRUD + Search endpoints for questions |
| QuestionType/MediaType Enums | Feb 24, 2026 | Enums for question classification |
| MediaMetadata Value Object | Feb 24, 2026 | Audio/Video attachment support |
| AnswerOption Entity | Feb 24, 2026 | MCQ options with points and correctness |
| Tag Entity | Feb 24, 2026 | Question tags with normalized search |
| Education CRUD Endpoints | Feb 22, 2026 | Added full CRUD endpoints for Education |
| BankAccount CRUD Endpoints | Feb 22, 2026 | Added full CRUD endpoints for BankAccount |
| ProfileResponse Update | Feb 22, 2026 | Added Educations and BankAccounts collections |
| Entity Refactoring | Feb 22, 2026 | Converted 1:N value objects to proper entities |
| Education Entity | Feb 22, 2026 | Added Education entity for qualifications |
| BankAccount Entity | Feb 22, 2026 | Added BankAccount entity for bank details |
| Value Object Restructure | Feb 22, 2026 | Created nested value objects for Address, Consent |
