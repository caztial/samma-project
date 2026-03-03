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
     * @param {string} profileId - The profile ID
     * @param {object} profileData - The profile data to update
     * @returns {Promise<import('axios').AxiosResponse>}
     */
    updateProfile: (profileId, profileData) => api.put(`/profile/${profileId}`, profileData),

    // Addresses
    getAddresses: (profileId) => api.get(`/profile/${profileId}/addresses`),
    addAddress: (profileId, addressData) => api.post(`/profile/${profileId}/addresses`, addressData),
    updateAddress: (profileId, addressId, addressData) => api.put(`/profile/${profileId}/addresses/${addressId}`, addressData),
    removeAddress: (profileId, addressId) => api.delete(`/profile/${profileId}/addresses/${addressId}`),

    // Education
    getEducations: (profileId) => api.get(`/profile/${profileId}/educations`),
    addEducation: (profileId, educationData) => api.post(`/profile/${profileId}/educations`, educationData),
    updateEducation: (profileId, educationId, educationData) => api.put(`/profile/${profileId}/educations/${educationId}`, educationData),
    removeEducation: (profileId, educationId) => api.delete(`/profile/${profileId}/educations/${educationId}`),

    // Bank Accounts
    getBankAccounts: (profileId) => api.get(`/profile/${profileId}/bank-accounts`),
    addBankAccount: (profileId, bankAccountData) => api.post(`/profile/${profileId}/bank-accounts`, bankAccountData),
    updateBankAccount: (profileId, bankAccountId, bankAccountData) => api.put(`/profile/${profileId}/bank-accounts/${bankAccountId}`, bankAccountData),
    removeBankAccount: (profileId, bankAccountId) => api.delete(`/profile/${profileId}/bank-accounts/${bankAccountId}`),

    // Emergency Contacts
    getEmergencyContacts: (profileId) => api.get(`/profile/${profileId}/emergency-contacts`),
    addEmergencyContact: (profileId, contactData) => api.post(`/profile/${profileId}/emergency-contacts`, contactData),
    updateEmergencyContact: (profileId, contactId, contactData) => api.put(`/profile/${profileId}/emergency-contacts/${contactId}`, contactData),
    removeEmergencyContact: (profileId, contactId) => api.delete(`/profile/${profileId}/emergency-contacts/${contactId}`),

    // Identifications
    getIdentifications: (profileId) => api.get(`/profile/${profileId}/identifications`),
    addIdentification: (profileId, identificationData) => api.post(`/profile/${profileId}/identifications`, identificationData),
    updateIdentification: (profileId, identificationId, identificationData) => api.put(`/profile/${profileId}/identifications/${identificationId}`, identificationData),
    removeIdentification: (profileId, identificationId) => api.delete(`/profile/${profileId}/identifications/${identificationId}`),

    // Consents
    getConsents: (profileId) => api.get(`/profile/${profileId}/consents`),
    addConsent: (profileId, consentData) => api.post(`/profile/${profileId}/consents`, consentData),
    updateConsent: (profileId, consentId, consentData) => api.put(`/profile/${profileId}/consents/${consentId}`, consentData),
    removeConsent: (profileId, consentId) => api.delete(`/profile/${profileId}/consents/${consentId}`),

    // Biometrics
    getBiometrics: () => api.get('/profile/biometrics'),
    updateBiometrics: (biometricsData) => api.put('/profile/biometrics', biometricsData),
  };
}

export default createProfileService;