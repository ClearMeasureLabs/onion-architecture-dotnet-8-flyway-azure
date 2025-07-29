using MediatR;
using UI.Client;

namespace ProgrammingWithPalermo.ChurchBulletin.UI.Server.Handlers;

public class ServerHealthCheckHandler(HealthCheckService healthCheckService) : IRequestHandler<ServerHealthCheckQuery, HealthStatus>
{
    public async Task<HealthStatus> Handle(ServerHealthCheckQuery request, CancellationToken cancellationToken)
    {
        var checkHealthAsync = await healthCheckService.CheckHealthAsync(cancellationToken);
        return checkHealthAsync.Status;
    }
}