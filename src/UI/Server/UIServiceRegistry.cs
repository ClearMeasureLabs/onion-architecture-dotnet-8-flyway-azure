using Lamar;
using Microsoft.EntityFrameworkCore;
using ProgrammingWithPalermo.ChurchBulletin.Core;
using ProgrammingWithPalermo.ChurchBulletin.Core.Model;
using ProgrammingWithPalermo.ChurchBulletin.DataAccess;
using ProgrammingWithPalermo.ChurchBulletin.DataAccess.Mappings;
using UI.Client;

namespace ProgrammingWithPalermo.ChurchBulletin.UI.Server;

public class UiServiceRegistry : ServiceRegistry
{
    public UiServiceRegistry()
    {
        this.AddScoped<DbContext, DataContext>();
        this.AddDbContextFactory<DataContext>();
        this.AddDbContextFactory<DbContext>();
        
        this.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<UiServiceRegistry>());
        this.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Api.HealthCheck>());
        this.AddTransient<IBus, Bus>();

        Scan(scanner =>
        {
            scanner.WithDefaultConventions();
            scanner.AssemblyContainingType<CanConnectToDatabaseHealthCheck>();
            scanner.AssemblyContainingType<Api.HealthCheck>();
            scanner.AssemblyContainingType<Is64BitProcessHealthCheck>();
            scanner.AssemblyContainingType<CanConnectToLlmServerHealthCheck>();
        });

        this.AddHealthChecks()
            .AddCheck<CanConnectToLlmServerHealthCheck>("LlmGateway")
            .AddCheck<CanConnectToDatabaseHealthCheck>("DataAccess")
            .AddCheck<Is64BitProcessHealthCheck>("Server")
            .AddCheck<Api.HealthCheck>("API");
    }
}