using Core.Entities.UserProfiles;
using Core.Entities.ValueObjects;
using Core.Enums;
using Core.Events;
using Core.Repositories;

namespace Core.Services;

public class UserProfileService : IUserProfileService
{
    private readonly IUserProfileRepository _userProfileRepository;

    public UserProfileService(IUserProfileRepository userProfileRepository)
    {
        _userProfileRepository = userProfileRepository;
    }

    public async Task<UserProfile> CreateFromEventAsync(UserCreatedEvent userEvent)
    {
        // Check if profile already exists
        if (await _userProfileRepository.ExistsAsync(userEvent.UserId))
        {
            var existing = await _userProfileRepository.GetByUserIdAsync(userEvent.UserId);
            if (existing != null)
            {
                return existing;
            }
        }

        // Create UserProfile from event
        var userProfile = UserProfile.CreateFromEvent(userEvent);

        // Save to database
        return await _userProfileRepository.CreateAsync(userProfile);
    }

    public async Task<UserProfile?> GetByUserIdAsync(string userId)
    {
        return await _userProfileRepository.GetByUserIdAsync(userId);
    }

    public async Task<UserProfile?> GetByIdAsync(Guid id)
    {
        return await _userProfileRepository.GetByIdAsync(id);
    }

    public async Task<UserProfile?> UpdateProfileAsync(
        Guid id,
        string? firstName,
        string? lastName,
        string? profileImageUrl,
        Gender? gender,
        DateOnly? dateOfBirth,
        Contact? contact)
    {
        var profile = await _userProfileRepository.GetByIdAsync(id);
        if (profile == null)
            return null;

        // Update fields if provided
        if (firstName != null)
            profile.FirstName = firstName;
        if (lastName != null)
            profile.LastName = lastName;
        if (profileImageUrl != null)
            profile.ProfileImageUrl = profileImageUrl;
        if (gender.HasValue)
            profile.Gender = gender.Value;
        if (dateOfBirth.HasValue)
            profile.DateOfBirth = dateOfBirth.Value;
        if (contact != null)
            profile.Contact = contact;

        return await _userProfileRepository.UpdateAsync(profile);
    }

    public async Task<UserProfile?> UpdateBiometricsAsync(Guid id, Biometrics biometrics)
    {
        var profile = await _userProfileRepository.GetByIdAsync(id);
        if (profile == null)
            return null;

        profile.Biometrics = biometrics;
        return await _userProfileRepository.UpdateAsync(profile);
    }

    // ========== Emergency Contacts ==========

    public async Task<IEnumerable<EmergencyContact>> GetEmergencyContactsAsync(Guid profileId)
    {
        var profile = await _userProfileRepository.GetByIdAsync(profileId);
        return profile?.EmergencyContacts ?? Enumerable.Empty<EmergencyContact>();
    }

    public async Task<EmergencyContact?> AddEmergencyContactAsync(Guid profileId, EmergencyContact emergencyContact)
    {
        return await _userProfileRepository.AddEmergencyContactAsync(profileId, emergencyContact);
    }

    public async Task<bool> RemoveEmergencyContactAsync(Guid profileId, Guid emergencyContactId)
    {
        return await _userProfileRepository.RemoveEmergencyContactAsync(profileId, emergencyContactId);
    }

    public async Task<EmergencyContact?> UpdateEmergencyContactAsync(
        Guid profileId,
        Guid emergencyContactId,
        EmergencyContact emergencyContact
    )
    {
        return await _userProfileRepository.UpdateEmergencyContactAsync(
            profileId,
            emergencyContactId,
            emergencyContact
        );
    }

    // ========== Addresses ==========

    public async Task<IEnumerable<UserAddress>> GetAddressesAsync(Guid profileId)
    {
        var profile = await _userProfileRepository.GetByIdAsync(profileId);
        return profile?.Addresses ?? Enumerable.Empty<UserAddress>();
    }

    public async Task<UserAddress?> AddAddressAsync(Guid profileId, UserAddress address)
    {
        return await _userProfileRepository.AddAddressAsync(profileId, address);
    }

