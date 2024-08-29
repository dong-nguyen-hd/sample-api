using EPAY.AIRWAY.KIOSK.API.Controllers.Config;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Administrative.Response;
using Microsoft.AspNetCore.Http.Timeouts;

namespace EPAY.AIRWAY.KIOSK.API.Controllers;

[Route("api/v1/administrative")]
[ApiController]
public sealed class AdministrativeController : ParentController
{
    #region Action

    [Authorize(Policy = MyPolicy.Device)]
    [HttpGet("get-province")]
    [RequestTimeout(CustomTimeoutProfile.Over15S)]
    [ResponseCache(CacheProfileName = CustomCacheProfile.Any5M)]
    [ProducesResponseType(typeof(BaseResult<List<ProvinceInnerResponse>>), 200)]
    [SwaggerOperation(summary: "Lấy thông tin tỉnh/thành phố")]
    public IActionResult GetProvinceAsync()
    {
        var result = GetBaseResult(CodeMessage._0000, data: ProvinceResponse.Provinces);

        return GetBaseResult(200, result);
    }

    #endregion
}