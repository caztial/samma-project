using API.DTOs.UserProfile;
using Core.Entities.UserProfiles;
using Core.Entities.ValueObjects;
using Core.Enums;
using FastEndpoints;

namespace API.Mappers;

/// <summary>
/// Mapper for UserProfile entity - handles all profile-related mappings.
/// </summary>
public class ProfileMapper : Mapper<UpdateProfileRequest, ProfileResponse, UserProfile>
{
    public override UserProfile ToEntity(UpdateProfileRequest r)
    {
        // Parse Gender string to enum
        Gender? gender = null;
        if (!string.IsNullOrEmpty(r.Gender))
        {
            gender = Enum.Parse<Gender>(r.Gender, ignoreCase: true);
        }

        // Parse DateOfBirth string to DateOnly
        DateOnly? dateOfBirth = null;
        if (!string.IsNullOrEmpty(r.DateOfBirth))
        {
            dateOfBirth = DateOnly.Parse(r.DateOfBirth);
        }

        var profile = new UserProfile
        {
            FirstName = r.FirstName ?? string.Empty,
            LastName = r.LastName ?? string.Empty,
            ProfileImageUrl = r.ProfileImageUrl,
            Gender = gender,
            DateOfBirth = dateOfBirth
        };

        if (!string.IsNullOrEmpty(r.ContactNumber) || !string.IsNullOrEmpty(r.Email))
        {
            profile.Contact = new Contact(
                r.ContactNumber ?? string.Empty,
                r.Email ?? string.Empty
            );
        }

        return profile;
    }

    public override ProfileResponse FromEntity(UserProfile e) =>
        new()
        {
            Id = e.Id,
            UserId = e.UserId,
            FirstName = e.FirstName,
            LastName = e.LastName,
            ProfileImageUrl = e.ProfileImageUrl,
            Gender = e.Gender,
            DateOfBirth = e.DateOfBirth,
            Contact = new ContactDto
            {
                ContactNumber = e.Contact.ContactNumber,
                Email = e.Contact.Email
            },
            Biometrics = new BiometricsDto
            {
                FingerPrint = e.Biometrics.FingerPrint,
                Face = e.Biometrics.Face
            },
            // Map collections
            EmergencyContacts =
            [
                .. e.EmergencyContacts.Select(ec => new EmergencyContactResponse
                {
                    Id = ec.Id,
                    Name = ec.Name,
                    ContactNumber = ec.Contact.ContactNumber,
                    Relationship = ec.Relationship,
                    Email = ec.Contact.Email
                })
            ],
            Addresses =
            [
                .. e.Addresses.Select(a => new AddressResponse
                {
                    Id = a.Id,
                    Type = a.Type,
                    Line1 = a.Address.Line1,
                    Line2 = a.Address.Line2,
                    Suburb = a.Address.Suburb,
                    StateProvince = a.Address.StateProvince,
                    Country = a.Address.Country,
                    Postcode = a.Address.Postcode
                })
            ],
            Identifications =
            [
                .. e.Identifications.Select(i => new IdentificationResponse
                {
                    Id = i.Id,
                    Type = i.Type,
                    Value = i.Value
                })
            ],
            Consents =
            [
                .. e.Consents.Select(c => new ConsentResponse
                {
                    Id = c.Id,
                    TermId = c.Consent.TermId,
                    TermLink = c.Consent.TermLink,
                    TermsVersion = c.Consent.TermsVersion,
                    AcceptedAt = c.Consent.AcceptedAt,
                    IpAddress = c.Consent.IpAddress
                })
            ]
        };
}

/// <summary>
/// Mapper for EmergencyContact entity.
/// </summary>
public class EmergencyContactMapper
    : Mapper<EmergencyContactRequest, EmergencyContactResponse, EmergencyContact>
{
    public override EmergencyContact ToEntity(EmergencyContactRequest r) =>
        new()
        {
            Name = r.Name,
            Relationship = r.Relationship,
            Contact = new Contact(r.ContactNumber, r.Email)
        };

    public override EmergencyContactResponse FromEntity(EmergencyContact e) =>
        new()
        {
            Id = e.Id,
            Name = e.Name,
            ContactNumber = e.Contact.ContactNumber,
            Relationship = e.Relationship,
            Email = e.Contact.Email
        };
}

/// <summary>
/// Mapper for UserAddress entity.
/// </summary>
public class AddressMapper : Mapper<AddressRequest, AddressResponse, UserAddress>
{
    public override UserAddress ToEntity(AddressRequest r) =>
        new()
        {
            Address = new Address(r.Line1, r.Suburb, r.StateProvince, r.Country, r.Postcode, r.Line2)
        };

    public override AddressResponse FromEntity(UserAddress e) =>
        new()
        {
            Id = e.Id,
            Type = e.Type,
            Line1 = e.Address.Line1,
            Line2 = e.Address.Line2,
            Suburb = e.Address.Suburb,
            StateProvince = e.Address.StateProvince,
            Country = e.Address.Country,
            Postcode = e.Address.Postcode
        };
}

/// <summary>
/// Mapper for Identification entity.
/// </summary>
public class IdentificationMapper
    : Mapper<IdentificationRequest, IdentificationResponse, Identification>
{
    public override Identification ToEntity(IdentificationRequest r) => new() { Type = r.Type, Value = r.Value };

    public override IdentificationResponse FromEntity(Identification e) =>
        new()
        {
            Id = e.Id,
            Type = e.Type,
            Value = e.Value
        };
}

/// <summary>
/// Mapper for UserConsent entity.
/// </summary>
public class ConsentMapper : Mapper<ConsentRequest, ConsentResponse, UserConsent>
{
    public override UserConsent ToEntity(ConsentRequest r)
    {
        // IP address will be set by the endpoint
        return new()
        {
            Consent = new Consent(r.TermId, r.TermLink, r.TermsVersion, string.Empty)
        };
    }

    public override ConsentResponse FromEntity(UserConsent e) =>
        new()
        {
            Id = e.Id,
            TermId = e.Consent.TermId,
            TermLink = e.Consent.TermLink,
            TermsVersion = e.Consent.TermsVersion,
            AcceptedAt = e.Consent.AcceptedAt,
            IpAddress = e.Consent.IpAddress
        };
}

/// <summary>
/// Mapper for Biometrics entity.
/// </summary>
public class BiometricsMapper : Mapper<BiometricsRequest, BiometricsResponse, Biometrics>
{
    public override Biometrics ToEntity(BiometricsRequest r) => new(r.FingerPrint, r.Face);

    public override BiometricsResponse FromEntity(Biometrics e) =>
        new() { FingerPrint = e.FingerPrint, Face = e.Face };
}