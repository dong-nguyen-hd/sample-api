using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Pagination.Request;

namespace EPAY.AIRWAY.KIOSK.API.Extensions;

public static class RelatePagination
{
    public static void CreatePaginationResult<TRes, TFilter, TSort>(this PaginationResult<TRes> response,
        QueryDataRequest<TFilter, TSort> pagination,
        int totalRecords) where TFilter : FilterRequest where TSort : SortRequest
    {
        // Assign Query-Resource
        response.Page = pagination.Page;
        response.PageSize = pagination.PageSize;

        // Assign Total-Pages
        var totalPages = ((double)totalRecords / (double)pagination.PageSize!);
        int roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));

        // Assign Next-Page
        response.NextPage = (pagination.Page >= 1 && pagination.Page < roundedTotalPages) ? pagination.Page + 1 : null;

        // Assign Previous-Page
        response.PreviousPage = (pagination.Page - 1 >= 1 && pagination.Page <= roundedTotalPages) ? pagination.Page - 1 : null;

        // Assign First-Page
        response.FirstPage = 1;

        // Assign Last-Page
        response.LastPage = roundedTotalPages;

        // Assign Total-Pages
        response.TotalPages = roundedTotalPages;

        // Assign Total-Records
        response.TotalRecords = totalRecords;
    }
}