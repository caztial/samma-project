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

    // ========== Addresses ==========
    Task<Address?> AddAddressAsync(Guid profileId, Address address);
    Task<bool> RemoveAddressAsync(Guid profileId, Guid addressId);

    // ========== Identifications ==========
    Task<Identification?> AddIdentificationAsync(Guid profileId, Identification identification);
    Task<bool> RemoveIdentificationAsync(Guid profileId, Guid identificationId);

    // ========== Consents ==========
    Task<Consent?> AddConsentAsync(Guid profileId, Consent consent);
    Task<bool> RemoveConsentAsync(Guid profileId, Guid consentId);
}
