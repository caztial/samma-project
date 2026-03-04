import { createApiClient } from './apiClient';

/**
 * Create a session service for participant session operations
 * @param {Object} options - Configuration options
 * @param {Function} options.getToken - Function to get the auth token
 * @param {Function} options.onUnauthorized - Function to handle 401 errors
 * @returns {Object} Session service methods
 */
export function createSessionService({ getToken, onUnauthorized }) {
  const apiClient = createApiClient({ getToken, onUnauthorized });

  return {
    /**
     * Join a session by session code
     * @param {string} sessionCode - The session code
     * @returns {Promise<Object>} Session participant data
     */
    joinSession: async (sessionCode) => {
      const response = await apiClient.post('/sessions/join', { Code: sessionCode });
      return response.data;
    },

    /**
     * Get session details by session ID
     * @param {string} sessionId - The session ID
     * @returns {Promise<Object>} Session details
     */
    getSession: async (sessionId) => {
      const response = await apiClient.get(`/sessions/${sessionId}`);
      return response.data;
    },

    /**
     * Leave a session
     * @param {string} sessionId - The session ID
     * @returns {Promise<void>}
     */
    leaveSession: async (sessionId) => {
      await apiClient.post(`/sessions/${sessionId}/leave`);
    },
  };
}

export default createSessionService;
