using API.DTOs.UserProfile;
using API.Mappers;
using Core.Authorization;
using Core.Entities.UserProfiles;
using Core.Enums;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.UserProfile;

/// <summary>
/// Endpoint to add an education to a profile.
/// </summary>
public class AddEducationEndpoint
    : Endpoint<AddEducationRequest, EducationResponse, EducationMapper>
{
    private readonly IUserProfileService _userProfileService;

    public AddEducationEndpoint(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    public override void Configure()
    {
        Post("/profile/{id}/educations");
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
            s.Summary = "Add education";
            s.Description = "Adds an education to a profile. Owner, Admin, or Moderator can add.";
        });
    }

    public override async Task HandleAsync(AddEducationRequest req, CancellationToken ct)
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

        var education = Map.ToEntity(req.Education);

        var added = await _userProfileService.AddEducationAsync(id, education);

        if (added == null)
        {
            await HttpContext.Response.SendAsync(
                new { error = "Failed to add education" },
                500,
                cancellation: ct
            );
            return;
        }

        Response = Map.FromEntity(added);
        await HttpContext.Response.SendAsync(Response, 201, cancellation: ct);
    }
}
