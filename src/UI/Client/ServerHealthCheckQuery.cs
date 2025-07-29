using MediatR;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ProgrammingWithPalermo.ChurchBulletin.Core;

namespace UI.Client;

public record ServerHealthCheckQuery : IRequest<HealthStatus>, IRemotableRequest;