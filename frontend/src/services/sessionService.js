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
     * Join a session by session ID
     * @param {string} sessionId - The session ID
     * @returns {Promise<Object>} Session details
     */
    joinSession: async (sessionId) => {
      const response = await apiClient.post(`/sessions/${sessionId}/join`);
      return response.data;
    },
  };
}

export default createSessionService;