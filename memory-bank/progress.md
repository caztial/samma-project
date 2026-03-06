# Progress

## Build Status
✅ Build succeeded (with 5 nullable warnings)

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

### Session Management (Feb 25, 2026)

#### Core Layer - Session Entities
- ✅ **Session** - Aggregate Root for session management
  - Properties: Name, Code, Location, State, StartedAt, EndedAt
  - State machine: Draft → Active ↔ Inactive → Ended
  - Methods: Activate(), Deactivate(), Reactivate(), End(), CanJoin()
- ✅ **SessionParticipant** - Entity for session participants
  - Properties: SessionId, UserId, JoinedAt, LeftAt
  - Methods: Leave(), IsActiveInSession()
- ✅ **SessionQuestion** - Entity for assigned questions
  - Properties: Order, IsPresented, ShowTitle, ShowOptionValues, MaxAttempts
  - Methods: Present(), Deactivate(), UpdateOrder(), IsActive()
- ✅ **QuestionAttempt** - Entity for question attempts
  - Properties: AttemptNumber, IsActive, ScoreMultiplier
  - Methods: Activate(), Deactivate(), IsAcceptingAnswers(), GetScoreMultiplier()
- ✅ **ParticipantAnswer** - Abstract base for answers
  - Properties: IsCorrect, BasePoints, FinalScore, AnswerType
- ✅ **ParticipantMCQAnswer** - MCQ answer implementation
  - Properties: SelectedOptionId (Guid)

#### Core Layer - Session Enums
- ✅ **SessionState** enum: Draft, Active, Inactive, Ended

#### Core Layer - Session Events
- ✅ SessionCreatedEvent
- ✅ SessionActivatedEvent
- ✅ SessionEndedEvent
- ✅ ParticipantJoinedEvent
- ✅ ParticipantLeftEvent
- ✅ QuestionAssignedToSessionEvent
- ✅ QuestionPresentedEvent
- ✅ QuestionAttemptActivatedEvent
- ✅ QuestionDeactivatedEvent
- ✅ AnswerSubmittedEvent
- ✅ **SubmitAnswerCommand** (Feb 27, 2026) - Async command for answer submission
- ✅ **AnswerSubmissionResultEvent** (Feb 27, 2026) - Result event for SignalR notification

#### Core Layer - Session Interfaces
- ✅ ISessionRepository - Full repository interface
- ✅ ISessionService - Full service interface with:
  - Session Management: Create, Update, Delete, Activate, Deactivate, Reactivate, End
  - Participant Management: Join, Leave, GetParticipants
  - Question Assignment: Assign, Remove, Reorder
  - Question Presentation: Present, Deactivate, ActivateAttempt
  - Answer Submission: SubmitMCQAnswer
  - Scoring: GetSessionScores, GetQuestionScores, GetParticipantTotalScore

#### Infrastructure Layer - Session
- ✅ SessionRepository implementation with EF Core
- ✅ SessionService implementation with business logic
- ✅ MassTransit event publishing

#### API Layer - Session DTOs
- ✅ CreateSessionRequest, UpdateSessionRequest
- ✅ JoinSessionRequest, AssignQuestionRequest
- ✅ PresentQuestionRequest, ActivateAttemptRequest
- ✅ SubmitMCQAnswerRequest (legacy - SelectedOptionId as Guid)
- ✅ **SubmitMCQAnswerRequestBody** (Feb 27, 2026) - Only SelectedOptionId in body
- ✅ **SubmitAnswerAcceptedResponse** (Feb 27, 2026) - 202 Accepted response
- ✅ SessionResponse, SessionParticipantResponse
- ✅ SessionQuestionResponse, PresentedQuestionResponse
- ✅ ActiveAttemptResponse, AvailableAttemptResponse
- ✅ SubmitAnswerResponse
- ✅ SessionScoresResponse, QuestionScoresResponse
- ✅ SessionListResponse with pagination

