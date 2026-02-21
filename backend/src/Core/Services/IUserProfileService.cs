using Core.Entities.UserProfiles;
using Core.Entities.ValueObjects;
using Core.Enums;
using Core.Events;

namespace Core.Services;

public interface IUserProfileService
{
    /// <summary>
    /// Creates a new UserProfile from a UserCreatedEvent and saves it to the database.
    /// </summary>
    Task<UserProfile> CreateFromEventAsync(UserCreatedEvent userEvent);

    /// <summary>
    /// Gets UserProfile by user ID.
    /// </summary>
    Task<UserProfile?> GetByUserIdAsync(string userId);

    /// <summary>
    /// Gets UserProfile by aggregate ID.
    /// </summary>
    Task<UserProfile?> GetByIdAsync(Guid id);

    /// <summary>
    /// Updates the basic profile information and contact.
    /// </summary>
    Task<UserProfile?> UpdateProfileAsync(
        Guid id,
        string? firstName,
        string? lastName,
        string? profileImageUrl,
        Gender? gender,
        DateOnly? dateOfBirth,
        Contact? contact
    );

    /// <summary>
    /// Updates the biometrics data.
    /// </summary>
    Task<UserProfile?> UpdateBiometricsAsync(Guid id, Biometrics biometrics);

    // ========== Emergency Contacts ==========

    /// <summary>
    /// Gets all emergency contacts for a profile.
    /// </summary>
    Task<IEnumerable<EmergencyContact>> GetEmergencyContactsAsync(Guid profileId);

    /// <summary>
    /// Adds an emergency contact to the profile.
    /// </summary>
    Task<EmergencyContact?> AddEmergencyContactAsync(
        Guid profileId,
        EmergencyContact emergencyContact
    );

    /// <summary>
    /// Removes an emergency contact from the profile.
    /// </summary>
    Task<bool> RemoveEmergencyContactAsync(Guid profileId, Guid emergencyContactId);

    /// <summary>
    /// Updates an emergency contact in the profile.
    /// </summary>
    Task<EmergencyContact?> UpdateEmergencyContactAsync(
        Guid profileId,
        Guid emergencyContactId,
        EmergencyContact emergencyContact
    );

    // ========== Addresses ==========

    /// <summary>
    /// Gets all addresses for a profile.
    /// </summary>
    Task<IEnumerable<UserAddress>> GetAddressesAsync(Guid profileId);

    /// <summary>
    /// Adds an address to the profile.
    /// </summary>
    Task<UserAddress?> AddAddressAsync(Guid profileId, UserAddress address);

    /// <summary>
    /// Removes an address from the profile.
    /// </summary>
    Task<bool> RemoveAddressAsync(Guid profileId, Guid addressId);

    /// <summary>
    /// Updates an address in the profile.
    /// </summary>
    Task<UserAddress?> UpdateAddressAsync(
        Guid profileId,
        Guid addressId,
        UserAddress address
    );

    // ========== Identifications ==========

    /// <summary>
    /// Gets all identifications for a profile.
    /// </summary>
    Task<IEnumerable<Identification>> GetIdentificationsAsync(Guid profileId);

    /// <summary>
    /// Adds an identification to the profile.
    /// </summary>
    Task<Identification?> AddIdentificationAsync(Guid profileId, Identification identification);

    /// <summary>
    /// Removes an identification from the profile.
    /// </summary>
    Task<bool> RemoveIdentificationAsync(Guid profileId, Guid identificationId);

    /// <summary>
    /// Updates an identification in the profile.
    /// </summary>
    Task<Identification?> UpdateIdentificationAsync(
        Guid profileId,
        Guid identificationId,
        Identification identification
    );

    // ========== Consents ==========

    /// <summary>
    /// Gets all consents for a profile.
    /// </summary>
    Task<IEnumerable<UserConsent>> GetConsentsAsync(Guid profileId);

    /// <summary>
    /// Adds a consent to the profile.
    /// </summary>
    Task<UserConsent?> AddConsentAsync(Guid profileId, UserConsent consent);

    /// <summary>
    /// Removes a consent from the profile.
    /// </summary>
    Task<bool> RemoveConsentAsync(Guid profileId, Guid consentId);

    /// <summary>
    /// Updates a consent in the profile.
    /// </summary>
    Task<UserConsent?> UpdateConsentAsync(
        Guid profileId,
        Guid consentId,
        UserConsent consent
    );

    // ========== Educations ==========

    /// <summary>
    /// Gets all educations for a profile.
    /// </summary>
    Task<IEnumerable<Education>> GetEducationsAsync(Guid profileId);

    /// <summary>
    /// Adds an education to the profile.
    /// </summary>
    Task<Education?> AddEducationAsync(Guid profileId, Education education);

    /// <summary>
    /// Removes an education from the profile.
    /// </summary>
    Task<bool> RemoveEducationAsync(Guid profileId, Guid educationId);

    /// <summary>
    /// Updates an education in the profile.
    /// </summary>
    Task<Education?> UpdateEducationAsync(
        Guid profileId,
        Guid educationId,
        Education education
    );

    // ========== Bank Accounts ==========

    /// <summary>
    /// Gets all bank accounts for a profile.
    /// </summary>
    Task<IEnumerable<BankAccount>> GetBankAccountsAsync(Guid profileId);

    /// <summary>
    /// Adds a bank account to the profile.
    /// </summary>
    Task<BankAccount?> AddBankAccountAsync(Guid profileId, BankAccount bankAccount);

    /// <summary>
    /// Removes a bank account from the profile.
    /// </summary>
    Task<bool> RemoveBankAccountAsync(Guid profileId, Guid bankAccountId);

    /// <summary>
    /// Updates a bank account in the profile.
    /// </summary>
    Task<BankAccount?> UpdateBankAccountAsync(
        Guid profileId,
        Guid bankAccountId,
        BankAccount bankAccount
    );
}