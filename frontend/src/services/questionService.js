import { createApiClient } from './apiClient';

/**
 * Create a question service for question operations
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

    /**
     * Get paginated list of questions with optional filters
     * @param {Object} params - Query parameters
     * @param {number} params.pageNumber - Page number (default: 1)
     * @param {number} params.pageSize - Page size (default: 20)
     * @param {string} [params.searchText] - Search text for question text/number
     * @param {string} [params.tags] - Comma-separated tag names
     * @param {string} [params.questionType] - Question type filter (e.g., "MCQ")
     * @returns {Promise<Object>} QuestionListResponse { items, totalCount, pageNumber, pageSize, totalPages }
     */
    listQuestions: async (params = {}) => {
      const { pageNumber = 1, pageSize = 20, searchText, tags, questionType } = params;
      const queryParams = new URLSearchParams();
      queryParams.append('pageNumber', pageNumber.toString());
      queryParams.append('pageSize', pageSize.toString());
      if (searchText) queryParams.append('searchText', searchText);
      if (tags) queryParams.append('tags', tags);
      if (questionType) queryParams.append('questionType', questionType);

      const response = await apiClient.get(`/questions?${queryParams.toString()}`);
      return response.data;
    },

    /**
     * Search tags for autocomplete/typeahead
     * @param {string} search - Search text for tag name
     * @returns {Promise<Array>} Array of TagResponse { id, name }
     */
    searchTags: async (search = '') => {
      const queryParams = new URLSearchParams();
      if (search) queryParams.append('search', search);
      const response = await apiClient.get(`/tags?${queryParams.toString()}`);
      return response.data;
    },

    /**
     * Delete a question by ID
     * @param {string} questionId - The question ID
     * @returns {Promise<void>}
     */
    deleteQuestion: async (questionId) => {
      await apiClient.delete(`/questions/${questionId}`);
    },

    /**
     * Get a single question by ID (MCQ question with answer options)
     * @param {string} questionId - The question ID
     * @returns {Promise<Object>} MCQQuestionResponse
     */
    getQuestion: async (questionId) => {
      const response = await apiClient.get(`/questions/${questionId}/mcq`);
      return response.data;
    },
  };
}

export default createQuestionService;