#### API Layer - Session Mappers
- ✅ SessionMapper
- ✅ SessionParticipantMapper
- ✅ SessionQuestionMapper
- ✅ PresentedQuestionMapper
- ✅ SubmitAnswerMapper

#### API Layer - Session Endpoints
| Endpoint | Method | Route | Description |
|----------|--------|-------|-------------|
| CreateSessionEndpoint | POST | /sessions | Create new session |
| GetSessionEndpoint | GET | /sessions/{id} | Get session by ID |
| GetSessionByCodeEndpoint | GET | /sessions/code/{code} | Get session by code |
| ListSessionsEndpoint | GET | /sessions | List sessions with pagination |
| UpdateSessionEndpoint | PUT | /sessions/{id} | Update session |
| DeleteSessionEndpoint | DELETE | /sessions/{id} | Delete session |
| ActivateSessionEndpoint | POST | /sessions/{id}/activate | Activate session |
| DeactivateSessionEndpoint | POST | /sessions/{id}/deactivate | Pause session |
| ReactivateSessionEndpoint | POST | /sessions/{id}/reactivate | Resume session |
| EndSessionEndpoint | POST | /sessions/{id}/end | End session |
| JoinSessionEndpoint | POST | /sessions/{id}/join | Join session |
| LeaveSessionEndpoint | POST | /sessions/{id}/leave | Leave session |
| GetParticipantsEndpoint | GET | /sessions/{id}/participants | Get participants |
| AssignQuestionEndpoint | POST | /sessions/{id}/questions | Assign question |
| RemoveQuestionEndpoint | DELETE | /sessions/{id}/questions/{questionId} | Remove question |
| ReorderQuestionsEndpoint | PUT | /sessions/{id}/questions/reorder | Reorder questions |
| PresentQuestionEndpoint | POST | /sessions/{id}/present | Present question |
| DeactivateQuestionEndpoint | POST | /sessions/{id}/questions/{questionId}/deactivate | Deactivate question |
| ActivateAttemptEndpoint | POST | /sessions/{id}/attempts/activate | Activate attempt |
| SubmitMCQAnswerEndpoint | POST | /sessions/{id}/questions/{questionId}/attempts/{attemptNumber}/answers | Submit answer (async) |
| GetSessionScoresEndpoint | GET | /sessions/{id}/scores | Get session scores |
| GetQuestionScoresEndpoint | GET | /sessions/{id}/questions/{questionId}/scores | Get question scores |

#### API Layer - SignalR Hub
- ✅ SessionHub - SignalR hub for real-time updates
  - JoinSessionGroupAsync - Join session group
  - LeaveSessionGroupAsync - Leave session group
  - JoinAdminGroupAsync - Join admin group for session
  - **AnswerResult** (Feb 27, 2026) - Receive answer submission results

#### API Layer - Event Consumers
- ✅ ParticipantJoinedEventConsumer - SSE notification to admin
- ✅ ParticipantLeftEventConsumer - SSE notification to admin
- ✅ QuestionPresentedEventConsumer - SSE notification to participants
- ✅ QuestionAttemptActivatedEventConsumer - SSE notification to participants
- ✅ QuestionDeactivatedEventConsumer - SSE notification to participants
- ✅ SessionEndedEventConsumer - SSE notification to all
- ✅ AnswerSubmittedEventConsumer - Audit logging
- ✅ **SubmitAnswerCommandConsumer** (Feb 27, 2026) - Background answer processing with idempotency

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

### Question Aggregate (Feb 24, 2026)
- ✅ **Question** - Aggregate Root with MCQ validation
- ✅ **McqQuestion** - MCQ question type with answer options
- ✅ **McqAnswerOption** - Entity for MCQ options with points (Id is Guid)
  - **OptionNumber** (Mar 3, 2026) - Added string property for option identifier (e.g., "A", "B", "C")
