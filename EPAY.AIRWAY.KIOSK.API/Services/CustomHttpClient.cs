using System.Net.Mime;
using System.Text;
using EPAY.AIRWAY.KIOSK.API.Domain.Services;
using EPAY.AIRWAY.KIOSK.API.Extensions.AddConfig;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.CustomHttpClient.Request;
using EPAY.AIRWAY.KIOSK.API.Resources.Enums;
using Polly;
using Polly.Retry;

namespace EPAY.AIRWAY.KIOSK.API.Services;

public sealed class CustomHttpClient(
    IHttpClientFactory httpClientFactory,
    ILogService logService,
    IHttpContextAccessor httpContextAccessor) : ICustomHttpClient
{
    #region Properties

    private readonly HttpContext? _httpContext = httpContextAccessor?.HttpContext;

    #endregion

    #region Method

    public async Task<(CodeMessage codeMessage, TRes? data)> SendAsync<TReq, TRes>(MyHttpRequest<TReq> request,
        Func<HttpResponseMessage, string, (CodeMessage, TRes?)>? func = null,
        CodeMessage codeMessageWhenException = CodeMessage._3005,
        CancellationToken cancellationToken = default)
    {
        var log = GetLog(request);

        try
        {
            // Set up client
            var client = GetHttpClient(request);
            
            SetHeader(client, request, log);

            // Request
            HttpRequestMessage httpRequest = new()
            {
                Method = MappingHttpMethod(request),
                RequestUri = request.Uri,
                Content = new StringContent(request.Payload?.MySerialize() ?? string.Empty, Encoding.UTF8, MediaTypeNames.Application.Json)
            };
            var response = await RetryRequestAsync(request, client, httpRequest, cancellationToken);
            var rawPayload = await response.Content.ReadAsStringAsync(cancellationToken);

            SetLogResponse<TRes>(log, response, rawPayload);

            // Process result
            if (func != null)
                return func.Invoke(response, rawPayload);
            else
            {
                if (response.IsSuccessStatusCode && !string.IsNullOrEmpty(rawPayload))
                    return (CodeMessage._0000, JsonSerializer.Deserialize<TRes>(rawPayload));

                return (codeMessageWhenException, default);
            }
        }
        catch (Exception ex)
        {
            SetLogException(log, ex);
            return (codeMessageWhenException, default);
        }
        finally
        {
            logService.CreateAsync(log);
        }
    }

    #region Private work

    private HttpClient GetHttpClient<TReq>(MyHttpRequest<TReq> webHook)
    {
        if (webHook.EnableVerifyTls)
            return httpClientFactory.CreateClient(RelateHttpClient.EnableTLS);

        return httpClientFactory.CreateClient(RelateHttpClient.DisableTLS);
    }

    private static HttpMethod MappingHttpMethod<TReq>(MyHttpRequest<TReq> webHook)
    {
        switch (webHook.MyHttpMethod)
        {
            case MyHttpMethod.DELETE:
                return HttpMethod.Delete;
            case MyHttpMethod.GET:
                return HttpMethod.Get;
            case MyHttpMethod.POST:
                return HttpMethod.Post;
            case MyHttpMethod.PUT:
                return HttpMethod.Put;
            default:
                return HttpMethod.Get;
        }
    }

    /// <summary>
    /// Chức năng: tăng số lượt thử lại request khi thất bại
    /// </summary>
    /// <param name="webHook"></param>
    /// <param name="httpClient"></param>
    /// <param name="httpRequest"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<HttpResponseMessage> RetryRequestAsync<TReq>(MyHttpRequest<TReq> webHook, HttpClient httpClient, HttpRequestMessage httpRequest, CancellationToken cancellationToken)
    {
        var maxRetryAttempts = webHook.NumberRetry;
        if (maxRetryAttempts != 0)
        {
            var pauseBetweenFailures = TimeSpan.FromSeconds(2);

            var pipeline = new ResiliencePipelineBuilder<HttpResponseMessage>()
                .AddRetry(new RetryStrategyOptions<HttpResponseMessage>
                {
                    ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                        .Handle<Exception>()
                        .HandleResult(static result => !result.IsSuccessStatusCode),
                    Delay = pauseBetweenFailures,
                    MaxRetryAttempts = maxRetryAttempts,
                    BackoffType = DelayBackoffType.Constant
                })
                .Build();

            return await pipeline.ExecuteAsync(async token =>
                await httpClient.SendAsync(new HttpRequestMessage()
                {
                    Method = httpRequest.Method,
                    RequestUri = httpRequest.RequestUri,
                    Content = httpRequest.Content
                }, cancellationToken), cancellationToken);
        }

        return await httpClient.SendAsync(httpRequest, cancellationToken);
    }

    /// <summary>
    /// Chức năng: gán header cho request
    /// </summary>
    /// <param name="client"></param>
    /// <param name="webHook"></param>
    /// <param name="log"></param>
    private void SetHeader<TReq>(HttpClient client, MyHttpRequest<TReq> webHook, Model.Log log)
    {
        // Set other header
        if (webHook.Headers is not null and { Count: > 0 })
        {
            Dictionary<string, string> headers = new();

            foreach (var header in webHook.Headers)
            {
                if (string.IsNullOrEmpty(header.Key))
                    continue;

                client.DefaultRequestHeaders.Add(header.Key, header.Value ?? string.Empty);
                headers.TryAdd(header.Key, header.Value ?? string.Empty);
            }

            log.RequestHeaders = headers;
        }
    }

    /// <summary>
    /// Chức năng: tạo log-model
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    private Model.Log GetLog<TReq>(MyHttpRequest<TReq> request)
    {
        Model.Log temp = new()
        {
            Node = SystemInformation.Node,
            TraceId = _httpContext != null ? _httpContext.TraceIdentifier : "<none>",
            LogType = LogType.ThirdPartyLog,
            RequestMethod = Enum.GetName(request.MyHttpMethod),
            RequestDatetimeUtc = DateTime.UtcNow,
            RequestPath = request.Uri?.AbsolutePath,
            RequestQuery = request.Uri?.Query,
            RequestHost = request.Uri?.Host,
            RequestScheme = request.Uri?.Scheme,
            RequestQueries = request.Uri?.Query.FormatQueries(),
            RequestContentType = MediaTypeNames.Application.Json
        };

        // Xử lí redaction
        if (request.Payload != null)
        {
            try
            {
                temp.RequestBody = request.Payload.MaskSensitiveData();
            }
            catch
            {
                temp.RequestBody = string.Empty;
            }
        }

        return temp;
    }

    /// <summary>
    /// Chức năng: gán dữ liệu response cho log-model
    /// </summary>
    /// <param name="log"></param>
    /// <param name="response"></param>
    /// <param name="rawResponse"></param>
    private void SetLogResponse<TRes>(Model.Log log, HttpResponseMessage response, string? rawResponse)
    {
        log.ResponseDatetimeUtc = DateTime.UtcNow;
        log.ResponseStatus = response?.StatusCode.ToString("D");
        log.ResponseContentType = response?.Content?.Headers?.ContentType?.MediaType;
        log.ResponseHeaders = GetHeader();

        // Xử lí redaction
        if (!string.IsNullOrEmpty(rawResponse))
        {
            try
            {
                var parseObj = JsonSerializer.Deserialize<TRes>(rawResponse);
                log.ResponseBody = parseObj?.MaskSensitiveData();
            }
            catch
            {
                log.ResponseBody = rawResponse;
            }
        }

        Dictionary<string, string> GetHeader()
        {
            Dictionary<string, string> temp = new();

            foreach (var item in response?.Headers?.ToDictionary())
                temp.TryAdd(item.Key, string.Join(';', item.Value?.ToArray()));

            return temp;
        }
    }

    /// <summary>
    /// Chức năng: gán dữ liệu exception cho log-model
    /// </summary>
    /// <param name="log"></param>
    /// <param name="ex"></param>
    private void SetLogException(Model.Log log, Exception ex)
    {
        log.HasException = true;
        log.ExceptionMessage = ex.Message;
        log.ExceptionStackTrace = ex.StackTrace;
    }

    #endregion

    #endregion
}