using Serilog.Context;

namespace TaskManager.API.Middlewares;

public class CorrelationLogMiddleware
{
    private readonly ILogger<CorrelationLogMiddleware> _logger;
    private readonly RequestDelegate _next;

    public CorrelationLogMiddleware(RequestDelegate next, ILogger<CorrelationLogMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        var correlationId = httpContext.Request.Headers["X-Correlation-ID"].FirstOrDefault();

        if (string.IsNullOrEmpty(correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
        }

        httpContext.Response.OnStarting(() =>
        {
            httpContext.Response.Headers["X-Correlation-ID"] = correlationId;
            return Task.CompletedTask;
        });

        _logger.LogInformation($"Correlation ID: {correlationId}");

        httpContext.Items["CorrelationId"] = correlationId;

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(httpContext);
        }
    }
}