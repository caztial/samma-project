import { createApiClient } from './apiClient';

/**
 * Create a question service for session question operations
 * @param {Object} options - Configuration options
 * @param {Function} options.getToken - Function to get the auth token
 * @param {Function} options.onUnauthorized - Function to handle 401 errors
 * @returns {Object} Question service methods
 */
export function createQuestionService({ getToken, onUnauthorized }) {
  const apiClient = createApiClient({ getToken, onUnauthorized });

  return {
    /**
     * Get all currently presented questions for a session
     * Returns an array of PresentedMcqQuestionResponse
     * @param {string} sessionId - The session ID
     * @returns {Promise<Array>} Array of presented MCQ question responses
     */
    getPresentedQuestions: async (sessionId) => {
      const response = await apiClient.get(`/sessions/${sessionId}/presented`);
      // API may return a single object or an array — normalise to array
      const data = response.data;
      return Array.isArray(data) ? data : data ? [data] : [];
    },

    /**
     * Submit an MCQ answer for a specific question attempt
     * Returns 202 Accepted with a CommandId for SignalR correlation
     * @param {string} sessionId - The session ID
     * @param {string} questionId - The question ID
     * @param {number} attemptNumber - The attempt number
     * @param {string} selectedOptionId - The selected option ID (UUID)
     * @returns {Promise<Object>} SubmitAnswerAcceptedResponse { commandId, status, message, ... }
     */
    submitAnswer: async (sessionId, questionId, attemptNumber, selectedOptionId) => {
      const response = await apiClient.post(
        `/sessions/${sessionId}/questions/${questionId}/attempts/${attemptNumber}/answers`,
        { selectedOptionId }
      );
      return response.data;
    },
  };
}

export default createQuestionService;
