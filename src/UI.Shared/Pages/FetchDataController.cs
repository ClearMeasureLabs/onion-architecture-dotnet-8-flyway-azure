using System.Diagnostics;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Palermo.BlazorMvc;
using ProgrammingWithPalermo.ChurchBulletin.Core;
using ProgrammingWithPalermo.ChurchBulletin.Core.Model;
using ProgrammingWithPalermo.ChurchBulletin.Core.Queries;

namespace UI.Shared.Pages;

[Route("/fetchdata")]
public class FetchDataController : ControllerComponentBase<FetchDataView>
{
    private WeatherForecast[]? _forecasts;
    [Inject] public IBus? ApplicationBus { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Logger.LogInformation("FetchDataController");
        _forecasts = await ApplicationBus!.Send(new ForecastQuery());
        View.Model = _forecasts;
    }
}