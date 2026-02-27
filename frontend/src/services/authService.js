import axios from 'axios';
import { API_BASE_URL } from '../config';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

export const authService = {
  /**
   * Login with email and password.
   * @param {string} email
   * @param {string} password
   * @returns {Promise<import('axios').AxiosResponse>}
   */
  login: (email, password) =>
    api.post('/auth/login', { email, password }),

  /**
   * Register a new user.
   * @param {string} firstName
   * @param {string} lastName
   * @param {string} email
   * @param {string} password
   * @returns {Promise<import('axios').AxiosResponse>}
   */
  register: (firstName, lastName, email, password) =>
    api.post('/auth/register', { firstName, lastName, email, password }),
};

export default authService;
