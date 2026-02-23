using API.DTOs.UserProfile;
using Core.Authorization;
using Core.Enums;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.UserProfile;

/// <summary>
/// Endpoint to get all bank accounts for a profile.
/// </summary>
public class GetBankAccountsEndpoint : EndpointWithoutRequest
{
    private readonly IUserProfileService _userProfileService;

    public GetBankAccountsEndpoint(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    public override void Configure()
    {
        Get("/profile/{id}/bank-accounts");
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
            s.Summary = "Get bank accounts";
            s.Description =
                "Retrieves all bank accounts for a profile. Owner, Admin, or Moderator can access.";
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

        var bankAccounts = await _userProfileService.GetBankAccountsAsync(id);

        var response = bankAccounts.Select(ba => new BankAccountResponse
        {
            Id = ba.Id,
            BankName = ba.BankName,
            AccountType = ba.AccountType,
            AccountHolderName = ba.AccountHolderName,
            AccountNumber = ba.AccountNumber,
            BranchCode = ba.BranchCode,
            IsVerified = ba.IsVerified
        });

        await HttpContext.Response.SendAsync(response, 200, cancellation: ct);
    }
}
