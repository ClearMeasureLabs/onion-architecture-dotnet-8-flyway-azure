using ClearMeasure.Bootcamp.Core;
using ClearMeasure.Bootcamp.Core.Model;
using ClearMeasure.Bootcamp.Core.Queries;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Palermo.BlazorMvc;

namespace ClearMeasure.Bootcamp.UI.Shared.Pages;

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