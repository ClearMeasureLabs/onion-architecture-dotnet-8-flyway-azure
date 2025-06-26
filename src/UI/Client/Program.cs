using System.Collections;
using System.Net.Http.Json;
using BlazorApplicationInsights;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Palermo.BlazorMvc;
using ProgrammingWithPalermo.ChurchBulletin.Core;
using UI.Client;
using UI.Shared.Authentication;
using Core.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var http = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
var ConfigurationModel = await http.GetFromJsonAsync<ConfigurationModel>("Configuration");

if (ConfigurationModel != null)
{
    builder.Services.AddBlazorApplicationInsights(x =>
    {
        x.ConnectionString = ConfigurationModel.AppInsightsConnectionString;
    });
}

// Add authentication services
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => 
    provider.GetRequiredService<CustomAuthenticationStateProvider>());

builder.Services.AddScoped<IUiBus>(provider => new MvcBus(NullLogger<MvcBus>.Instance));
builder.Services.AddScoped(sp => http);
builder.Services.AddScoped<IEmployeeRepository, UI.Client.HttpEmployeeRepository>();
builder.Services.AddScoped<IUserSession, UI.Services.UserSession>();

await builder.Build().RunAsync();
