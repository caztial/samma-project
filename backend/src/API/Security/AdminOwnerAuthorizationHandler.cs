using Core.Authorization;
using Core.Enums;
using Core.Services;
using Microsoft.AspNetCore.Authorization;

namespace API.Security;

/// <summary>
/// Authorization handler for AdminOwnerRequirement.
/// Uses IResourceOwnerAuthorization to check ownership based on the AggregatedRootName.
/// </summary>
public class AdminOwnerAuthorizationHandler : AuthorizationHandler<AdminOwnerRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AdminOwnerAuthorizationHandler> _logger;

    public AdminOwnerAuthorizationHandler(
        IHttpContextAccessor httpContextAccessor,
        IServiceProvider serviceProvider,
        ILogger<AdminOwnerAuthorizationHandler> logger
    )
    {
        _httpContextAccessor = httpContextAccessor;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AdminOwnerRequirement requirement
    )
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            _logger.LogWarning("HttpContext is null in AdminOwnerAuthorizationHandler");
            context.Fail();
            return;
        }

        // Check if user is authenticated
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            _logger.LogWarning(
                "Unauthenticated user attempted to access resource requiring AdminOwnerAuthorization"
            );
            context.Fail();
            return;
        }

        // Get the resource ID based on ValueFetchFrom
        var resourceId = GetResourceId(httpContext, requirement);
        if (!resourceId.HasValue)
        {
            _logger.LogWarning(
                "Resource ID not found for parameter: {ParameterName} from {Source}",
                requirement.ResourceIdParameterName,
                requirement.ValueFetchFrom
            );
            context.Fail();
            return;
        }

        // Get the appropriate IResourceOwnerAuthorization using keyed service
        using var scope = _serviceProvider.CreateScope();

        // Use GetKeyedService to resolve by the AggregatedRootName key
        var authorization = scope.ServiceProvider.GetKeyedService<IResourceOwnerAuthorization>(
            requirement.AggregatedRootName
        );
        if (authorization == null)
        {
            _logger.LogError(
                "No authorization handler found for AggregatedRootName: {AggregatedRootName}",
                requirement.AggregatedRootName
            );
            context.Fail();
            return;
        }

        // Check ownership
        var claims = context.User;
        var isAuthorized = await authorization.IsOwnerAsync(resourceId.Value, claims);

        if (isAuthorized)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
    }

    private static Guid? GetResourceId(HttpContext httpContext, AdminOwnerRequirement requirement)
    {
        return requirement.ValueFetchFrom switch
        {
            ValueFetchFrom.Route => GetFromRoute(httpContext, requirement.ResourceIdParameterName),
            ValueFetchFrom.Query => GetFromQuery(httpContext, requirement.ResourceIdParameterName),
            ValueFetchFrom.Header
                => GetFromHeader(httpContext, requirement.ResourceIdParameterName),
            ValueFetchFrom.Body => GetFromBody(httpContext, requirement.ResourceIdParameterName),
            _ => null
        };
    }

    private static Guid? GetFromRoute(HttpContext httpContext, string parameterName)
    {
        if (httpContext.Request.RouteValues.TryGetValue(parameterName, out var value))
        {
            if (value is Guid guidValue)
            {
                return guidValue;
            }

            if (Guid.TryParse(value?.ToString(), out var parsedGuid))
            {
                return parsedGuid;
            }
        }

        return null;
    }

    private static Guid? GetFromQuery(HttpContext httpContext, string parameterName)
    {
        if (httpContext.Request.Query.TryGetValue(parameterName, out var queryValue))
        {
            if (Guid.TryParse(queryValue, out var parsedGuid))
            {
                return parsedGuid;
            }
        }

        return null;
    }

    private static Guid? GetFromHeader(HttpContext httpContext, string headerName)
    {
        if (httpContext.Request.Headers.TryGetValue(headerName, out var headerValue))
        {
            if (Guid.TryParse(headerValue, out var parsedGuid))
            {
                return parsedGuid;
            }
        }

        return null;
    }

    private static Guid? GetFromBody(HttpContext httpContext, string propertyName)
    {
        // Note: Body reading requires the request body to be available
        // This is a simplified implementation - in production you'd want to use model binding
        // For now, we'll return null as reading body requires additional setup
        _ = httpContext; // Suppress unused warning
        _ = propertyName;

        // TODO: Implement body reading if needed
        // This would require enabling request body buffering or using a middleware
        return null;
    }
}
