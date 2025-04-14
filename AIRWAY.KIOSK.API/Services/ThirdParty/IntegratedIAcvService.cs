using AIRWAY.KIOSK.API.Domain.Context;
using AIRWAY.KIOSK.API.Domain.Services;
using AIRWAY.KIOSK.API.Resources.DTOs.Flight.Response;
using AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.IACV.Application;
using AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.IACV.Mapping;
using AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.IACV.Request;
using AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.IACV.Response;
using AIRWAY.KIOSK.API.Resources.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace AIRWAY.KIOSK.API.Services.ThirdParty;

public class IntegratedIAcvService(
    IConfigurationService configurationService,
    CoreContext context,
    IFlightService flightService) : BaseService, IIntegratedIAcvService
{
    #region Properties

    private readonly PaylaterUri _paylaterUri = new();

    #endregion

    #region Method

    #region Pagination

    public async Task<PaginationResult<QueryDataResponse>> GetByPartnerKeyAsync(QueryDataRequest request, CancellationToken cancellationToken = default)
    {
        await GetConfigDataAsync(cancellationToken);
        var masterDataResult = await flightService.GetMasterDataAsync(false, cancellationToken);

        if (masterDataResult.CodeMessage != CodeMessage._0000)
            return GetPaginationResult<QueryDataResponse>(CodeMessage._0002);
        var billResult = await GetBillsAsync(request, masterDataResult.Data, cancellationToken);

        // Mapping master-data
        QueryDataResponse data = new();
        if (billResult.total > 0)
            data.Items = RelateMapping.MappingModelToResouce(billResult.records, masterDataResult.Data,
                _paylaterUri with
                {
                    PlatformType = request.PlatformType
                });

        var result = GetPaginationResult(CodeMessage._0000, data);
        result.CreatePaginationResult(request, billResult.total);

        return result;
    }

    private async Task<(List<Model.Bill> records, int total)> GetBillsAsync(QueryDataRequest request,
        MasterDataResponse? masterData,
        CancellationToken cancellationToken = default)
    {
        DateTime utcNow = DateTime.UtcNow;

        var queryable = context.Bills
            .Include(x => x.FlightDatas)
            .Include(x => x.PaymentTransactions
                .Where(y => y.PaymentProviderStatus == MyEnum.PaymentStatus.Success))
            .Where(x => x.PartnerKey == request.PartnerKey &&
                        (x.ExpiredDatetimeUtc > utcNow ||
                         x.PaymentTransactions.Any(y => y.PaymentProviderStatus == MyEnum.PaymentStatus.Success)))
            .AsQueryable();

        // Condition filter
        if (request.Filter != null)
        {
            if (!string.IsNullOrEmpty(request.Filter?.PointName))
            {
                var airportsCode = masterData?.Airports?.Where(x =>
                        request.Filter.PointName.ContainsVietnameseString(x.Code) ||
                        request.Filter.PointName.ContainsVietnameseString(x.Name) ||
                        request.Filter.PointName.ContainsVietnameseString(x.CityName))
                    .Select(x => x.Code)
                    .ToHashSet();

                var billIds = context.FlightDatas
                    .Where(x => airportsCode.Contains(x.StartPoint) || airportsCode.Contains(x.EndPoint))
                    .Select(x => x.BillId);

                queryable = queryable.Where(x => billIds.Contains(x.Id));
            }

            if (!string.IsNullOrEmpty(request.Filter?.OrderId))
                queryable = queryable.Where(x => x.AbTripOrderId == request.Filter.OrderId);

            if (request.Filter?.StartDate != null)
            {
                DateTime converted = request.Filter.StartDate.Value.ToDateTime(new()).ConvertVietnamTzToUtc();
                queryable = queryable.Where(x => x.CreatedDatetimeUtc >= converted);
            }

            if (request.Filter?.EndDate != null)
            {
                DateTime converted = request.Filter.EndDate.Value.ToDateTime(new(23, 59, 59, 999)).ConvertVietnamTzToUtc();
                queryable = queryable.Where(x => x.CreatedDatetimeUtc <= converted);
            }
        }

        // Condition sort
        if (request.Sort != null)
        {
            if (request.Sort?.OrderType != null)
            {
                if (request.Sort?.OrderType == MyEnum.OrderType.FlightStartAsc)
                    queryable = queryable.OrderBy(x => x.FlightDatas.Where(y => y.Departure).Min(z => z.CreatedDatetimeUtc));
                if (request.Sort?.OrderType == MyEnum.OrderType.FlightStartDesc)
                    queryable = queryable.OrderByDescending(x => x.FlightDatas.Where(y => y.Departure).Min(z => z.CreatedDatetimeUtc));

                if (request.Sort?.OrderType == MyEnum.OrderType.FlightEndAsc)
                    queryable = queryable.OrderBy(x => x.FlightDatas.Where(y => !y.Departure).Min(z => z.CreatedDatetimeUtc));
                if (request.Sort?.OrderType == MyEnum.OrderType.FlightEndDesc)
                    queryable = queryable.OrderByDescending(x => x.FlightDatas.Where(y => !y.Departure).Min(z => z.CreatedDatetimeUtc));

                if (request.Sort?.OrderType == MyEnum.OrderType.OrderAsc)
                    queryable = queryable.OrderBy(x => x.CreatedDatetimeUtc);
                if (request.Sort?.OrderType == MyEnum.OrderType.OrderDesc)
                    queryable = queryable.OrderByDescending(x => x.CreatedDatetimeUtc);
            }
            else
                queryable = queryable.OrderByDescending(x => x.CreatedDatetimeUtc);
        }
        else
        {
            queryable = queryable.OrderByDescending(x => x.CreatedDatetimeUtc);
        }

        var total = await queryable.CountAsync(cancellationToken);

        var records = await queryable.AsNoTracking()
            .Skip((request.Page!.Value - 1) * request.PageSize!.Value)
            .Take(request.PageSize.Value)
            .ToListAsync(cancellationToken);

        return (records, total);
    }

    #endregion

    #region Private work

    private async Task GetConfigDataAsync(CancellationToken cancellationToken = default)
    {
        // Get config from DB
        var configurations = await configurationService.GetAllAsync(false, cancellationToken);

        if (configurations.CodeMessage != CodeMessage._0000)
            throw new MessageResultException("Không thể thực hiện lấy config");

        foreach (var configuration in configurations.Data!)
        {
            if (configuration.Key == SystemConfig.SystemFeHost)
            {
                this._paylaterUri.HostFe = configuration.Value!;
                continue;
            }

            if (configuration.Key == SystemConfig.SystemPaylaterEndpoint)
            {
                this._paylaterUri.PaylaterEndpoint = configuration.Value!;
                continue;
            }
        }
    }

    #endregion

    #endregion
}