# Active Context

## Current Phase: Frontend Rebuild

### Recent Work
The frontend has been rebuilt from scratch with a focus on internationalization (i18n) support.

### What Was Done
1. **Deleted old frontend** - Removed existing React/Polaris implementation
2. **Created fresh Vite + React project** - Using JSX (not TypeScript)
3. **Installed dependencies:**
   - `@react-spectrum/s2` - React Spectrum S2 design system
   - `unplugin-parcel-macros` - Required for S2 styling
   - `@react-aria/optimize-locales-plugin` - Locale optimization
   - `react-router-dom` - Routing
   - `axios` - HTTP client

4. **Configured Vite for S2:**
   - Macros plugin (must be first)
   - Locale optimization for `en-US` and `si-LK`
   - CSS bundling for S2 styles
   - API proxy to backend

5. **Created i18n system:**
   - `LocaleContext.jsx` - React Context for language state
   - `useTranslation.js` - Hook for accessing translations
   - `en-US.json` - English translations
   - `si-LK.json` - Sinhala translations

### Current Frontend Structure
```
frontend/
├── src/
│   ├── config.js                 ← API_BASE_URL from VITE_API_URL; central config
│   ├── i18n/
│   │   ├── LocaleContext.jsx
│   │   ├── useTranslation.js
│   │   └── locales/
│   │       ├── en-US.json        (login, signup, auth, theme, language, common keys)
│   │       └── si-LK.json        (full Sinhala translations)
│   ├── contexts/
│   │   └── ThemeContext.jsx      (dark/light mode - persisted to localStorage)
│   ├── pages/
│   │   └── auth/
│   │       ├── AuthLayout.jsx    (top bar: language picker + dark mode switch)
│   │       ├── LoginPage.jsx     (email, password, validation, server errors)
│   │       └── SignupPage.jsx    (firstName, lastName, email, password)
│   ├── services/
│   │   └── authService.js        (login + register; baseURL from config.js)
│   ├── App.jsx                   (ThemeProvider → LocaleProvider → BrowserRouter → S2 Provider → Routes)
│   ├── main.jsx
│   ├── index.css                 (minimal reset, 100vh body)
│   └── App.css                   (empty - layout handled by S2 style macro)
├── .env.development              (VITE_API_URL=http://localhost:5001/api)
├── .env.production               (VITE_API_URL=https://api.yourdomain.com/api)
├── .env.example                  (documents all env variables)
└── vite.config.js                (uses loadEnv to derive proxy target from VITE_API_URL)
```

### Key Decisions
- **React Context** for state management (not Zustand)
- **React Spectrum S2** for UI components
- **JSX** (not TypeScript)
- **Mobile-first** for client pages, tablet/laptop for admin pages
- **i18n from day one** - English and Sinhala supported
- **ThemeContext** wraps outside LocaleProvider so colorScheme is available to S2 Provider
- **S2 style macro quirks**: use `'end'` not `'flex-end'`; use `paddingLeft`/`paddingRight` not `paddingStart`/`paddingEnd`
- **AuthLayout pattern** - shared layout component for top bar + centered content area
- **Global API config**: `VITE_API_URL` in `.env.*` files controls both the axios `baseURL` (via `src/config.js`) AND the Vite dev proxy target (via `loadEnv` in `vite.config.js`)
- **Dark mode backgrounds**: Use `backgroundColor: 'base'` on root containers for automatic light/dark adaptation

### Environment Variables
```
VITE_API_URL=http://localhost:5001/api
VITE_ENV=development
```

### Running Services
- Frontend Dev Server: http://localhost:5173
- Backend API: http://localhost:5001 (or Docker port 8080)

### Recent Backend Changes (Feb 28, 2026)
- **LoginResponse now includes user roles** - Added `Roles` property (List<string>) to return user's assigned roles (Admin, Moderator, Presenter, Participant) on login
- **LoginEndpoint** - Now injects `UserManager<ApplicationUser>` to fetch roles via `_userManager.GetRolesAsync(user)`

### Recent Frontend Changes (Feb 28, 2026)
- **AuthContext** - Created authentication context that stores JWT token, user info (email, firstName, lastName, profileId, roles), and provides login/logout functions
- **MainLayout** - Created layout for authenticated pages with language toggle, dark mode toggle, and logout button
- **MyProfilePage** - Created placeholder page for user profile (all authenticated users)
- **AdminPortalPage** - Created placeholder page for admin portal (Admin/Moderator only)
- **ProtectedRoute** - Created route wrapper that checks authentication and optional role requirements
- **Role-based redirection** - After login, Admin/Moderator users redirect to /admin, others redirect to /profile
- **Dark mode background fix** - Added `backgroundColor: 'base'` to MainLayout and AuthLayout root containers for proper dark mode support
- **Routing structure**:
  - `/login` → LoginPage (public)
  - `/signup` → SignupPage (public)
  - `/profile` → MyProfilePage (protected, all authenticated)
  - `/admin` → AdminPortalPage (protected, Admin/Moderator only)
  - `/` → Redirects based on auth status and role

### Next Steps
1. Build out MyProfile page with user profile form
2. Build out AdminPortal page with admin features
3. Add navigation menu for switching between pages

### Environment Variables
| Variable | Purpose | Default |
|----------|---------|---------|
| `VITE_API_URL` | Full backend API URL (scheme + host + port + /api) | `http://localhost:5001/api` |
| `VITE_ENV` | Environment label string | `development` |

**To change the backend domain/port:** edit only `VITE_API_URL` in `.env.development` (dev) or `.env.production` (prod build). No code changes needed.

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
