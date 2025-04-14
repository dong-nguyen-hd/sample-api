using AIRWAY.KIOSK.API.Domain.Context;
using AIRWAY.KIOSK.API.Domain.Services;
using AIRWAY.KIOSK.API.Resources.DTOs.Device.Request;
using AIRWAY.KIOSK.API.Resources.DTOs.Device.Response;
using Microsoft.EntityFrameworkCore;

namespace AIRWAY.KIOSK.API.Services;

public sealed class DeviceService(IMapper mapper, CoreContext context) : BaseService, IDeviceService
{
    public async Task<BaseResult<DeviceResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var device = await context.Devices.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (device == null)
            return GetBaseResult<DeviceResponse>(CodeMessage._3001);

        return GetBaseResult(CodeMessage._0000, data: mapper.Map<DeviceResponse>(device));
    }

    public async Task<BaseResult<DeviceResponse>> CreateAsync(CreateRequest request, CancellationToken cancellationToken = default)
    {
        var device = mapper.Map<Model.ReportSection.Device>(request);
        await context.Devices.AddAsync(device, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return GetBaseResult(CodeMessage._0000, data: mapper.Map<DeviceResponse>(device));
    }

    public async Task<BaseResult<DeviceResponse>> UpdateAsync(Guid id, UpdateRequest request, CancellationToken cancellationToken = default)
    {
        var device = await context.Devices.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (device == null)
            return GetBaseResult<DeviceResponse>(CodeMessage._3001);

        mapper.Map(request, device);

        context.Devices.Update(device);
        await context.SaveChangesAsync(cancellationToken);

        return GetBaseResult(CodeMessage._0000, data: mapper.Map<DeviceResponse>(device));
    }
}