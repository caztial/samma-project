import axios from 'axios';
import { API_BASE_URL } from '../config';

/**
 * Create a shared axios instance for API calls.
 * 
 * @param {Object} options - Configuration options
 * @param {function} [options.getToken] - Function that returns the current auth token
 * @param {function} [options.onUnauthorized] - Callback for 401 responses (logout + redirect)
 * @returns {import('axios').AxiosInstance} Configured axios instance
 */
export function createApiClient({ getToken, onUnauthorized } = {}) {
  const api = axios.create({
    baseURL: API_BASE_URL,
    headers: {
      'Content-Type': 'application/json',
    },
  });

  // Request interceptor: Add Bearer token if available
  api.interceptors.request.use(
    (config) => {
      const token = getToken?.();
      if (token) {
        config.headers.Authorization = `Bearer ${token}`;
      }
      return config;
    },
    (error) => Promise.reject(error)
  );

  // Response interceptor: Handle 401 Unauthorized globally
  api.interceptors.response.use(
    (response) => response,
    (error) => {
      // Handle 401 Unauthorized - token expired or invalid
      if (error.response?.status === 401) {
        console.warn('[apiClient] Received 401 Unauthorized - token may be expired');
        onUnauthorized?.(error);
      }
      return Promise.reject(error);
    }
  );

  return api;
}

/**
 * Create a public API client (no auth token injection).
 * Used for login, register, and other public endpoints.
 * 
 * @returns {import('axios').AxiosInstance} Configured axios instance
 */
export function createPublicApiClient() {
  return axios.create({
    baseURL: API_BASE_URL,
    headers: {
      'Content-Type': 'application/json',
    },
  });
}

export default createApiClient;