- ✅ **Tag** - Entity for question tags with normalized name
- ✅ **MediaMetadata** - Value Object for audio/video attachments
- ✅ **QuestionType** enum - MCQ type
- ✅ **MediaType** enum - Audio/Video types
- ✅ IQuestionRepository with search and pagination
- ✅ IQuestionService with full CRUD and tag operations
- ✅ QuestionRepository implementation
- ✅ QuestionService implementation with MCQ validation

### Authentication
- ✅ LoginEndpoint - Returns FirstName/LastName from UserProfile
- ✅ LoginEndpoint - Returns user roles (List<string>) via UserManager.GetRolesAsync (Feb 28, 2026)
- ✅ LoginResponse - Added `Roles` property for frontend role-based UI (Feb 28, 2026)
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
- ✅ **Domain Events** - MassTransit for event publishing
- ✅ **SignalR for real-time** - SSE notifications via hub context
- ✅ **Async Command Pattern** (Feb 27, 2026) - Queue-based processing for high-frequency operations
- ✅ **Idempotency Pattern** (Feb 27, 2026) - Command deduplication using CommandId

## Frontend (Feb 27, 2026 - Rebuild + Auth Pages)

### Project Setup
- ✅ Vite + React 18 project (JSX, not TypeScript)
- ✅ React Spectrum S2 design system
- ✅ Vite configuration with macros plugin for S2 styling
- ✅ Locale optimization for en-US and si-LK

### Dependencies Installed
- ✅ `@react-spectrum/s2` - UI components
- ✅ `unplugin-parcel-macros` - S2 style macros
- ✅ `@react-aria/optimize-locales-plugin` - Locale optimization
- ✅ `react-router-dom` - Routing
- ✅ `axios` - HTTP client

### i18n System
- ✅ `LocaleContext.jsx` - React Context for language state
- ✅ `useTranslation.js` - Hook for accessing translations
- ✅ `en-US.json` - Full English translations (login, signup, auth, theme, language, common)
- ✅ `si-LK.json` - Full Sinhala translations (all keys)

### Auth Pages (Feb 27, 2026)
- ✅ `ThemeContext.jsx` - Dark/light mode state, persisted to localStorage, respects OS preference
- ✅ `authService.js` - Login + register API calls; baseURL from `config.js`
- ✅ `AuthLayout.jsx` - Shared layout with top bar (S2 Picker for language, S2 Switch for dark mode)
- ✅ `LoginPage.jsx` - Email/password form with validation, server error InlineAlert, translations
- ✅ `SignupPage.jsx` - First/Last name, email, password with validation and translations
- ✅ `App.jsx` - ThemeProvider → LocaleProvider → BrowserRouter → AppRoutes → S2 Provider → Routes
- ✅ Routes: `/login`, `/signup`, `/` → redirect to `/login`

### Authentication & Role-Based Routing (Feb 28, 2026)
- ✅ `AuthContext.jsx` - Authentication context with JWT token, user info (email, firstName, lastName, profileId, roles)
- ✅ `AuthProvider` - Wraps app, provides `login()`, `logout()`, `isAuthenticated`, `isAdmin`, `isModerator`, `hasAnyRole()`
- ✅ `ProtectedRoute.jsx` - Route wrapper that checks authentication and optional role requirements
- ✅ `MainLayout.jsx` - Layout for authenticated pages with language toggle, dark mode toggle, logout button
- ✅ `MyProfilePage.jsx` - Placeholder page for user profile (all authenticated users)
- ✅ `AdminPortalPage.jsx` - Placeholder page for admin portal (Admin/Moderator only)
- ✅ Role-based redirect after login: Admin/Moderator → `/admin`, others → `/profile`
- ✅ Updated routes with protection:
  - `/login` → LoginPage (public)
  - `/signup` → SignupPage (public)
  - `/profile` → MyProfilePage (protected, all authenticated)
  - `/admin` → AdminPortalPage (protected, Admin/Moderator only)
  - `/` → Smart redirect based on auth status and role
- ✅ Translations for navigation, profile, and admin pages (en-US, si-LK)

