using API.DTOs.UserProfile;
using FastEndpoints;
using FluentValidation;

namespace API.Validators;

/// <summary>
/// Validator for UpdateProfileRequest.
/// </summary>
public class UpdateProfileRequestValidator : Validator<UpdateProfileRequest>
{
    public UpdateProfileRequestValidator()
    {
        // FirstName: Required, minimum 3 characters
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required")
            .MinimumLength(3)
            .WithMessage("First name must be at least 3 characters");

        // LastName: Optional (no validation required)

        // Gender: Required, must be a valid value
        RuleFor(x => x.Gender)
            .NotEmpty()
            .WithMessage("Gender is required")
            .Must(BeAValidGender)
            .WithMessage("Gender must be one of: Male, Female, Other, PreferNotToSay");

        // DateOfBirth: Required, must be a valid date format
        RuleFor(x => x.DateOfBirth)
            .NotEmpty()
            .WithMessage("Date of birth is required")
            .Must(BeAValidDate)
            .WithMessage("Date of birth must be a valid date in YYYY-MM-DD format");

        // Email: Optional, but must be valid email format if provided
        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("Invalid email format");
    }

    private bool BeAValidGender(string? gender)
    {
        if (string.IsNullOrEmpty(gender))
            return false;

        var validGenders = new[] { "Male", "Female", "Other", "PreferNotToSay" };
        return validGenders.Contains(gender, StringComparer.OrdinalIgnoreCase);
    }

    private bool BeAValidDate(string? dateStr)
    {
        if (string.IsNullOrEmpty(dateStr))
            return false;

        return DateOnly.TryParse(dateStr, out _);
    }
}
