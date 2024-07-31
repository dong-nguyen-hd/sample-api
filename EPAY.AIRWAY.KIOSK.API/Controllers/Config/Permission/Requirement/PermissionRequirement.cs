namespace EPAY.AIRWAY.KIOSK.API.Controllers.Config.Permission.Requirement;

using Microsoft.AspNetCore.Authorization;

public sealed class PermissionRequirement : IAuthorizationRequirement
{
    #region Properties
    public List<string> Permissions { get; init; }
    #endregion

    #region Constructor
    public PermissionRequirement(List<string> permissions) =>
        Permissions = permissions;
    #endregion
}
