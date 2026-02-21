using Core.Entities.UserProfiles;
using Core.Entities.ValueObjects;
using Core.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.PostgreSQL.Repositories;

public class UserProfileRepository : IUserProfileRepository
{
    private readonly ApplicationDbContext _context;

    public UserProfileRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserProfile?> GetByUserIdAsync(string userId)
    {
        return await _context.UserProfiles.FirstOrDefaultAsync(up => up.UserId == userId);
    }

    public async Task<UserProfile?> GetByIdAsync(Guid id)
    {
        return await _context
            .UserProfiles.Include(up => up.EmergencyContacts)
            .Include(up => up.Addresses)
            .Include(up => up.Identifications)
            .Include(up => up.Consents)
            .Include(up => up.Educations)
            .Include(up => up.BankAccounts)
            .FirstOrDefaultAsync(up => up.Id == id);
    }

    public async Task<UserProfile> CreateAsync(UserProfile userProfile)
    {
        userProfile.CreatedAt = DateTime.UtcNow;
        _context.UserProfiles.Add(userProfile);
        await _context.SaveChangesAsync();
        return userProfile;
    }

    public async Task<UserProfile> UpdateAsync(UserProfile userProfile)
    {
        userProfile.UpdatedAt = DateTime.UtcNow;
        _context.UserProfiles.Update(userProfile);
        await _context.SaveChangesAsync();
        return userProfile;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var userProfile = await _context.UserProfiles.FindAsync(id);
        if (userProfile == null)
            return false;

        _context.UserProfiles.Remove(userProfile);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(string userId)
    {
        return await _context.UserProfiles.AnyAsync(up => up.UserId == userId);
    }

    // ========== Emergency Contacts ==========

    public async Task<EmergencyContact?> AddEmergencyContactAsync(
        Guid profileId,
        EmergencyContact emergencyContact
    )
    {
        var profile = await _context
            .UserProfiles.Include(up => up.EmergencyContacts)
            .FirstOrDefaultAsync(up => up.Id == profileId);

        if (profile == null)
            return null;

        emergencyContact.UserProfileId = profileId;
        profile.EmergencyContacts.Add(emergencyContact);
        await _context.AddAsync(emergencyContact);
        profile.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return emergencyContact;
    }

    public async Task<bool> RemoveEmergencyContactAsync(Guid profileId, Guid emergencyContactId)
    {
        var profile = await _context
            .UserProfiles.Include(up => up.EmergencyContacts)
            .FirstOrDefaultAsync(up => up.Id == profileId);

        if (profile == null)
            return false;

        var emergencyContact = profile.EmergencyContacts.FirstOrDefault(ec =>
            ec.Id == emergencyContactId
        );
        if (emergencyContact == null)
            return false;

        profile.EmergencyContacts.Remove(emergencyContact);
        profile.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<EmergencyContact?> UpdateEmergencyContactAsync(
        Guid profileId,
        Guid emergencyContactId,
        EmergencyContact emergencyContact
    )
    {
        var profile = await _context
            .UserProfiles.Include(up => up.EmergencyContacts)
            .FirstOrDefaultAsync(up => up.Id == profileId);

        if (profile == null)
            return null;

        var existing = profile.EmergencyContacts.FirstOrDefault(ec => ec.Id == emergencyContactId);
        if (existing == null)
            return null;

        // Update the existing entity
        existing.Name = emergencyContact.Name;
        existing.Relationship = emergencyContact.Relationship;
        existing.Contact = emergencyContact.Contact;

        profile.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return existing;
    }

    // ========== Addresses ==========

    public async Task<UserAddress?> AddAddressAsync(Guid profileId, UserAddress address)
    {
        var profile = await _context
            .UserProfiles.Include(up => up.Addresses)
            .FirstOrDefaultAsync(up => up.Id == profileId);

        if (profile == null)
            return null;

        address.UserProfileId = profileId;
        profile.Addresses.Add(address);
        profile.UpdatedAt = DateTime.UtcNow;

        await _context.AddAsync(address);
        await _context.SaveChangesAsync();
        return address;
    }

    public async Task<bool> RemoveAddressAsync(Guid profileId, Guid addressId)
    {
        var profile = await _context
            .UserProfiles.Include(up => up.Addresses)
            .FirstOrDefaultAsync(up => up.Id == profileId);

        if (profile == null)
            return false;

        var address = profile.Addresses.FirstOrDefault(a => a.Id == addressId);
        if (address == null)
            return false;

        profile.Addresses.Remove(address);
        profile.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<UserAddress?> UpdateAddressAsync(Guid profileId, Guid addressId, UserAddress address)
    {
        var profile = await _context
            .UserProfiles.Include(up => up.Addresses)
            .FirstOrDefaultAsync(up => up.Id == profileId);

        if (profile == null)
            return null;

        var existing = profile.Addresses.FirstOrDefault(a => a.Id == addressId);
        if (existing == null)
            return null;

        // Update the existing entity
        existing.Type = address.Type;
        existing.Address = address.Address;

        profile.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return existing;
    }

    // ========== Identifications ==========

    public async Task<Identification?> AddIdentificationAsync(
        Guid profileId,
        Identification identification
    )
    {
        var profile = await _context
            .UserProfiles.Include(up => up.Identifications)
            .FirstOrDefaultAsync(up => up.Id == profileId);

        if (profile == null)
            return null;

        identification.UserProfileId = profileId;
        profile.Identifications.Add(identification);
        profile.UpdatedAt = DateTime.UtcNow;

        await _context.AddAsync(identification);
        await _context.SaveChangesAsync();
        return identification;
    }

    public async Task<bool> RemoveIdentificationAsync(Guid profileId, Guid identificationId)
    {
        var profile = await _context
            .UserProfiles.Include(up => up.Identifications)
            .FirstOrDefaultAsync(up => up.Id == profileId);

        if (profile == null)
            return false;

        var identification = profile.Identifications.FirstOrDefault(i => i.Id == identificationId);
        if (identification == null)
            return false;

        profile.Identifications.Remove(identification);
        profile.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Identification?> UpdateIdentificationAsync(
        Guid profileId,
        Guid identificationId,
        Identification identification
    )
    {
        var profile = await _context
            .UserProfiles.Include(up => up.Identifications)
            .FirstOrDefaultAsync(up => up.Id == profileId);

        if (profile == null)
            return null;

        var existing = profile.Identifications.FirstOrDefault(i => i.Id == identificationId);
        if (existing == null)
            return null;

        // Update the existing entity
        existing.Type = identification.Type;
        existing.Value = identification.Value;

        profile.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return existing;
    }

    // ========== Consents ==========

    public async Task<UserConsent?> AddConsentAsync(Guid profileId, UserConsent consent)
    {
        var profile = await _context
            .UserProfiles.Include(up => up.Consents)
            .FirstOrDefaultAsync(up => up.Id == profileId);

        if (profile == null)
            return null;

        consent.UserProfileId = profileId;
        profile.Consents.Add(consent);
        profile.UpdatedAt = DateTime.UtcNow;
        await _context.AddAsync(consent);
        await _context.SaveChangesAsync();
        return consent;
    }

    public async Task<bool> RemoveConsentAsync(Guid profileId, Guid consentId)
    {
        var profile = await _context
            .UserProfiles.Include(up => up.Consents)
            .FirstOrDefaultAsync(up => up.Id == profileId);

        if (profile == null)
            return false;

        var consent = profile.Consents.FirstOrDefault(c => c.Id == consentId);
        if (consent == null)
            return false;

        profile.Consents.Remove(consent);
        profile.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<UserConsent?> UpdateConsentAsync(Guid profileId, Guid consentId, UserConsent consent)
    {
        var profile = await _context
            .UserProfiles.Include(up => up.Consents)
            .FirstOrDefaultAsync(up => up.Id == profileId);

        if (profile == null)
            return null;

        var existing = profile.Consents.FirstOrDefault(c => c.Id == consentId);
        if (existing == null)
            return null;

        // Update the existing entity
        existing.Consent = consent.Consent;

        profile.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return existing;
    }

    // ========== Educations ==========

    public async Task<Education?> AddEducationAsync(Guid profileId, Education education)
    {
        var profile = await _context
            .UserProfiles.Include(up => up.Educations)
            .FirstOrDefaultAsync(up => up.Id == profileId);

        if (profile == null)
            return null;

        education.UserProfileId = profileId;
        profile.Educations.Add(education);
        profile.UpdatedAt = DateTime.UtcNow;

        await _context.AddAsync(education);
        await _context.SaveChangesAsync();
        return education;
    }

    public async Task<bool> RemoveEducationAsync(Guid profileId, Guid educationId)
    {
        var profile = await _context
            .UserProfiles.Include(up => up.Educations)
            .FirstOrDefaultAsync(up => up.Id == profileId);

        if (profile == null)
            return false;

        var education = profile.Educations.FirstOrDefault(e => e.Id == educationId);
        if (education == null)
            return false;

        profile.Educations.Remove(education);
        profile.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Education?> UpdateEducationAsync(Guid profileId, Guid educationId, Education education)
    {
        var profile = await _context
            .UserProfiles.Include(up => up.Educations)
            .FirstOrDefaultAsync(up => up.Id == profileId);

        if (profile == null)
            return null;

        var existing = profile.Educations.FirstOrDefault(e => e.Id == educationId);
        if (existing == null)
            return null;

        // Update the existing entity
        existing.Institution = education.Institution;
        existing.Degree = education.Degree;
        existing.FieldOfStudy = education.FieldOfStudy;
        existing.StartDate = education.StartDate;
        existing.EndDate = education.EndDate;
        existing.Grade = education.Grade;
        existing.CertificateNumber = education.CertificateNumber;
        existing.IsVerified = education.IsVerified;

        profile.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return existing;
    }

    // ========== Bank Accounts ==========

    public async Task<BankAccount?> AddBankAccountAsync(Guid profileId, BankAccount bankAccount)
    {
        var profile = await _context
            .UserProfiles.Include(up => up.BankAccounts)
            .FirstOrDefaultAsync(up => up.Id == profileId);

        if (profile == null)
            return null;

        bankAccount.UserProfileId = profileId;
        profile.BankAccounts.Add(bankAccount);
        profile.UpdatedAt = DateTime.UtcNow;

        await _context.AddAsync(bankAccount);
        await _context.SaveChangesAsync();
        return bankAccount;
    }

    public async Task<bool> RemoveBankAccountAsync(Guid profileId, Guid bankAccountId)
    {
        var profile = await _context
            .UserProfiles.Include(up => up.BankAccounts)
            .FirstOrDefaultAsync(up => up.Id == profileId);

        if (profile == null)
            return false;

        var bankAccount = profile.BankAccounts.FirstOrDefault(ba => ba.Id == bankAccountId);
        if (bankAccount == null)
            return false;

        profile.BankAccounts.Remove(bankAccount);
        profile.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<BankAccount?> UpdateBankAccountAsync(Guid profileId, Guid bankAccountId, BankAccount bankAccount)
    {
        var profile = await _context
            .UserProfiles.Include(up => up.BankAccounts)
            .FirstOrDefaultAsync(up => up.Id == profileId);

        if (profile == null)
            return null;

        var existing = profile.BankAccounts.FirstOrDefault(ba => ba.Id == bankAccountId);
        if (existing == null)
            return null;

        // Update the existing entity
        existing.BankName = bankAccount.BankName;
        existing.AccountType = bankAccount.AccountType;
        existing.AccountHolderName = bankAccount.AccountHolderName;
        existing.AccountNumber = bankAccount.AccountNumber;
        existing.BranchCode = bankAccount.BranchCode;
        existing.IsVerified = bankAccount.IsVerified;

        profile.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return existing;
    }
}