# Active Context

## Current Phase: Frontend Profile Pages Development

### Recent Work (Feb 28, 2026)
Created a complete profile section with mobile navigation, separate pages for each profile section, and a reusable ProfileLayout component.

### What Was Done Today
1. **ProfileNavigation Component** (`frontend/src/components/profile/ProfileNavigation.jsx`)
   - Collapsible navigation with expandable Profile section
   - Uses React Router (`useNavigate`, `useLocation`) for navigation
   - Disclosure component for expandable sections
   - Nav items: Profile Overview, Education, Bank Accounts, Sessions

2. **ProfileLayout Component** (`frontend/src/layouts/ProfileLayout.jsx`)
   - Wraps MainLayout with mobile navigation drawer
   - Hamburger menu button (fixed position, top-left)
   - Slide-out drawer with ProfileNavigation
   - All profile pages use this layout

3. **Separate Profile Pages** (each with own route):
   - `ProfileOverviewPage.jsx` - `/profile/overview`
   - `EducationPage.jsx` - `/profile/education`
   - `BankAccountsPage.jsx` - `/profile/bank-accounts`
   - `SessionsPage.jsx` - `/profile/sessions`

4. **Route Structure**:
   - `/profile` → Redirects to `/profile/overview`
   - `/profile/overview` → ProfileOverviewPage
   - `/profile/education` → EducationPage
   - `/profile/bank-accounts` → BankAccountsPage
   - `/profile/sessions` → SessionsPage

5. **MainLayout Changes**:
   - Replaced MenuHamburger icon with Settings (cogwheel) icon for mobile settings menu
   - Settings menu contains language toggle, dark mode, and logout

6. **Theme Support**:
   - All pages respect light/dark mode through S2 Provider's `colorScheme` prop
   - Semantic color tokens (e.g., `backgroundColor: 'base'`) auto-adapt to theme

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
│   │   └── authService.js
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
1. Build out ProfileOverviewPage with actual user profile form
2. Build out EducationPage with education management
3. Build out BankAccountsPage with bank account management
4. Build out SessionsPage with session history
5. Add SignalR integration for real-time updates

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