using System.Diagnostics;
using System.Net.Http.Json;
using ClearMeasure.Bootcamp.Core.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Palermo.BlazorMvc;

namespace ClearMeasure.Bootcamp.UI.Shared.Pages;

[Route("/fetchchurchbulletin")]
public class FetchChurchBulletinController : ControllerComponentBase<FetchChurchBulletinView>
{
    private ChurchBulletinItem[]? _bulletins;
    [Inject] public HttpClient? Http { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Logger.LogInformation("FetchChurchBulletinController");
        Debug.Assert(Http != null, nameof(Http) + " != null");
        _bulletins = await Http.GetFromJsonAsync<ChurchBulletinItem[]>("ChurchBulletinItem");
        View.Model = _bulletins;
    }
}