using System.Data;
using Dapper;
using EPAY.AIRWAY.KIOSK.API.Resources.Enums;

namespace EPAY.AIRWAY.KIOSK.API.Services.Log;

public partial class LogService
{
    #region Method

    private static (string sql, DynamicParameters param) CreateQuery(Model.Log model)
    {
        // Param component
        var param = new DynamicParameters();
        param.Add(":id", model.Id, dbType: DbType.String, direction: ParameterDirection.Input);
        param.Add(":node", model.Node, dbType: DbType.String, direction: ParameterDirection.Input);
        param.Add(":clientIp", model.ClientIp, dbType: DbType.String, direction: ParameterDirection.Input);

        param.Add(":requestQueries", JsonSerializer.Serialize(model.RequestQueries), dbType: DbType.String, direction: ParameterDirection.Input);
        param.Add(":traceId", model.TraceId, dbType: DbType.String, direction: ParameterDirection.Input);
        param.Add(":logType", Enum.GetName(typeof(LogType), model.LogType), dbType: DbType.String, direction: ParameterDirection.Input);

        param.Add(":requestDatetimeUtc", model.RequestDatetimeUtc, dbType: DbType.DateTime, direction: ParameterDirection.Input);
        param.Add(":requestPath", model.RequestPath, dbType: DbType.String, direction: ParameterDirection.Input);
        param.Add(":requestQuery", model.RequestQuery, dbType: DbType.String, direction: ParameterDirection.Input);

        param.Add(":requestMethod", model.RequestMethod, dbType: DbType.String, direction: ParameterDirection.Input);
        param.Add(":requestHost", model.RequestHost, dbType: DbType.String, direction: ParameterDirection.Input);
        param.Add(":requestBody", model.RequestBody, dbType: DbType.String, direction: ParameterDirection.Input);

        param.Add(":requestContentType", model.RequestContentType, dbType: DbType.String, direction: ParameterDirection.Input);
        param.Add(":responseDatetimeUtc", model.ResponseDatetimeUtc, dbType: DbType.DateTime, direction: ParameterDirection.Input);
        param.Add(":responseStatus", model.ResponseStatus, dbType: DbType.String, direction: ParameterDirection.Input);

        param.Add(":responseBody", model.ResponseBody, dbType: DbType.String, direction: ParameterDirection.Input);
        param.Add(":responseContentType", model.ResponseContentType, dbType: DbType.String, direction: ParameterDirection.Input);
        param.Add(":hasException", model.HasException, dbType: DbType.Boolean, direction: ParameterDirection.Input);

        param.Add(":exceptionMessage", model.ExceptionMessage, dbType: DbType.String, direction: ParameterDirection.Input);
        param.Add(":exceptionStackTrace", model.ExceptionStackTrace, dbType: DbType.String, direction: ParameterDirection.Input);
        param.Add(":requestHeaders", JsonSerializer.Serialize(model.RequestHeaders), dbType: DbType.String, direction: ParameterDirection.Input);

        param.Add(":responseHeaders", JsonSerializer.Serialize(model.ResponseHeaders), dbType: DbType.String, direction: ParameterDirection.Input);
        param.Add(":requestScheme", model.RequestScheme, dbType: DbType.String, direction: ParameterDirection.Input);

        // SQL component
        string query = @"INSERT INTO tbl_log
                            (id, node, client_ip,
                            request_queries, trace_id, log_type,
                            request_datetime_utc, request_path, request_query,
                            request_method, request_host, request_body,
                            request_content_type, response_datetime_utc, response_status,
                            response_body, response_content_type, has_exception,
                            exception_message, exception_stack_trace, request_headers,
                            response_headers, request_scheme)
                        VALUES (:id, :node, :clientIp,
                            :requestQueries::json, :traceId, :logType,
                            :requestDatetimeUtc, :requestPath, :requestQuery,
                            :requestMethod, :requestHost, :requestBody,
                            :requestContentType, :responseDatetimeUtc, :responseStatus,
                            :responseBody, :responseContentType, :hasException,
                            :exceptionMessage, :exceptionStackTrace, :requestHeaders::json,
                            :responseHeaders::json, :requestScheme)";

        return (query, param);
    }

    private static (string sql, DynamicParameters param) DeleteExpiredQuery(DateTime pivot)
    {
        // Param component
        var param = new DynamicParameters();
        param.Add(":pivot", pivot, dbType: DbType.DateTime, direction: ParameterDirection.Input);

        // SQL component
        string query = "DELETE FROM tbl_log WHERE request_datetime_utc <= :pivot";

        return (query, param);
    }

    #endregion
}