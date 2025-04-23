using Microsoft.AspNetCore.Http.Extensions;
using Serilog.Context;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Api.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly ILogger<RequestLoggingMiddleware> logger;
        private readonly RequestDelegate requestDelegate;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            this.requestDelegate = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                if (httpContext.Request.Path.Value != null && !httpContext.Request.Path.Value.ToLower().Contains("swagger"))
                {
                    var requestBodyStream = new MemoryStream();
                    var originalRequestBody = httpContext.Request.Body;

                    await httpContext.Request.Body.CopyToAsync(requestBodyStream);
                    requestBodyStream.Seek(0, SeekOrigin.Begin);

                    var url = UriHelper.GetDisplayUrl(httpContext.Request);
                    var requestBodyText = new StreamReader(requestBodyStream).ReadToEnd();
                    using (LogContext.PushProperty("Source", "TradeEase-Api"))
                    {
                        try
                        {
                            if ((httpContext.Request.Method == HttpMethods.Post || httpContext.Request.Method == HttpMethods.Put) && !httpContext.Request.HasFormContentType)
                            {
                                var requestTokens = JsonObject.Parse(requestBodyText);

                                logger.LogInformation("Received request to Api. Request Info: {@request}", new
                                {
                                    Method = httpContext.Request.Method,
                                    Path = url,
                                    ResponseBody = JsonSerializer.Serialize(requestTokens)
                                });
                            }
                            else
                            {
                                logger.LogInformation("Received request to Api. Request Info: {@request}", new
                                {
                                    Method = httpContext.Request.Method,
                                    Path = url,
                                    ResponseBody = JsonSerializer.Serialize(requestBodyText)
                                });
                            }
                        }
                        catch
                        {
                            logger.LogInformation($"REQUEST Log :: METHOD: {httpContext.Request.Method},REQUEST URL: {url}, Catched exception while parsing.This will not effect the request.");
                            logger.LogInformation($"REQUEST Log :: METHOD: {httpContext.Request.Method},REQUEST URL: {url}, REQUEST BODY: {requestBodyText}");
                        }
                    }
                    requestBodyStream.Seek(0, SeekOrigin.Begin);
                    httpContext.Request.Body = requestBodyStream;

                    var bodyStream = httpContext.Response.Body;

                    var responseBodyStream = new MemoryStream();
                    httpContext.Response.Body = responseBodyStream;

                    await requestDelegate(httpContext);

                    httpContext.Request.Body = originalRequestBody;
                    responseBodyStream.Seek(0, SeekOrigin.Begin);
                    var responseBody = new StreamReader(responseBodyStream).ReadToEnd();
                    using (LogContext.PushProperty("Source", "TradeEase-Api"))
                    {
                        logger.LogInformation("Completed request in Api. Response Info: {@response}", new
                        {
                            Method = httpContext.Request.Method,
                            Path = httpContext.Request.Path,
                            StatusCode = httpContext.Response.StatusCode,
                            ResponseBody = responseBody
                        });
                    }
                    responseBodyStream.Seek(0, SeekOrigin.Begin);

                    if (httpContext.Response.StatusCode != StatusCodes.Status204NoContent)
                    {
                        await responseBodyStream.CopyToAsync(bodyStream);
                    }
                }
                else
                {
                    await requestDelegate(httpContext);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(
                    $"Exception : {ex.Message},Inner Exception : {ex.InnerException?.Message},Stack Trace : {ex.StackTrace}");
                await requestDelegate(httpContext);
            }
        }
    }
}
