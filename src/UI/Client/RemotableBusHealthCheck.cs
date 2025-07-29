using Microsoft.Extensions.Diagnostics.HealthChecks;
using ProgrammingWithPalermo.ChurchBulletin.Core;

namespace UI.Client;

public class RemotableBusHealthCheck(IBus bus, ILogger<RemotableBusHealthCheck> logger) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        var remotableRequest = new HealthCheckRemotableRequest();
        var result = await bus.Send(remotableRequest);
        logger.LogInformation(result.ToString());
        return new HealthCheckResult(result);
    }
}