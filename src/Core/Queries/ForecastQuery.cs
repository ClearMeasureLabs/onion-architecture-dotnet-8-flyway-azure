using MediatR;
using ProgrammingWithPalermo.ChurchBulletin.Core.Model;

namespace ProgrammingWithPalermo.ChurchBulletin.Core.Queries;

public class ForecastQuery : IRequest<WeatherForecast[]>, IRemoteableRequest
{
}