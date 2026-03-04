# Active Context

## Current Phase: Frontend Profile UI Enhancements

### Recent Work (Mar 3, 2026 - Night)
Implemented Add/Edit Identification dialog with form validation and API integration. This completes all dialogs for the multi-item sections in Profile Overview.

#### Identification Dialog Implementation (Mar 3, 2026 - Night)
1. **ProfileOverviewPage.jsx** - Add/Edit Identification functionality
   - Added identification dialog state: `identificationDialogOpen`, `identificationData`, `identificationDialogMode`, `isSavingIdentification`
   - Implemented validation for required fields (ID Type, ID Number/Value)
   - `handleAddClick('identifications')` - Opens dialog in 'add' mode
   - `handleEditClick('identifications', id)` - Opens dialog in 'edit' mode with pre-populated data
   - `handleSaveIdentification()` - Submits to API with correct payload structure

2. **API Payload Structure**:
   ```javascript
   // Payload structure for identifications
   {
     type: "string (required)",
     value: "string (required)"
   }
   ```

3. **i18n Updates** (en-US.json, si-LK.json):
   - `profile.overview.identificationDialog.addTitle` / `editTitle`
   - `profile.overview.identificationDialog.typePlaceholder` / `valuePlaceholder`
   - `profile.overview.identificationDialog.add` / `update` / `cancel`
   - `profile.overview.identificationDialog.addSuccess` / `updateSuccess` / `error`

4. **Key Implementation Details**:
   - Direct payload (no wrapping object like addresses/emergency contacts)
   - Dialog uses S2 Form, TextField components
   - Cancel and Add/Update buttons with proper disabled state during validation errors
   - Toast notifications for success/error feedback
   - Local state updated optimistically after successful API call
   - Build verified successful

### What Was Done Today

#### Emergency Contact Dialog Implementation (Mar 3, 2026 - Late Evening)
1. **ProfileOverviewPage.jsx** - Add/Edit Emergency Contact functionality
   - Added emergency contact dialog state: `emergencyContactDialogOpen`, `emergencyContactData`, `emergencyContactDialogMode`, `isSavingEmergencyContact`
   - Implemented validation for required fields (Name, Contact Number)
   - Email validation using regex if provided
   - `handleAddClick('emergencyContacts')` - Opens dialog in 'add' mode
   - `handleEditClick('emergencyContacts', id)` - Opens dialog in 'edit' mode with pre-populated data
   - `handleSaveEmergencyContact()` - Submits to API with correct payload structure

2. **API Payload Structure** (FIXED - was causing 400 Bad Request):
   ```javascript
   // Correct payload structure - wraps contact data in 'emergencyContact' object
   {
     emergencyContact: {
       name: "string (required)",
       relationship: "string | null",
       contactNumber: "string (required)",
       email: "string | null"
     }
   }
   ```

3. **i18n Updates** (en-US.json):
   - `profile.overview.emergencyContactDialog.addTitle` / `editTitle`
   - `profile.overview.emergencyContactDialog.namePlaceholder` / `relationshipPlaceholder` / `phonePlaceholder` / `emailPlaceholder`
   - `profile.overview.emergencyContactDialog.add` / `update` / `cancel`
   - `profile.overview.emergencyContactDialog.addSuccess` / `updateSuccess` / `error`

4. **Key Implementation Details**:
   - Payload wraps contact data in `emergencyContact` object (same pattern as addresses)
   - `null` sent for optional blank fields (not empty strings)
   - Dialog uses S2 Form, TextField components
   - Cancel and Add/Update buttons with proper disabled state during validation errors
   - Toast notifications for success/error feedback
   - Local state updated optimistically after successful API call

#### Address Dialog Implementation (Mar 3, 2026 - Earlier)
1. **ProfileOverviewPage.jsx** - Add/Edit Address functionality
   - Added address dialog state: `addressDialogOpen`, `addressData`, `addressDialogMode`, `isSavingAddress`
   - Created `addressTypeOptions` for Picker (Home, Work, Other)
   - Implemented validation for required fields (Type, Line 1, Country)
   - `handleAddClick('addresses')` - Opens dialog in 'add' mode
   - `handleEditClick('addresses', id)` - Opens dialog in 'edit' mode with pre-populated data
   - `handleSaveAddress()` - Submits to API with correct payload structure

2. **API Payload Structure**:
   ```javascript
   {
     type: "Home" | "Work" | "Other",
     address: {
       line1: "string (required)",
       line2: "string (empty if blank)",
       suburb: "string (empty if blank)",
       stateProvince: "string (empty if blank)",
       country: "string (required)",
       postcode: "string (empty if blank)"
     }
   }
   ```

