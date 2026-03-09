# Active Context

## Current Phase: Admin Question Bank Management (Completed)

### Recent Work (Mar 9, 2026)
- **QuestionsTable Implementation**: Implemented fully functional `QuestionsTable` component with:
  - Search (debounced)
  - Tag filtering (typeahead, top 5 matches)
  - Paginated TableView (resizable columns, checkbox selection)
  - CRUD action buttons (Edit, Delete with confirmation)
  - Verified status indicator icon
  - Fixed pagination alignment and re-fetching logic.
- **Admin UI Fixes**: Updated `QuestionsPage`, `DashboardPage`, `SessionsPage`, and `UsersPage` to use supported heading tokens (`heading`) to resolve S2 macro build errors.

### Previous Work Summary
- SignalR Integration (Mar 6, 2026) - Real-time communication in `ActiveSessionPage`.
- Profile Overview UI Enhancements (Mar 3, 2026) - Identification, Emergency Contact, and Address dialogs.
- Shared API Client Module (Mar 3, 2026) - Centralized axios instance factory with 401 handling.

## Important Patterns & Preferences
- **S2 Style Macros**: Use supported tokens (e.g., `heading`, `body-sm`) for all styling.
- **Async Pattern**: High-frequency operations (e.g., answer submission) use queue-based async processing with SignalR notifications for results.
- **Service Layer**: Factory-based services using a shared `apiClient` for consistent auth token injection.
- **State Management**: React Context for Auth, Theme, and Language.

## Next Steps
- Implement Edit functionality for questions.
- Build out `SessionsPage` and `UsersPage` within Admin portal.
- Testing and deployment preparation.
