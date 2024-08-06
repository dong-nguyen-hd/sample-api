using EPAY.AIRWAY.KIOSK.API.Domain.Services;

namespace EPAY.AIRWAY.KIOSK.API.Services;

public abstract class BaseService : IBaseService
{
    #region Method

    public virtual BaseResult<Inner> GetBaseResult<Inner>(CodeMessage codeMessage, Inner? data = default, string message = "") =>
        new()
        {
            Data = data,
            CodeMessage = codeMessage,
            Message = message
        };

    #endregion
}