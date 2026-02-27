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
│   ├── i18n/
│   │   ├── LocaleContext.jsx
│   │   ├── useTranslation.js
│   │   └── locales/
│   │       ├── en-US.json
│   │       └── si-LK.json
│   ├── contexts/          (empty)
│   ├── pages/             (empty)
│   ├── App.jsx            (default Vite template)
│   └── main.jsx           (default Vite template)
├── .env.development
└── vite.config.js
```

### Key Decisions
- **React Context** for state management (not Zustand)
- **React Spectrum S2** for UI components
- **JSX** (not TypeScript)
- **Mobile-first** for client pages, tablet/laptop for admin pages
- **i18n from day one** - English and Sinhala supported

### Environment Variables
```
VITE_API_URL=http://localhost:5001/api
VITE_ENV=development
```

### Running Services
- Frontend Dev Server: http://localhost:5173
- Backend API: http://localhost:5001 (or Docker port 8080)

### Next Steps
1. Create AuthContext for authentication state
2. Create Login page with language switcher
3. Set up routing with React Router
4. Build dashboard pages incrementally

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
