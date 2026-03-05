/**
 * Session storage utilities for managing current session state in localStorage
 */

export const SESSION_STORAGE_KEY = 'dhamma-current-session';

// ─── Answer State Storage ────────────────────────────────────────────────────
// Persists selected options and submitted answers per session so state survives
// page reloads and navigation. Cleared on logout via clearAllAnswerStates().

export const ANSWER_STATE_KEY = 'dhamma-answer-state';

/**
 * Save answer state for a specific session.
 * Merges with any existing states for other sessions.
 *
 * @param {string} sessionId
 * @param {{ selectedOptions: Object, submittedAnswers: Object }} state
 *   selectedOptions  – { [questionId]: optionId } — last selected option per question
 *   submittedAnswers – { [questionId]: { [attemptNumber]: optionId } } — confirmed submissions
 */
export function saveAnswerState(sessionId, { selectedOptions, submittedAnswers }) {
  const all = getAllAnswerStates();
  all[sessionId] = {
    selectedOptions: selectedOptions ?? {},
    submittedAnswers: submittedAnswers ?? {},
    savedAt: new Date().toISOString(),
  };
  localStorage.setItem(ANSWER_STATE_KEY, JSON.stringify(all));
}

/**
 * Get persisted answer state for a specific session.
 * @param {string} sessionId
 * @returns {{ selectedOptions: Object, submittedAnswers: Object } | null}
 */
export function getAnswerState(sessionId) {
  const all = getAllAnswerStates();
  return all[sessionId] ?? null;
}

/**
 * Clear answer state for a single session (e.g. when session ends).
 * @param {string} sessionId
 */
export function clearAnswerState(sessionId) {
  const all = getAllAnswerStates();
  delete all[sessionId];
  localStorage.setItem(ANSWER_STATE_KEY, JSON.stringify(all));
}

/**
 * Clear ALL answer states — call this on logout so the next user starts fresh.
 */
export function clearAllAnswerStates() {
  localStorage.removeItem(ANSWER_STATE_KEY);
}

/** Internal: read the full answer-state map. */
function getAllAnswerStates() {
  const raw = localStorage.getItem(ANSWER_STATE_KEY);
  if (!raw) return {};
  try {
    return JSON.parse(raw);
  } catch {
    return {};
  }
}

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


