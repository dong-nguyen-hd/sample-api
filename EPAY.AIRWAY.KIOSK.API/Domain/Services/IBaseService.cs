namespace EPAY.AIRWAY.KIOSK.API.Domain.Services;

using AutoMapper;
using Context;

public interface IBaseService
{
    #region Properties
    IMapper Mapper { get; init; }
    CoreContext Context { get; init; }
    #endregion

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