### Mobile-First Responsive Menu (Feb 28, 2026)
- ✅ `MainLayout.jsx` - Responsive header with mobile hamburger menu
- ✅ Mobile menu (`< 640px`): MenuTrigger + Menu with settings section and logout
- ✅ Desktop controls (`>= 640px`): Inline Picker, Switch, Button
- ✅ S2 style macro responsive breakpoints: `sm: { display: 'none' }` / `sm: { display: 'flex' }`
- ✅ Icons used: `Settings`, `Translate`, `Lightbulb`, `Leave`
- ✅ Translation keys added: `navigation.menu`, `navigation.settings`, `navigation.language`
- ✅ Build verified with `npm run build`

### Profile Pages with Navigation (Feb 28, 2026)
- ✅ `ProfileNavigation.jsx` - Collapsible navigation component with React Router integration
- ✅ `ProfileLayout.jsx` - Layout wrapper with mobile navigation drawer
- ✅ `ProfileOverviewPage.jsx` - Profile overview page at `/profile/overview`
- ✅ `EducationPage.jsx` - Education management page at `/profile/education`
- ✅ `BankAccountsPage.jsx` - Bank accounts page at `/profile/bank-accounts`
- ✅ `SessionsPage.jsx` - Session history page at `/profile/sessions`
- ✅ `MyProfilePage.jsx` - Redirects to `/profile/overview`
- ✅ Profile routes added to App.jsx with ProtectedRoute wrapper
- ✅ Navigation icons: `User`, `Education`, `Wallet`, `Calendar`, `ChevronRight`, `MenuHamburger`, `Close`
- ✅ Translation keys for profile navigation and content sections
- ✅ Light/dark mode support via S2 Provider colorScheme

### Profile Overview Implementation (Mar 2, 2026)
- ✅ `profileService.js` - Factory-based service with Bearer token authentication via axios interceptor
- ✅ ProfileOverviewPage with full API integration (`GET /api/profile`)
- ✅ S2 Accordion component with 8 collapsible sections
- ✅ Edit button on each section (placeholder for future)
- ✅ Loading state (ProgressCircle) and error state (IllustratedMessage)
- ✅ Field mappings corrected for API payload:
  - Emergency Contacts: `contactNumber`, `email`
  - Identifications: `type`, `value`
  - Consents: `termsVersion`, `acceptedAt`, `ipAddress`
- ✅ Badge components for item counts and verified status
- ✅ S2 style macro with valid tokens (`fontSize: 'detail-sm'`, `color: 'neutral-subdued'`)
- ✅ i18n keys added: `termsVersion`, `acceptedAt`, `ipAddress` (en-US, si-LK)
- ✅ Build verified successful

### Global API Configuration (Feb 27, 2026)
- ✅ `src/config.js` - Exports `API_BASE_URL` from `import.meta.env.VITE_API_URL` (fallback: `http://localhost:5001/api`)
- ✅ `authService.js` - Uses `API_BASE_URL` from `config.js` (no more hardcoded URL)
- ✅ `vite.config.js` - Uses `loadEnv()` to derive proxy target from `VITE_API_URL`
- ✅ `.env.development` - `VITE_API_URL=http://localhost:5001/api` (with clear comments)
- ✅ `.env.production` - `VITE_API_URL=https://api.yourdomain.com/api` (placeholder)
- ✅ `.env.example` - Documents all variables with usage examples

### Shared API Client Module (Mar 3, 2026)
- ✅ `apiClient.js` - NEW shared axios instance factory
  - `createApiClient({ getToken, onUnauthorized })` - Authenticated API client
  - `createPublicApiClient()` - Public API client (no auth)
  - Request interceptor: Auto-injects Bearer token
  - Response interceptor: Handles 401 globally, calls `onUnauthorized()`
- ✅ `authService.js` - Refactored to use `createPublicApiClient()`
- ✅ `profileService.js` - Refactored to use `createApiClient()`
  - New signature: `createProfileService({ getToken, onUnauthorized })`
