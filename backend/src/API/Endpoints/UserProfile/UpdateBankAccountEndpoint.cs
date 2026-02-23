using API.DTOs.UserProfile;
using Core.Authorization;
using Core.Entities.UserProfiles;
using Core.Enums;
using Core.Services;
using FastEndpoints;

namespace API.Endpoints.UserProfile;

/// <summary>
/// Endpoint to update a bank account in a profile.
/// </summary>
public class UpdateBankAccountEndpoint : Endpoint<UpdateBankAccountRequest, BankAccountResponse>
{
    private readonly IUserProfileService _userProfileService;

    public UpdateBankAccountEndpoint(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    public override void Configure()
    {
        Put("/profile/{id}/bank-accounts/{bankAccountId}");
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
            s.Summary = "Update bank account";
            s.Description =
                "Updates a bank account in a profile. Owner, Admin, or Moderator can update.";
        });
    }

    public override async Task HandleAsync(UpdateBankAccountRequest req, CancellationToken ct)
    {
        var profileId = Route<Guid>("id");
        var bankAccountId = Route<Guid>("bankAccountId");

        var profile = await _userProfileService.GetByIdAsync(profileId);

        if (profile == null)
        {
            await HttpContext.Response.SendAsync(new { error = "Profile not found" }, 404);
            return;
        }

        var bankAccount = new BankAccount
        {
            BankName = req.BankAccount.BankName,
            AccountType = req.BankAccount.AccountType,
            AccountHolderName = req.BankAccount.AccountHolderName,
            AccountNumber = req.BankAccount.AccountNumber,
            BranchCode = req.BankAccount.BranchCode,
            IsVerified = req.BankAccount.IsVerified
        };

        var updated = await _userProfileService.UpdateBankAccountAsync(
            profileId,
            bankAccountId,
            bankAccount
        );

        if (updated == null)
        {
            await HttpContext.Response.SendAsync(new { error = "Bank account not found" }, 404);
            return;
        }

        var response = new BankAccountResponse
        {
            Id = updated.Id,
            BankName = updated.BankName,
            AccountType = updated.AccountType,
            AccountHolderName = updated.AccountHolderName,
            AccountNumber = updated.AccountNumber,
            BranchCode = updated.BranchCode,
            IsVerified = updated.IsVerified
        };

        await HttpContext.Response.SendAsync(response, 200, cancellation: ct);
    }
}
