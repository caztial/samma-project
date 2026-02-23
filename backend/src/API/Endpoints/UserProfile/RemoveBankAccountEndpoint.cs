using API.DTOs.UserProfile;
using Core.Authorization;
using Core.Enums;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.UserProfile;

/// <summary>
/// Endpoint to remove a bank account from a profile.
/// </summary>
public class RemoveBankAccountEndpoint : Endpoint<RemoveBankAccountRequest>
{
    private readonly IUserProfileService _userProfileService;

    public RemoveBankAccountEndpoint(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    public override void Configure()
    {
        Delete("/profile/{id}/bank-accounts/{bankAccountId}");
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
            s.Summary = "Remove bank account";
            s.Description =
                "Removes a bank account from a profile. Owner, Admin, or Moderator can remove.";
        });
    }

    public override async Task HandleAsync(RemoveBankAccountRequest req, CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var bankAccountId = Route<Guid>("bankAccountId");

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

        var removed = await _userProfileService.RemoveBankAccountAsync(id, bankAccountId);

        if (!removed)
        {
            await HttpContext.Response.SendAsync(new { error = "Bank account not found" }, 404);
            return;
        }

        await HttpContext.Response.SendAsync(
            new { message = "Bank account removed successfully" },
            200,
            cancellation: ct
        );
    }
}
