using EPAY.AIRWAY.KIOSK.API.Domain.Context;
using EPAY.AIRWAY.KIOSK.API.Domain.Services;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Device.Request;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Device.Response;
using Microsoft.EntityFrameworkCore;

namespace EPAY.AIRWAY.KIOSK.API.Services;

public sealed class DeviceService(IMapper mapper, CoreContext context) : BaseService, IDeviceService
{
    public async Task<BaseResult<DeviceResponse>> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<BaseResult<DeviceResponse>> CreateAsync(CreateRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<BaseResult<DeviceResponse>> UpdateAsync(string id, UpdateRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}