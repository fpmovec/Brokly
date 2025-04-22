using Brokly.Contracts.RequestsHandling;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Brokly.Application.RequestHandling;

/// <summary>
/// Represents the class that contains request handling implementation
/// </summary>
public class RequestSender(
    ILogger<RequestSender> logger,
    IServiceProvider serviceProvider) : IRequestSender
{
    public async Task<TResult> Send<TRequest, TResult>(TRequest request, CancellationToken cancellationToken)
        where TRequest : IRequest<TResult>
    {
        try
        {
            var handler = serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResult>>();

            var handleResult = await handler.HandleAsync(request, cancellationToken);

            return handleResult;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);

            throw;
        }
    }

    public async Task Send<TRequest>(TRequest request, CancellationToken cancellationToken)
        where TRequest : IRequest
    {
        try
        {
            var handler = serviceProvider.GetRequiredService<IRequestHandler<TRequest>>();

            await handler.HandleAsync(request, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);

            throw;
        }
    }
}