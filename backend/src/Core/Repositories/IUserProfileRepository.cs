using Core.Entities.UserProfiles;
using Core.Entities.ValueObjects;

namespace Core.Repositories;

public interface IUserProfileRepository
{
    Task<UserProfile?> GetByUserIdAsync(string userId);
    Task<UserProfile?> GetByIdAsync(Guid id);
    Task<UserProfile> CreateAsync(UserProfile userProfile);
    Task<UserProfile> UpdateAsync(UserProfile userProfile);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(string userId);

    // ========== Emergency Contacts ==========
    Task<EmergencyContact?> AddEmergencyContactAsync(Guid profileId, EmergencyContact emergencyContact);
    Task<bool> RemoveEmergencyContactAsync(Guid profileId, Guid emergencyContactId);
    Task<EmergencyContact?> UpdateEmergencyContactAsync(Guid profileId, Guid emergencyContactId, EmergencyContact emergencyContact);

    // ========== Addresses ==========
    Task<UserAddress?> AddAddressAsync(Guid profileId, UserAddress address);
    Task<bool> RemoveAddressAsync(Guid profileId, Guid addressId);
    Task<UserAddress?> UpdateAddressAsync(Guid profileId, Guid addressId, UserAddress address);

    // ========== Identifications ==========
    Task<Identification?> AddIdentificationAsync(Guid profileId, Identification identification);
    Task<bool> RemoveIdentificationAsync(Guid profileId, Guid identificationId);
    Task<Identification?> UpdateIdentificationAsync(Guid profileId, Guid identificationId, Identification identification);

    // ========== Consents ==========
    Task<UserConsent?> AddConsentAsync(Guid profileId, UserConsent consent);
    Task<bool> RemoveConsentAsync(Guid profileId, Guid consentId);
    Task<UserConsent?> UpdateConsentAsync(Guid profileId, Guid consentId, UserConsent consent);

    // ========== Educations ==========
    Task<Education?> AddEducationAsync(Guid profileId, Education education);
    Task<bool> RemoveEducationAsync(Guid profileId, Guid educationId);
    Task<Education?> UpdateEducationAsync(Guid profileId, Guid educationId, Education education);

    // ========== Bank Accounts ==========
    Task<BankAccount?> AddBankAccountAsync(Guid profileId, BankAccount bankAccount);
    Task<bool> RemoveBankAccountAsync(Guid profileId, Guid bankAccountId);
    Task<BankAccount?> UpdateBankAccountAsync(Guid profileId, Guid bankAccountId, BankAccount bankAccount);
}