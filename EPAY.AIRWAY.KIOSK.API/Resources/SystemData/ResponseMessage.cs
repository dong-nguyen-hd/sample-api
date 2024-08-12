namespace EPAY.AIRWAY.KIOSK.API.Resources.SystemData;

public sealed class ResponseMessage
{
    #region Property
    public static Dictionary<string, string> Values { get; private set; }
    #endregion
}

public enum CodeMessage
{
    _0000,
    _100,
    _101,
}