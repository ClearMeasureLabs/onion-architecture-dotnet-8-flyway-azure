using MediatR;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ProgrammingWithPalermo.ChurchBulletin.Core;

namespace ProgrammingWithPalermo.ChurchBulletin.DataAccess;

public class HealthCheckRemotableRequestHandler : IRequestHandler<HealthCheckRemotableRequest, HealthStatus>
{
    public Task<HealthStatus> Handle(HealthCheckRemotableRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(HealthStatus.Healthy);
    }
}