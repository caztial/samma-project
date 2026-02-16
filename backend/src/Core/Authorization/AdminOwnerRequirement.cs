using Core.Enums;
using Microsoft.AspNetCore.Authorization;

namespace Core.Authorization;

/// <summary>
/// Authorization requirement that checks if the user is the owner of a resource or has admin privileges.
/// </summary>
public class AdminOwnerRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// The name of the aggregated root (e.g., "UserProfile").
    /// Used as key to resolve the IResourceOwnerAuthorization implementation.
    /// </summary>
    public string AggregatedRootName { get; }

    /// <summary>
    /// The variable name of the resource ID (e.g., "id", "profileId").
    /// </summary>
    public string ResourceIdParameterName { get; }

    /// <summary>
    /// Indicates where to fetch the resource ID from (Route, Query, Header, Body).
    /// </summary>
    public ValueFetchFrom ValueFetchFrom { get; }

    /// <summary>
    /// Creates a new AdminOwnerRequirement.
    /// </summary>
    /// <param name="aggregatedRootName">The name of the aggregated root (e.g., "UserProfile").</param>
    /// <param name="resourceIdParameterName">The variable name of the resource ID (e.g., "id", "profileId").</param>
    /// <param name="valueFetchFrom">Where to fetch the resource ID from (Route, Query, Header, Body).</param>
    public AdminOwnerRequirement(
        string aggregatedRootName,
        string resourceIdParameterName,
        ValueFetchFrom valueFetchFrom
    )
    {
        AggregatedRootName = aggregatedRootName;
        ResourceIdParameterName = resourceIdParameterName;
        ValueFetchFrom = valueFetchFrom;
    }
}