- ✅ `AuthContext.jsx` - Added `getToken()` and `onUnauthorized()` callbacks
- ✅ `App.jsx` - Fixed provider order (AuthProvider inside BrowserRouter)
  - Added `AppWithProviders` component for proper nesting
- ✅ Build verified successful

### Profile Overview UI Enhancement (Mar 3, 2026 - Evening)
- ✅ Multi-item section actions - Add (+) button in header, Edit/Delete per item
- ✅ Single-item sections (Personal, Contact) - Edit button in header only
- ✅ Divider component between items for visual separation
- ✅ Consistent inline field layout using `inlineFieldGroupStyle`
- ✅ New style: `listItemActionsStyle` for action button alignment
- ✅ i18n keys: `addSection`, `editItem`, `deleteItem`
- ✅ New imports: `Divider`, `Add`, `Delete` icons

### Address Dialog Implementation (Mar 3, 2026 - Earlier)
- ✅ Add/Edit Address dialog with Form, Picker, TextField components
- ✅ Address type options: Home, Work, Other
- ✅ Validation for required fields (Type, Line 1, Country)
- ✅ API integration with correct payload structure:
  - `type`: string (Home/Work/Other)
  - `address`: nested object with line1, line2, suburb, stateProvince, country, postcode
- ✅ Empty strings sent for optional blank fields (not null)
- ✅ Dialog state management: `addressDialogOpen`, `addressData`, `addressDialogMode`, `isSavingAddress`
- ✅ Optimistic local state update after successful API call
- ✅ Toast notifications for success/error feedback
- ✅ i18n keys for dialog: `addTitle`, `editTitle`, `selectType`, `typeOptions.*`, placeholders, buttons, success messages

### Emergency Contact Dialog Implementation (Mar 3, 2026 - Late Evening)
- ✅ Add/Edit Emergency Contact dialog with Form, TextField components
- ✅ Validation for required fields (Name, Contact Number)
- ✅ Email validation using regex if provided
- ✅ API integration with correct payload structure:
  - `emergencyContact`: nested object with name, relationship, contactNumber, email
- ✅ `null` sent for optional blank fields (not empty strings)
- ✅ Dialog state management: `emergencyContactDialogOpen`, `emergencyContactData`, `emergencyContactDialogMode`, `isSavingEmergencyContact`
- ✅ Optimistic local state update after successful API call
- ✅ Toast notifications for success/error feedback
- ✅ i18n keys for dialog: `addTitle`, `editTitle`, placeholders, buttons, success messages
- ✅ Fixed 400 Bad Request error by wrapping payload in `emergencyContact` object

### Identification Dialog Implementation (Mar 3, 2026 - Night)
- ✅ Add/Edit Identification dialog with Form, TextField components
- ✅ Validation for required fields (ID Type, ID Number/Value)
- ✅ API integration with direct payload structure:
  - `type`: string (required)
  - `value`: string (required)
- ✅ Dialog state management: `identificationDialogOpen`, `identificationData`, `identificationDialogMode`, `isSavingIdentification`
- ✅ Optimistic local state update after successful API call
- ✅ Toast notifications for success/error feedback
- ✅ i18n keys for dialog: `addTitle`, `editTitle`, placeholders, buttons, success messages
- ✅ Build verified successful
- ✅ **All multi-item section dialogs now complete** (Addresses, Emergency Contacts, Identifications)

### Key Decisions
- React Context for state management (not Zustand)
- JSX (not TypeScript)
- Mobile-first for client pages, tablet/laptop for admin pages
- i18n from day one - English and Sinhala supported
- S2 style macro: `'end'` not `'flex-end'`; `paddingLeft`/`paddingRight` not `paddingStart`/`paddingEnd`
- API URL configured once in `.env.*` — drives both axios `baseURL` and Vite dev proxy target
- **Per-attempt state tracking** — `submittedAnswers[qId][attemptNumber]` allows multiple attempts per question
- **Composite timeout keys** — `"questionId:attemptNumber"` ensures fresh timer for each attempt