3. **i18n Updates** (en-US.json):
   - `profile.overview.addressDialog.addTitle` / `editTitle`
   - `profile.overview.addressDialog.selectType`
   - `profile.overview.addressDialog.typeOptions.home` / `work` / `other`
   - `profile.overview.addressDialog.line1Placeholder` / `line2Placeholder` / `countryPlaceholder`
   - `profile.overview.addressDialog.add` / `update` / `cancel`
   - `profile.overview.addressDialog.addSuccess` / `updateSuccess` / `error`

4. **Key Implementation Details**:
   - Empty strings (`""`) sent instead of `null` for optional blank fields
   - Dialog uses S2 Form, Picker, TextField components
   - Cancel and Add/Update buttons with proper disabled state during validation errors
   - Toast notifications for success/error feedback
   - Local state updated optimistically after successful API call

#### Profile Overview UI Enhancement (Mar 3, 2026 - Evening)
1. **ProfileOverviewPage.jsx** - UI improvements for multi-item sections
   - Replaced Edit button with Add (+) button in multi-item section headers
   - Added Edit and Delete action buttons to each item within multi-item sections
   - Added Divider component between items for visual separation
   - Updated all sections to use `inlineFieldGroupStyle` for consistent label-value display
   - New style: `listItemActionsStyle` for action button alignment

2. **Multi-item vs Single-item Sections**:
   | Section Type | Header Action | Item Actions |
   |--------------|---------------|--------------|
   | Personal Info | Edit | - |
   | Contact Info | Edit | - |
   | Addresses | Add (+) | Edit, Delete per item |
   | Emergency Contacts | Add (+) | Edit, Delete per item |
   | Education | Add (+) | Edit, Delete per item |
   | Bank Accounts | Add (+) | Edit, Delete per item |
   | Identifications | Add (+) | Edit, Delete per item |
   | Consents | Add (+) | Edit, Delete per item |

3. **i18n Updates** (en-US.json):
   - Added `profile.overview.addSection`: "Add new {section}"
   - Added `profile.overview.editItem`: "Edit"
   - Added `profile.overview.deleteItem`: "Delete"

4. **New Imports**:
   - `Divider` from @react-spectrum/s2
   - `Add` icon from @react-spectrum/s2/icons
   - `Delete` icon from @react-spectrum/s2/icons

5. **S2 Style Macro Note**: 
   - ✅ Use `justifyContent: 'end'` (not `'flex-end'`)

#### Shared API Client Module (Mar 3, 2026 - Earlier)
1. **apiClient.js** (`frontend/src/services/apiClient.js`) - NEW
   - `createApiClient({ getToken, onUnauthorized })` - Creates axios instance with:
     - Request interceptor: Injects Bearer token via `getToken()`
     - Response interceptor: Handles 401 responses via `onUnauthorized()`
   - `createPublicApiClient()` - For public endpoints (login, register)
   - Single source of truth for API configuration

2. **authService.js** (`frontend/src/services/authService.js`)
   - Refactored to use `createPublicApiClient()` from apiClient
   - No auth token injection (public endpoints)

3. **profileService.js** (`frontend/src/services/profileService.js`)
   - Refactored to use `createApiClient()` from apiClient
   - New signature: `createProfileService({ getToken, onUnauthorized })`
   - Automatic auth token injection and 401 handling

4. **AuthContext.jsx** (`frontend/src/contexts/AuthContext.jsx`)
   - Added `getToken()` - Returns current token for API client
   - Added `onUnauthorized()` - Handles 401 by logging out and redirecting to /login

5. **App.jsx** (`frontend/src/App.jsx`)
   - Fixed: `AuthProvider` moved inside `BrowserRouter` for `useNavigate` to work
   - Added `AppWithProviders` component to maintain proper provider nesting

#### Architecture Benefits
- **Single source of truth** for API configuration
- **Automatic 401 handling** - Users are logged out and redirected when token expires
- **Consistent auth token injection** across all services
- **Easy to extend** for future services (sessionService, questionService, etc.)

#### Previous Work - Profile Overview Page (Mar 2, 2026)
1. **Profile Service** (`frontend/src/services/profileService.js`)
   - Factory function `createProfileService(getToken)` for authenticated API calls
   - Auto-injects Bearer token via axios interceptor
   - Full CRUD methods for all profile collections:
     - `getProfile()`, `getProfileById()`, `updateProfile()`
     - Addresses, Education, Bank Accounts, Emergency Contacts
     - Identifications, Consents, Biometrics

