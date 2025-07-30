using MediatR;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ClearMeasure.Bootcamp.Core;

public record HealthCheckRemotableRequest : IRequest<HealthStatus>, IRemotableRequest
{ }