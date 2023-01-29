using System;
using FluentValidation;
using MediatR;
using Domain.Requests;

namespace Application.Futures.StatusFuture.Queries;

public class PingQueryValidator : AbstractValidator<PingQueryRequest>
{
    public PingQueryValidator()
    {
    }
}

public class PingQueryRequest : IRequest<IRestResponseResult>
{
}

public class PingQueryRequestHandler : IRequestHandler<PingQueryRequest, IRestResponseResult>
{
    public Task<IRestResponseResult> Handle(PingQueryRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(RestResponseResult.Success("Pong!"));
    }
}

