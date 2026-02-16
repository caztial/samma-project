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
        existing.ContactNumber = emergencyContact.ContactNumber;
        existing.Relationship = emergencyContact.Relationship;
        existing.Email = emergencyContact.Email;

        profile.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return existing;
    }

    // ========== Addresses ==========

    public async Task<Address?> AddAddressAsync(Guid profileId, Address address)
    {
        var profile = await _context
            .UserProfiles.Include(up => up.Addresses)
            .FirstOrDefaultAsync(up => up.Id == profileId);

        if (profile == null)
            return null;

        address.UserProfileId = profileId;
        profile.Addresses.Add(address);
        profile.UpdatedAt = DateTime.UtcNow;

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

    public async Task<Address?> UpdateAddressAsync(
        Guid profileId,
        Guid addressId,
        Address address
    )
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
        existing.Line1 = address.Line1;
        existing.Line2 = address.Line2;
        existing.Suburb = address.Suburb;
        existing.StateProvince = address.StateProvince;
        existing.Country = address.Country;
        existing.Postcode = address.Postcode;

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
        existing.CIN = identification.CIN;
        existing.PassportNumber = identification.PassportNumber;

        profile.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return existing;
    }

    // ========== Consents ==========

    public async Task<Consent?> AddConsentAsync(Guid profileId, Consent consent)
    {
        var profile = await _context
            .UserProfiles.Include(up => up.Consents)
            .FirstOrDefaultAsync(up => up.Id == profileId);

        if (profile == null)
            return null;

        consent.UserProfileId = profileId;
        profile.Consents.Add(consent);
        profile.UpdatedAt = DateTime.UtcNow;

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

    public async Task<Consent?> UpdateConsentAsync(
        Guid profileId,
        Guid consentId,
        Consent consent
    )
    {
        var profile = await _context
            .UserProfiles.Include(up => up.Consents)
            .FirstOrDefaultAsync(up => up.Id == profileId);

        if (profile == null)
            return null;

        var existing = profile.Consents.FirstOrDefault(c => c.Id == consentId);
        if (existing == null)
            return null;

        // Update the existing entity (only updatable fields)
        existing.TermId = consent.TermId;
        existing.TermLink = consent.TermLink;
        existing.TermsVersion = consent.TermsVersion;
        // Note: AcceptedAt and IpAddress are typically not updated after creation

        profile.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return existing;
    }
}
