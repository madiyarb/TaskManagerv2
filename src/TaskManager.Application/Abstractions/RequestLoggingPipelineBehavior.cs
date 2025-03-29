using Ardalis.Result;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace TaskManager.Application.Abstractions;

public class RequestLoggingPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
    where TRequest : class, IRequest<TResponse>
    where TResponse : Result
{
    private readonly ILogger<RequestLoggingPipelineBehavior<TRequest, TResponse>> _logger;

    public RequestLoggingPipelineBehavior(ILogger<RequestLoggingPipelineBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        string requestName = typeof(TRequest).Name;
        _logger.LogInformation("Handling request {requestName}", requestName);
        TResponse response = await next();
        if (response.IsSuccess)
        {
            _logger.LogInformation("Successfully handled request {requestName}", requestName);
        }
        else
        {
            using (LogContext.PushProperty("Error", response.Errors, true))
            {
                _logger.LogError("Failed handling request {requestName} with Errors {errors}, with {validationErrors}",
                    requestName, response.Errors, response.ValidationErrors);
            }
        }

        return response;
    }
}