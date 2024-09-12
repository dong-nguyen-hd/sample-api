using System.Net;
using EPAY.AIRWAY.KIOSK.API.Resources.Exceptions;
using FluentValidation;

namespace EPAY.AIRWAY.KIOSK.API.Controllers.Middlewares;

public sealed class ErrorHandlerMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception error)
        {
            var response = context.Response;
            response.ContentType = MimeType.JSON;
            BaseResult<object> result;

            // Using switch for custom exception
            switch (error)
            {
                // Add custom exception code below!
                case TaskCanceledException ex1:
                case OperationCanceledException ex2:
                    response.StatusCode = (int)HttpStatusCode.GatewayTimeout;
                    result = new(CodeMessage._3006);
                    break;
                case ValidationException:
                case BadRequestException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    result = new(CodeMessage._3001);
                    break;
                default:
                    // unhandled error
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    result = new(CodeMessage._3005);
                    break;
            }

            await response.WriteAsync(result.MySerialize());

            throw;
        }
    }
}