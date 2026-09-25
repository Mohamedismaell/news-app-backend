using System.Security.Claims;

namespace NewsBackend.API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal principal)
    {
        var value = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        return value is not null && int.TryParse(value, out var userId) ? userId : 0;
    }

    public static bool IsInRoles(this ClaimsPrincipal principal, params string[] roles)
    {
        foreach (var role in roles)
        {
            if (principal.IsInRole(role))
            {
                return true;
            }
        }

        return false;
    }
}