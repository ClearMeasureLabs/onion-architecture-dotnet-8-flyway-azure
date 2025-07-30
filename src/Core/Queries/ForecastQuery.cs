using MediatR;
using ProgrammingWithPalermo.ChurchBulletin.Core.Model;

namespace ProgrammingWithPalermo.ChurchBulletin.Core.Queries;

public record ForecastQuery : IRequest<WeatherForecast[]>, IRemotableRequest
{
}