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
    Task<Address?> AddAddressAsync(Guid profileId, Address address);
    Task<bool> RemoveAddressAsync(Guid profileId, Guid addressId);
    Task<Address?> UpdateAddressAsync(Guid profileId, Guid addressId, Address address);

    // ========== Identifications ==========
    Task<Identification?> AddIdentificationAsync(Guid profileId, Identification identification);
    Task<bool> RemoveIdentificationAsync(Guid profileId, Guid identificationId);
    Task<Identification?> UpdateIdentificationAsync(Guid profileId, Guid identificationId, Identification identification);

    // ========== Consents ==========
    Task<Consent?> AddConsentAsync(Guid profileId, Consent consent);
    Task<bool> RemoveConsentAsync(Guid profileId, Guid consentId);
    Task<Consent?> UpdateConsentAsync(Guid profileId, Guid consentId, Consent consent);
}
