namespace EPAY.AIRWAY.KIOSK.API.Domain.Services;

public interface IBaseService
{
    #region Method
    /// <summary>
    /// Chức năng: trả về kết quả theo format chung của service-layer
    /// </summary>
    /// <typeparam name="Inner"></typeparam>
    /// <param name="codeMessage"></param>
    /// <param name="data"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    BaseResult<Inner> GetBaseResult<Inner>(CodeMessage codeMessage, Inner? data = default, string message = "");
    #endregion
}
