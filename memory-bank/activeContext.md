# Active Context

## Current Phase: Frontend Profile Overview Implementation

### Recent Work (Mar 2, 2026)
Built out the ProfileOverviewPage with full API integration, Accordion sections, and edit buttons for each section.

### What Was Done Today

#### Profile Overview Page Implementation (Mar 2, 2026)
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

## React Spectrum S2 MCP Server

A React Spectrum S2 MCP server is available for looking up component documentation, props, icons, and style values.

**Server Name**: `React Spectrum (S2)`

### Available Tools
| Tool | Purpose |
|------|---------|
| `list_s2_pages` | List all documentation pages |
| `get_s2_page_info` | Get page description and sections |
| `get_s2_page` | Get full page or section content |
| `search_s2_icons` | Search workflow icons |
| `search_s2_illustrations` | Search illustrations |
| `get_style_macro_property_values` | Get allowed style property values |

> See `memory-bank/react-spectrum-s2-mcp.md` for full usage guide.