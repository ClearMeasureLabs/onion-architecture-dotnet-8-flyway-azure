using Microsoft.Extensions.Diagnostics.HealthChecks;
using ProgrammingWithPalermo.ChurchBulletin.Core;

namespace UI.Client;

public class ServerHealthCheck(IBus bus, ILogger<ServerHealthCheck> logger) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        HealthStatus status = await bus.Send(new ServerHealthCheckQuery());
        logger.LogInformation(status.ToString());
        return new HealthCheckResult(status);
    }
}