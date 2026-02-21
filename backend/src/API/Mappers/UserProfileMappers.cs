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
                    ContactNumber = ec.ContactNumber,
                    Relationship = ec.Relationship,
                    Email = ec.Email
                })
            ],
            Addresses =
            [
                .. e.Addresses.Select(a => new AddressResponse
                {
                    Id = a.Id,
                    Line1 = a.Line1,
                    Line2 = a.Line2,
                    Suburb = a.Suburb,
                    StateProvince = a.StateProvince,
                    Country = a.Country,
                    Postcode = a.Postcode
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
                    TermId = c.TermId,
                    TermLink = c.TermLink,
                    TermsVersion = c.TermsVersion,
                    AcceptedAt = c.AcceptedAt,
                    IpAddress = c.IpAddress
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
        new(r.Name, r.ContactNumber, r.Relationship, r.Email);

    public override EmergencyContactResponse FromEntity(EmergencyContact e) =>
        new()
        {
            Id = e.Id,
            Name = e.Name,
            ContactNumber = e.ContactNumber,
            Relationship = e.Relationship,
            Email = e.Email
        };
}

/// <summary>
/// Mapper for Address entity.
/// </summary>
public class AddressMapper : Mapper<AddressRequest, AddressResponse, Address>
{
    public override Address ToEntity(AddressRequest r) =>
        new(r.Line1, r.Suburb, r.StateProvince, r.Country, r.Postcode, r.Line2);

    public override AddressResponse FromEntity(Address e) =>
        new()
        {
            Id = e.Id,
            Line1 = e.Line1,
            Line2 = e.Line2,
            Suburb = e.Suburb,
            StateProvince = e.StateProvince,
            Country = e.Country,
            Postcode = e.Postcode
        };
}

/// <summary>
/// Mapper for Identification entity.
/// </summary>
public class IdentificationMapper
    : Mapper<IdentificationRequest, IdentificationResponse, Identification>
{
    public override Identification ToEntity(IdentificationRequest r) =>
        new(r.Type, r.Value);

    public override IdentificationResponse FromEntity(Identification e) =>
        new()
        {
            Id = e.Id,
            Type = e.Type,
            Value = e.Value
        };
}

/// <summary>
/// Mapper for Consent entity.
/// </summary>
public class ConsentMapper : Mapper<ConsentRequest, ConsentResponse, Consent>
{
    public override Consent ToEntity(ConsentRequest r)
    {
        // IP address will be set by the endpoint
        return new Consent(r.TermId, r.TermLink, r.TermsVersion, string.Empty);
    }

    public override ConsentResponse FromEntity(Consent e) =>
        new()
        {
            Id = e.Id,
            TermId = e.TermId,
            TermLink = e.TermLink,
            TermsVersion = e.TermsVersion,
            AcceptedAt = e.AcceptedAt,
            IpAddress = e.IpAddress
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