    public async Task<bool> RemoveAddressAsync(Guid profileId, Guid addressId)
    {
        return await _userProfileRepository.RemoveAddressAsync(profileId, addressId);
    }

    public async Task<UserAddress?> UpdateAddressAsync(
        Guid profileId,
        Guid addressId,
        UserAddress address
    )
    {
        return await _userProfileRepository.UpdateAddressAsync(profileId, addressId, address);
    }

    // ========== Identifications ==========

    public async Task<IEnumerable<Identification>> GetIdentificationsAsync(Guid profileId)
    {
        var profile = await _userProfileRepository.GetByIdAsync(profileId);
        return profile?.Identifications ?? Enumerable.Empty<Identification>();
    }

    public async Task<Identification?> AddIdentificationAsync(Guid profileId, Identification identification)
    {
        return await _userProfileRepository.AddIdentificationAsync(profileId, identification);
    }

    public async Task<bool> RemoveIdentificationAsync(Guid profileId, Guid identificationId)
    {
        return await _userProfileRepository.RemoveIdentificationAsync(profileId, identificationId);
    }

    public async Task<Identification?> UpdateIdentificationAsync(
        Guid profileId,
        Guid identificationId,
        Identification identification
    )
    {
        return await _userProfileRepository.UpdateIdentificationAsync(
            profileId,
            identificationId,
            identification
        );
    }

    // ========== Consents ==========

    public async Task<IEnumerable<UserConsent>> GetConsentsAsync(Guid profileId)
    {
        var profile = await _userProfileRepository.GetByIdAsync(profileId);
        return profile?.Consents ?? Enumerable.Empty<UserConsent>();
    }

    public async Task<UserConsent?> AddConsentAsync(Guid profileId, UserConsent consent)
    {
        return await _userProfileRepository.AddConsentAsync(profileId, consent);
    }

    public async Task<bool> RemoveConsentAsync(Guid profileId, Guid consentId)
    {
        return await _userProfileRepository.RemoveConsentAsync(profileId, consentId);
    }

    public async Task<UserConsent?> UpdateConsentAsync(
        Guid profileId,
        Guid consentId,
        UserConsent consent
    )
    {
        return await _userProfileRepository.UpdateConsentAsync(profileId, consentId, consent);
    }

    // ========== Educations ==========

    public async Task<IEnumerable<Education>> GetEducationsAsync(Guid profileId)
    {
        var profile = await _userProfileRepository.GetByIdAsync(profileId);
        return profile?.Educations ?? Enumerable.Empty<Education>();
    }

    public async Task<Education?> AddEducationAsync(Guid profileId, Education education)
    {
        return await _userProfileRepository.AddEducationAsync(profileId, education);
    }

    public async Task<bool> RemoveEducationAsync(Guid profileId, Guid educationId)
    {
        return await _userProfileRepository.RemoveEducationAsync(profileId, educationId);
    }

    public async Task<Education?> UpdateEducationAsync(
        Guid profileId,
        Guid educationId,
        Education education
    )
    {
        return await _userProfileRepository.UpdateEducationAsync(profileId, educationId, education);
    }

    // ========== Bank Accounts ==========

    public async Task<IEnumerable<BankAccount>> GetBankAccountsAsync(Guid profileId)
    {
        var profile = await _userProfileRepository.GetByIdAsync(profileId);
        return profile?.BankAccounts ?? Enumerable.Empty<BankAccount>();
    }

    public async Task<BankAccount?> AddBankAccountAsync(Guid profileId, BankAccount bankAccount)
    {
        return await _userProfileRepository.AddBankAccountAsync(profileId, bankAccount);
    }

    public async Task<bool> RemoveBankAccountAsync(Guid profileId, Guid bankAccountId)
    {
        return await _userProfileRepository.RemoveBankAccountAsync(profileId, bankAccountId);
    }

    public async Task<BankAccount?> UpdateBankAccountAsync(
        Guid profileId,
        Guid bankAccountId,
        BankAccount bankAccount
    )
    {
        return await _userProfileRepository.UpdateBankAccountAsync(profileId, bankAccountId, bankAccount);
    }
}