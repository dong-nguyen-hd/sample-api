using EPAY.AIRWAY.KIOSK.API.Controllers.Config;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Administrative.Response;

namespace EPAY.AIRWAY.KIOSK.API.Controllers;

[Route("api/v1/administrative")]
[ApiController]
public sealed class AdministrativeController : ParentController
{
    #region Action

    [Authorize(Policy = MyPolicy.Device)]
    [HttpGet("get-province")]
    [ResponseCache(CacheProfileName = CustomCacheProfile.Any5m)]
    [ProducesResponseType(typeof(BaseResult<List<ProvinceInnerResponse>>), 200)]
    [SwaggerOperation(summary: "Lấy thông tin tỉnh/thành phố")]
    public IActionResult GetProvinceAsync()
    {
        var result = GetBaseResult(CodeMessage._0000, data: ProvinceResponse.Provinces);

        return Ok(result);
    }

    #endregion
}