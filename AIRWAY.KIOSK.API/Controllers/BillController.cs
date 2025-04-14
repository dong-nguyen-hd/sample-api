using AIRWAY.KIOSK.API.Controllers.Config;
using AIRWAY.KIOSK.API.Domain.Services;
using AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.IACV.Request;
using AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.IACV.Response;
using FluentValidation;
using Microsoft.AspNetCore.Http.Timeouts;

namespace AIRWAY.KIOSK.API.Controllers;

[Route("api/v1/bill")]
[ApiController]
[Authorize]
public sealed class BillController(IIntegratedIAcvService integratedIAcvService) : ParentController
{
    #region Action

    [Authorize(Policy = MyPolicy.ThirdPartyIntegration)]
    [HttpPost("get-by-partner-key")]
    [RequestTimeout(CustomTimeoutProfile.Over15S)]
    [ResponseCache(CacheProfileName = CustomCacheProfile.NoCache)]
    [ProducesResponseType(typeof(PaginationResult<QueryDataResponse>), 200)]
    [SwaggerOperation(summary: "Lấy ra thông tin danh sách đơn hàng dựa vào partner-key")]
    public async Task<IActionResult> GetByPartnerKeyAsync([FromBody] QueryDataRequest request,
        [FromServices] IValidator<QueryDataRequest> validator,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var result = await integratedIAcvService.GetByPartnerKeyAsync(request, cancellationToken);
        return GetBaseResult(200, result);
    }

    #endregion
}