## What's Left
- ❌ Database migration
- ✅ Frontend: AuthContext (store JWT, user info, logged-in state) - Done Feb 28, 2026
- ✅ Frontend: Protected route wrapper (redirect to /login if not authenticated) - Done Feb 28, 2026
- ✅ Frontend: MyProfile/AdminPortal placeholder pages - Done Feb 28, 2026
- ✅ Frontend: Profile Overview page with API integration - Done Mar 2, 2026
- ✅ Frontend: Profile Address Add/Edit functionality - Done Mar 3, 2026
- ❌ Frontend: Profile edit functionality (Personal Info, Contact Info, other sections)
- ❌ Frontend: Education page with add/edit/delete
- ❌ Frontend: Bank Accounts page with add/edit/delete
- ❌ Frontend: Build out admin portal page with admin features
- ✅ Frontend: SignalR integration (Mar 6, 2026)
- ❌ Testing
- ❌ Session timer/auto-deactivation
- ❌ Distributed cache for idempotency (currently in-memory)

## Recent Changes Summary
| Change | Date | Description |
|--------|------|-------------|
| SignalR Integration | Mar 6, 2026 | Installed `@microsoft/signalr@10.0.0`; created `signalrService.js` with JWT auth and auto-reconnect; integrated into ActiveSessionPage for real-time session updates |
| McqAnswerOption.OptionNumber | Mar 3, 2026 | Added `OptionNumber` property (string, max 10 chars) for option identifier (e.g., "A", "B", "C"); updated entity, service, DTOs, mappers, endpoints, DbContext |
| Per-Attempt Answer Tracking | Mar 3, 2026 | ActiveSessionPage refactored: timer runs continuously, `submittedAnswers[qId][attemptNumber]` per-attempt tracking, composite timeout keys |
| Question Service + ActiveSessionPage | Mar 3, 2026 | New `questionService.js`; full MCQ question card, countdown timer, Meter, navigation, submit |
| Shared API Client Module | Mar 3, 2026 | Created `apiClient.js` with global 401 handling, refactored all services to use it |
| Profile Overview Page | Mar 2, 2026 | Full API integration, Accordion sections, Edit buttons, Field mappings fixed |
| Profile Service | Mar 2, 2026 | Factory-based service with Bearer token via axios interceptor |
| Mobile Responsive Menu | Feb 28, 2026 | MainLayout hamburger menu for mobile (<640px) with S2 style macro breakpoints |
| Frontend Workflow Rules | Feb 28, 2026 | Added memory bank rules: no `npm run dev`, use inline styles via S2 macro, check MCP for options |
| LoginResponse Roles | Feb 28, 2026 | Added `Roles` property to LoginResponse; LoginEndpoint returns user roles |
| Async Answer Submission | Feb 27, 2026 | Refactored SubmitMCQAnswerEndpoint for queue-based processing |
| SubmitAnswerCommand | Feb 27, 2026 | New command for async answer submission with CommandId for idempotency |
| SubmitAnswerCommandConsumer | Feb 27, 2026 | Background consumer with idempotency check and SignalR result notification |
| Route Refactoring | Feb 27, 2026 | Changed answer endpoint to use route params for sessionId, questionId, attemptNumber |
| 202 Accepted Response | Feb 27, 2026 | Endpoint returns immediately with CommandId, result sent via SignalR |
| Session Management | Feb 25, 2026 | Full session aggregate with participants, questions, attempts, answers |
| Session Events | Feb 25, 2026 | Domain events for session lifecycle |
| SignalR Hub | Feb 25, 2026 | Real-time updates via SessionHub |
| Event Consumers | Feb 25, 2026 | MassTransit consumers for SSE notifications |
| McqAnswerOption.Id | Feb 25, 2026 | Changed from int to Guid for consistency |
| Question Number Field | Feb 25, 2026 | Added mandatory `Number` field (string, max 50 chars) to Question aggregate |