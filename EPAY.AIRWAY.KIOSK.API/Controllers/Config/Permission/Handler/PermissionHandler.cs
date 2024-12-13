namespace EPAY.AIRWAY.KIOSK.API.Controllers.Config.Permission.Handler;

using Requirement;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (IsValid(context.User, requirement.Permissions))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }

    #region Private work

    private static bool IsValid(ClaimsPrincipal user, List<string> permissions)
    {
        List<Claim> roleClaims = user.FindAll(ClaimTypes.Role).ToList();

        if (roleClaims.Count == 0)
            return false;

        foreach (var role in roleClaims)
            if (!string.IsNullOrEmpty(role.Value) && permissions.Contains(role.Value))
                return true;

        return false;
    }

    #endregion
}