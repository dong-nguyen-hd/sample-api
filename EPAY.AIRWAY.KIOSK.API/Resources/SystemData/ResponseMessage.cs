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
    _0001,
    _0002,
    _0003,
    _0004,
    _0005,
    _0006,
    _0007,
    _0008,
    _0009,
    
    _3001,
    _3002,
    _3003,
    _3004,
    _3005,
    _3006,
    
    _4002,
    _4003,
    
    _5001,
    _5002,
    _5003,
    
    _6001,
    _6002,
    
    _7001,
    _7002,
    _7003,
    _7004,
    
    _8001,
    
    _9001,
    _9002,
    _9003,
    
    _10001,
    _10002,
    _10003,
}