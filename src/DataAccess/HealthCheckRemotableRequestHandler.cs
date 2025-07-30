using ClearMeasure.Bootcamp.Core;
using MediatR;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ClearMeasure.Bootcamp.DataAccess;

public class HealthCheckRemotableRequestHandler : IRequestHandler<HealthCheckRemotableRequest, HealthStatus>
{
    public Task<HealthStatus> Handle(HealthCheckRemotableRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(HealthStatus.Healthy);
    }
}