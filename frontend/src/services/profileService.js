import { createApiClient } from './apiClient';

/**
 * Create a profile service for fetching and updating user profile data.
 * 
 * @param {Object} options - Configuration options
 * @param {function} options.getToken - Function that returns the current auth token
 * @param {function} [options.onUnauthorized] - Callback for 401 responses
 * @returns {Object} Profile service with CRUD methods
 */
export function createProfileService({ getToken, onUnauthorized }) {
  // Create API client with auth token injection and 401 handling
  const api = createApiClient({ getToken, onUnauthorized });

  return {
    /**
     * Get the current user's profile.
     * Calls GET /profile (without ID) which returns the authenticated user's profile.
     * @returns {Promise<import('axios').AxiosResponse>}
     */
    getProfile: () => api.get('/profile'),

    /**
     * Get a specific user's profile by ID.
     * @param {string} profileId - The profile ID
     * @returns {Promise<import('axios').AxiosResponse>}
     */
    getProfileById: (profileId) => api.get(`/profile/${profileId}`),

    /**
     * Update the current user's profile.
     * @param {object} profileData - The profile data to update
     * @returns {Promise<import('axios').AxiosResponse>}
     */
    updateProfile: (profileData) => api.put('/profile', profileData),

    // Addresses
    getAddresses: () => api.get('/profile/addresses'),
    addAddress: (addressData) => api.post('/profile/addresses', addressData),
    updateAddress: (addressId, addressData) => api.put(`/profile/addresses/${addressId}`, addressData),
    removeAddress: (addressId) => api.delete(`/profile/addresses/${addressId}`),

    // Education
    getEducations: () => api.get('/profile/educations'),
    addEducation: (educationData) => api.post('/profile/educations', educationData),
    updateEducation: (educationId, educationData) => api.put(`/profile/educations/${educationId}`, educationData),
    removeEducation: (educationId) => api.delete(`/profile/educations/${educationId}`),

    // Bank Accounts
    getBankAccounts: () => api.get('/profile/bank-accounts'),
    addBankAccount: (bankAccountData) => api.post('/profile/bank-accounts', bankAccountData),
    updateBankAccount: (bankAccountId, bankAccountData) => api.put(`/profile/bank-accounts/${bankAccountId}`, bankAccountData),
    removeBankAccount: (bankAccountId) => api.delete(`/profile/bank-accounts/${bankAccountId}`),

    // Emergency Contacts
    getEmergencyContacts: () => api.get('/profile/emergency-contacts'),
    addEmergencyContact: (contactData) => api.post('/profile/emergency-contacts', contactData),
    updateEmergencyContact: (contactId, contactData) => api.put(`/profile/emergency-contacts/${contactId}`, contactData),
    removeEmergencyContact: (contactId) => api.delete(`/profile/emergency-contacts/${contactId}`),

    // Identifications
    getIdentifications: () => api.get('/profile/identifications'),
    addIdentification: (identificationData) => api.post('/profile/identifications', identificationData),
    updateIdentification: (identificationId, identificationData) => api.put(`/profile/identifications/${identificationId}`, identificationData),
    removeIdentification: (identificationId) => api.delete(`/profile/identifications/${identificationId}`),

    // Consents
    getConsents: () => api.get('/profile/consents'),
    addConsent: (consentData) => api.post('/profile/consents', consentData),
    updateConsent: (consentId, consentData) => api.put(`/profile/consents/${consentId}`, consentData),
    removeConsent: (consentId) => api.delete(`/profile/consents/${consentId}`),

    // Biometrics
    getBiometrics: () => api.get('/profile/biometrics'),
    updateBiometrics: (biometricsData) => api.put('/profile/biometrics', biometricsData),
  };
}

export default createProfileService;