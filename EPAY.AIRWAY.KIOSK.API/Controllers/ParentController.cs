namespace EPAY.AIRWAY.KIOSK.API.Controllers;

public abstract class ParentController : ControllerBase
{
    protected virtual BaseResult<Inner> GetBaseResult<Inner>(CodeMessage codeMessage, Inner? data = default, string message = "")
    {
        return new BaseResult<Inner>
        {
            Data = data,
            CodeMessage = codeMessage,
            Message = message
        };
    }

    protected virtual IActionResult GetBaseResult<T>(int httpCode, T? data)
    {
        HttpContext.RequestAborted.ThrowIfCancellationRequested();
        return StatusCode(httpCode, data);
    }
}