using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EPAY.AIRWAY.KIOSK.API.Controllers;

using AutoMapper;
using Microsoft.AspNetCore.Mvc;

public abstract class ParentController : ControllerBase
{
    #region Method

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

    #endregion
}