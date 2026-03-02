import { createPublicApiClient } from './apiClient';

/**
 * Auth service for public authentication endpoints.
 * Uses a public API client without auth token injection.
 */

// Create a single public API instance for auth endpoints
const publicApi = createPublicApiClient();

export const authService = {
  /**
   * Login with email and password.
   * @param {string} email
   * @param {string} password
   * @returns {Promise<import('axios').AxiosResponse>}
   */
  login: (email, password) =>
    publicApi.post('/auth/login', { email, password }),

  /**
   * Register a new user.
   * @param {string} firstName
   * @param {string} lastName
   * @param {string} email
   * @param {string} password
   * @returns {Promise<import('axios').AxiosResponse>}
   */
  register: (firstName, lastName, email, password) =>
    publicApi.post('/auth/register', { firstName, lastName, email, password }),
};

export default authService;