2. **ProfileOverviewPage Component** (`frontend/src/pages/profile/ProfileOverviewPage.jsx`)
   - Fetches profile data from `GET /api/profile` on mount
   - Loading state with ProgressCircle
   - Error state with IllustratedMessage
   - Uses S2 Accordion with `allowsMultipleExpanded` for collapsible sections
   - Each section has Edit button (placeholder for future implementation)

3. **Accordion Sections Implemented**:
   | Section | Fields Displayed |
   |---------|------------------|
   | Personal Information | FirstName, LastName, Gender, DateOfBirth |
   | Contact Information | Email, ContactNumber |
   | Addresses | Type, Line1, Line2, Suburb, StateProvince, Country, Postcode |
   | Emergency Contacts | Name, Relationship, ContactNumber, Email |
   | Education | Institution, Degree, FieldOfStudy, StartDate, EndDate, Grade, Verified badge |
   | Bank Accounts | BankName, AccountType, AccountHolderName, AccountNumber, BranchCode, Verified badge |
   | Identifications | Type, Value |
   | Consents | TermsVersion, AcceptedAt, IPAddress |

4. **API Payload Field Mappings Fixed**:
   - Emergency Contacts: `contactNumber` (not `phoneNumber`), added `email`
   - Identifications: `value` (not `number`), removed non-existent `expiryDate`, `isVerified`
   - Consents: `termsVersion`, `acceptedAt`, `ipAddress` (not `type`, `isGranted`, `grantedAt`)

5. **i18n Updates**:
   - Added new translation keys: `termsVersion`, `acceptedAt`, `ipAddress`
   - Both en-US and si-LK locales updated

#### Previous Profile Work (Feb 28, 2026)
1. **ProfileNavigation Component** - Collapsible navigation with React Router integration
2. **ProfileLayout Component** - Mobile navigation drawer wrapper
3. **Separate Profile Pages** - Each section has own route
4. **Theme Support** - Light/dark mode via S2 Provider

### Current Frontend Structure
```
frontend/
├── src/
│   ├── components/
│   │   ├── ProtectedRoute.jsx
│   │   └── profile/
│   │       └── ProfileNavigation.jsx
│   ├── layouts/
│   │   ├── MainLayout.jsx       (top bar, settings menu)
│   │   ├── AuthLayout.jsx       (auth pages layout)
│   │   └── ProfileLayout.jsx    (profile nav drawer)
│   ├── pages/
│   │   ├── auth/
│   │   │   ├── AuthLayout.jsx
│   │   │   ├── LoginPage.jsx
│   │   │   └── SignupPage.jsx
│   │   ├── profile/
│   │   │   ├── MyProfilePage.jsx      (redirect only)
│   │   │   ├── ProfileOverviewPage.jsx
│   │   │   ├── EducationPage.jsx
│   │   │   ├── BankAccountsPage.jsx
│   │   │   └── SessionsPage.jsx
│   │   └── admin/
│   │       └── AdminPortalPage.jsx
│   ├── contexts/
│   │   ├── AuthContext.jsx
│   │   └── ThemeContext.jsx
│   ├── i18n/
│   │   ├── LocaleContext.jsx
│   │   ├── useTranslation.js
│   │   └── locales/
│   │       ├── en-US.json
│   │       └── si-LK.json
│   ├── services/
│   │   ├── authService.js
│   │   └── profileService.js
│   ├── config.js
│   ├── App.jsx
│   └── main.jsx
```

### Key Decisions
- **Mobile-first approach** - Navigation as a slide-out drawer (no desktop sidebar)
- **Separate pages per section** - Each profile section has its own route and page
- **ProfileLayout pattern** - Reusable layout wrapper for all profile pages
- **S2 style macro** - All styles defined as static module-level constants

### Environment Variables
| Variable | Purpose | Default |
|----------|---------|---------|
| `VITE_API_URL` | Full backend API URL | `http://localhost:5001/api` |
| `VITE_ENV` | Environment label | `development` |

### Running Services
- Frontend Dev Server: http://localhost:5173
- Backend API: http://localhost:5001 (or Docker port 8080)

### Next Steps
1. ~~Build out ProfileOverviewPage with actual user profile form~~ ✅ Done (Mar 2, 2026)
2. Implement edit functionality for each profile section
3. Build out EducationPage with education management (add/edit/delete)
4. Build out BankAccountsPage with bank account management (add/edit/delete)
5. Build out SessionsPage with session history
6. Add SignalR integration for real-time updates

