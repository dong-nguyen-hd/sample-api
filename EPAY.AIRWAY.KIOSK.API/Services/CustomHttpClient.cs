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

    public async Task<(bool isSuccess, TRes? data)> SendAsync<TRes>(MyHttpRequest request, CancellationToken cancellationToken)
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
                Content = new StringContent(request.Payload ?? string.Empty, Encoding.UTF8, MimeType.JSON),
            };

            var task = client.SendAsync(httpRequest, cancellationToken);
            await RetryRequestAsync(request, task, cancellationToken);
            var rawResponse = await task.Result.Content.ReadAsStringAsync(cancellationToken);

            SetLogResponse(log, task.Result, rawResponse);

            // Process result
            if (task.Result.IsSuccessStatusCode && !string.IsNullOrEmpty(rawResponse))
                return (true, JsonSerializer.Deserialize<TRes>(rawResponse));

            return (false, default);
        }
        catch (Exception ex)
        {
            SetLogException(log, ex);
            return (false, default);
        }
        finally
        {
            logService.CreateAsync(log);
        }
    }

    #region Private work

    private HttpClient GetHttpClient(MyHttpRequest webHook)
    {
        if (webHook.EnableVerifyTls)
            return httpClientFactory.CreateClient(RelateHttpClient.EnableTLS);

        return httpClientFactory.CreateClient(RelateHttpClient.DisableTLS);
    }

    private static HttpMethod MappingHttpMethod(MyHttpRequest webHook)
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
    /// <param name="task"></param>
    /// <param name="cancellationToken"></param>
    private async Task RetryRequestAsync(MyHttpRequest webHook, Task<HttpResponseMessage> task, CancellationToken cancellationToken)
    {
        var maxRetryAttempts = webHook.NumberRetry;

        if (maxRetryAttempts == 0)
        {
            await task;
        }
        else
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

            await pipeline.ExecuteAsync(async token => await task, cancellationToken);
        }
    }

    /// <summary>
    /// Chức năng: gán header cho request
    /// </summary>
    /// <param name="client"></param>
    /// <param name="webHook"></param>
    /// <param name="log"></param>
    private void SetHeader(HttpClient client, MyHttpRequest webHook, Model.Log log)
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
    private Model.Log GetLog(MyHttpRequest request)
    {
        Model.Log temp = new()
        {
            Node = SystemInformation.Node,
            TraceId = _httpContext != null ? _httpContext.TraceIdentifier : "<none>",
            LogType = LogType.ThirdPartyLog,
            RequestMethod = Enum.GetName(request.MyHttpMethod),
            RequestDatetimeUtc = DateTime.UtcNow,
            RequestPath = request.Uri.AbsolutePath,
            RequestQuery = request.Uri.Query,
            RequestHost = request.Uri.Host,
            RequestScheme = request.Uri.Scheme,
            RequestQueries = request.Uri.Query.FormatQueries(),
            RequestBody = request.Payload,
            RequestContentType = MimeType.JSON
        };

        return temp;
    }

    /// <summary>
    /// Chức năng: gán dữ liệu response cho log-model
    /// </summary>
    /// <param name="log"></param>
    /// <param name="response"></param>
    /// <param name="rawResponse"></param>
    private void SetLogResponse(Model.Log log, HttpResponseMessage response, string? rawResponse)
    {
        log.ResponseDatetimeUtc = DateTime.UtcNow;
        log.ResponseStatus = response?.StatusCode.ToString("D");
        log.ResponseContentType = response?.Content?.Headers?.ContentType?.MediaType;
        log.ResponseHeaders = GetHeader();
        log.ResponseBody = rawResponse;

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