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
using Lamar;
using Lamar.Microsoft.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var http = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
var ConfigurationModel = await http.GetFromJsonAsync<ConfigurationModel>("Configuration");
builder.Services.AddScoped(sp => http);

if (ConfigurationModel != null)
{
    builder.Services.AddBlazorApplicationInsights(x =>
    {
        x.ConnectionString = ConfigurationModel.AppInsightsConnectionString;
    });
}

// Add authentication services
builder.Services.AddAuthorizationCore();
builder.ConfigureContainer<ServiceRegistry>(
    new LamarServiceProviderFactory(), registry => 
        registry.IncludeRegistry<UIClientServiceRegistry>());


string url = builder.Configuration.GetValue<string>("RemoteBusUrl") ?? throw new InvalidOperationException("Must have config value 'RemoteBusUrl'");
builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(url) });

await builder.Build().RunAsync();