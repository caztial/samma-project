using API.DTOs.UserProfile;
using Core.Authorization;
using Core.Entities.UserProfiles;
using Core.Enums;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.UserProfile;

/// <summary>
/// Endpoint to update an education in a profile.
/// </summary>
public class UpdateEducationEndpoint : Endpoint<UpdateEducationRequest, EducationResponse>
{
    private readonly IUserProfileService _userProfileService;

    public UpdateEducationEndpoint(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    public override void Configure()
    {
        Put("/profile/{id}/educations/{educationId}");
        Policy(policy =>
        {
            policy.AddRequirements(
                new AdminOwnerRequirement(
                    aggregatedRootName: nameof(UserProfile),
                    resourceIdParameterName: "id",
                    valueFetchFrom: ValueFetchFrom.Route
                )
            );
        });
        Summary(s =>
        {
            s.Summary = "Update education";
            s.Description =
                "Updates an education in a profile. Owner, Admin, or Moderator can update.";
        });
    }

    public override async Task HandleAsync(UpdateEducationRequest req, CancellationToken ct)
    {
        var profileId = Route<Guid>("id");
        var educationId = Route<Guid>("educationId");

        var profile = await _userProfileService.GetByIdAsync(profileId);

        if (profile == null)
        {
            await HttpContext.Response.SendAsync(new { error = "Profile not found" }, 404);
            return;
        }

        var education = new Education
        {
            Institution = req.Education.Institution,
            Degree = req.Education.Degree,
            FieldOfStudy = req.Education.FieldOfStudy,
            StartDate = req.Education.StartDate,
            EndDate = req.Education.EndDate,
            Grade = req.Education.Grade,
            CertificateNumber = req.Education.CertificateNumber,
            IsVerified = req.Education.IsVerified
        };

        var updated = await _userProfileService.UpdateEducationAsync(
            profileId,
            educationId,
            education
        );

        if (updated == null)
        {
            await HttpContext.Response.SendAsync(new { error = "Education not found" }, 404);
            return;
        }

        var response = new EducationResponse
        {
            Id = updated.Id,
            Institution = updated.Institution,
            Degree = updated.Degree,
            FieldOfStudy = updated.FieldOfStudy,
            StartDate = updated.StartDate,
            EndDate = updated.EndDate,
            Grade = updated.Grade,
            CertificateNumber = updated.CertificateNumber,
            IsVerified = updated.IsVerified
        };

        await HttpContext.Response.SendAsync(response, 200, cancellation: ct);
    }
}
