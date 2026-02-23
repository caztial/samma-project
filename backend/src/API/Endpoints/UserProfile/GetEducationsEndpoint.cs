using API.DTOs.UserProfile;
using Core.Authorization;
using Core.Entities.UserProfiles;
using Core.Enums;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.UserProfile;

/// <summary>
/// Endpoint to get all educations for a profile.
/// </summary>
public class GetEducationsEndpoint : EndpointWithoutRequest
{
    private readonly IUserProfileService _userProfileService;

    public GetEducationsEndpoint(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    public override void Configure()
    {
        Get("/profile/{id}/educations");
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
            s.Summary = "Get educations";
            s.Description =
                "Retrieves all educations for a profile. Owner, Admin, or Moderator can access.";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var profile = await _userProfileService.GetByIdAsync(id);

        if (profile == null)
        {
            await HttpContext.Response.SendAsync(
                new { error = "Profile not found" },
                404,
                cancellation: ct
            );
            return;
        }

        var educations = await _userProfileService.GetEducationsAsync(id);

        var response = educations.Select(e => new EducationResponse
        {
            Id = e.Id,
            Institution = e.Institution,
            Degree = e.Degree,
            FieldOfStudy = e.FieldOfStudy,
            StartDate = e.StartDate,
            EndDate = e.EndDate,
            Grade = e.Grade,
            CertificateNumber = e.CertificateNumber,
            IsVerified = e.IsVerified
        });

        await HttpContext.Response.SendAsync(response, 200, cancellation: ct);
    }
}
