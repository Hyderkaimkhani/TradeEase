using Serilog.Context;

namespace Api.Middleware
{
    public class ExceptionHandleMiddleware
    {
        private readonly RequestDelegate requestDelegate;
        private readonly ILogger<ExceptionHandleMiddleware> logger;

        public ExceptionHandleMiddleware(RequestDelegate next, ILogger<ExceptionHandleMiddleware> logger)
        {
            this.requestDelegate = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await requestDelegate(context);
            }
            catch (Exception ex)
            {
                HandleExceptionAsync(context, ex);
                throw;
            }
        }

        private void HandleExceptionAsync(HttpContext c, Exception ex)
        {
            using (LogContext.PushProperty("Source", "Api"))
            {
                logger.LogError(
                    $"Exception Message @ {ex.Message}, Inner Exception @ {ex.InnerException}, Stack Trace @ {ex.StackTrace}");
            }
        }
    }
}
