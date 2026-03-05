/**
 * Application-level configuration derived from Vite environment variables.
 *
 * To change the API domain/port, update VITE_API_URL in the appropriate .env file:
 *   .env.development  → local development
 *   .env.production   → production build
 *   .env.local        → machine-specific overrides (git-ignored)
 */

/** Full base URL for the backend API (e.g. "http://localhost:5001/api") */
export const API_BASE_URL =
  import.meta.env.VITE_API_URL ?? 'http://localhost:5001/api';

/**
 * Base URL for the backend server (without /api suffix).
 * Used for SignalR connections which have their own routes.
 */
export const BACKEND_BASE_URL = API_BASE_URL.replace(/\/api\/?$/, '');

/** SignalR hub URL for session real-time communication */
export const SIGNALR_HUB_URL = `${BACKEND_BASE_URL}hub/session`;

/** Current environment label ("development" | "production" | ...) */
export const APP_ENV = import.meta.env.VITE_ENV ?? import.meta.env.MODE;
