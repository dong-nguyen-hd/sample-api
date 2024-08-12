namespace EPAY.AIRWAY.KIOSK.API.Controllers.Config;

using Microsoft.AspNetCore.Mvc;

public static class CustomCacheProfile
{
    #region Profiles
    public const string NoCache = "NoCache";
    public const string Any60s = "Any60s";
    public const string Any5m = "Any5m";
    #endregion

    #region Method
    public static void ApplyProfile(this MvcOptions opt)
    {
        opt.CacheProfiles.Add(NoCache, new CacheProfile() { NoStore = true });
        opt.CacheProfiles.Add(Any60s, new CacheProfile()
        {
            Location = ResponseCacheLocation.Any,
            Duration = 60
        });
        opt.CacheProfiles.Add(Any5m, new CacheProfile()
        {
            Location = ResponseCacheLocation.Any,
            Duration = 5*60
        });
    }
    #endregion
}
