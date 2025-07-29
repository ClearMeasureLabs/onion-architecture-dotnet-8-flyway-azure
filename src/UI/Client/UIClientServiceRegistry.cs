using Core.Services;
using Lamar;
using MediatR;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging.Abstractions;
using Palermo.BlazorMvc;
using ProgrammingWithPalermo.ChurchBulletin.Core;
using UI.Shared;
using UI.Shared.Authentication;

namespace UI.Client;

// ReSharper disable once InconsistentNaming
public class UIClientServiceRegistry : ServiceRegistry
{
    public UIClientServiceRegistry()
    {
        this.AddScoped<CustomAuthenticationStateProvider>();
        this.AddScoped<AuthenticationStateProvider>(provider =>
            provider.GetRequiredService<CustomAuthenticationStateProvider>());

        this.AddScoped<IUiBus>(provider => new MvcBus(NullLogger<MvcBus>.Instance));
        this.AddScoped<IEmployeeRepository, UI.Client.HttpEmployeeRepository>();
        this.AddScoped<IWorkOrderRepository, UI.Client.HttpWorkOrderRepository>();
        this.AddScoped<IUserSession, UI.Services.UserSession>();
        this.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<RemotableBus>());
        this.AddTransient<IBus, RemotableBus>();

        this.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<UIClientServiceRegistry>());

        Scan(scanner =>
        {
            scanner.WithDefaultConventions();
            scanner.AssemblyContainingType<UIClientServiceRegistry>();
            scanner.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>));
            scanner.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));
        });

        this.AddHealthChecks().AddCheck<HealthCheckTracer>("UI.Client");
        this.AddHealthChecks().AddCheck<RemotableBusHealthCheck>("Remotable Bus");
        this.AddHealthChecks().AddCheck<ServerHealthCheck>("Server health check");
    }
}