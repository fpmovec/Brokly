﻿namespace Brokly.Contracts.RequestsHandling;

public interface IRequestHandler<in TRequest, TResult>
    where TRequest: IRequest<TResult>
{
    Task<TResult> HandleAsync(TRequest request, CancellationToken cancellationToken);
}

public interface IRequestHandler<in TRequest>
    where TRequest: IRequest
{
    Task HandleAsync(TRequest request, CancellationToken cancellationToken);
}