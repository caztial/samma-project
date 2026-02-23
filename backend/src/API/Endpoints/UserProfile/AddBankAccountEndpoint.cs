using API.DTOs.UserProfile;
using API.Mappers;
using Core.Authorization;
using Core.Entities.UserProfiles;
using Core.Enums;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.UserProfile;

/// <summary>
/// Endpoint to add a bank account to a profile.
/// </summary>
public class AddBankAccountEndpoint
    : Endpoint<AddBankAccountRequest, BankAccountResponse, BankAccountMapper>
{
    private readonly IUserProfileService _userProfileService;

    public AddBankAccountEndpoint(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    public override void Configure()
    {
        Post("/profile/{id}/bank-accounts");
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
            s.Summary = "Add bank account";
            s.Description = "Adds a bank account to a profile. Owner, Admin, or Moderator can add.";
        });
    }

    public override async Task HandleAsync(AddBankAccountRequest req, CancellationToken ct)
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

        var bankAccount = Map.ToEntity(req.BankAccount);

        var added = await _userProfileService.AddBankAccountAsync(id, bankAccount);

        if (added == null)
        {
            await HttpContext.Response.SendAsync(
                new { error = "Failed to add bank account" },
                500,
                cancellation: ct
            );
            return;
        }

        Response = Map.FromEntity(added);
        await HttpContext.Response.SendAsync(Response, 201, cancellation: ct);
    }
}
