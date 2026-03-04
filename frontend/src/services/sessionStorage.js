/**
 * Session storage utilities for managing current session state in localStorage
 */

export const SESSION_STORAGE_KEY = 'dhamma-current-session';

/**
 * Save current session data to localStorage
 * @param {Object} sessionData - Session data to save
 */
export function saveCurrentSession(sessionData) {
  localStorage.setItem(SESSION_STORAGE_KEY, JSON.stringify(sessionData));
}

/**
 * Retrieve current session data from localStorage
 * @returns {Object|null} Current session data or null if not found
 */
export function getCurrentSession() {
  const data = localStorage.getItem(SESSION_STORAGE_KEY);
  return data ? JSON.parse(data) : null;
}

/**
 * Clear current session data from localStorage
 */
export function clearCurrentSession() {
  localStorage.removeItem(SESSION_STORAGE_KEY);
}

/**
 * Check if session is still active by calling the backend
 * @param {Object} sessionData - Session data to validate
 * @param {Object} sessionService - Session service instance for API calls
 * @returns {Promise<boolean>} True if session is active
 */
export async function isSessionValid(sessionData, sessionService) {
  if (!sessionData || !sessionService) return false;
  
  try {
    // Call the session get endpoint to check current status
    const sessionResponse = await sessionService.getSession(sessionData.sessionId);
    
    // Check if session is still active
    const isActive = sessionResponse.state === 'Active';
    
    if (isActive) {
      // Session is active, return true (don't update currentSession)
      return true;
    } else {
      // Session is not active, clear it from localStorage
      clearCurrentSession();
      return false;
    }
  } catch (error) {
    // If we can't reach the backend, assume session might still be valid
    // Don't clear it - let user try to rejoin
    return false;
  }
}


