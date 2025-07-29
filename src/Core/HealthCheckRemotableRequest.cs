using MediatR;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ProgrammingWithPalermo.ChurchBulletin.Core;

public record HealthCheckRemotableRequest : IRequest<HealthStatus>, IRemotableRequest
{ }