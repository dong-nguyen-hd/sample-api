namespace AIRWAY.KIOSK.API.Resources.DTOs.Pagination.Request;

public abstract class QueryDataRequest<TFilter, TSort> where TFilter : FilterRequest where TSort : SortRequest
{
    private int? _page;

    public int? Page
    {
        get => _page;
        set => SetPage(value);
    }

    private int? _pageSize;

    public int? PageSize
    {
        get => _pageSize;
        set => SetPageSize(value);
    }

    public TFilter? Filter { get; set; }
    public TSort? Sort { get; set; }

    #region Method

    private void SetPage(int? value) =>
        _page = value switch
        {
            null or <= 0 => 1,
            _ => value
        };

    private void SetPageSize(int? value) =>
        _pageSize = value switch
        {
            null or <= 0 or > 100 => 20,
            _ => value
        };

    #endregion
}