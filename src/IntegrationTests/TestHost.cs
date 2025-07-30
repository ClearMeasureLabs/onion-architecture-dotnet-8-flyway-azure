using System.CodeDom;
using AutoBogus;
using AutoBogus.Conventions;
using Bogus.Extensions;
using Core.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProgrammingWithPalermo.ChurchBulletin.Core;
using ProgrammingWithPalermo.ChurchBulletin.Core.Queries;
using ProgrammingWithPalermo.ChurchBulletin.DataAccess.Handlers;
using ProgrammingWithPalermo.ChurchBulletin.DataAccess.Mappings;

namespace ProgrammingWithPalermo.ChurchBulletin.IntegrationTests;

public static class TestHost
{
    private static bool _dependenciesRegistered;
    private static readonly object Lock = new();
    private static IHost? _host;

    public static IHost Instance
    {
        get
        {
            EnsureDependenciesRegistered();
            return _host!;
        }
    }

    public static T GetRequiredService<T>() where T : notnull
    {
        var serviceScope = Instance.Services.CreateScope();
        var provider = serviceScope.ServiceProvider;
        return provider.GetRequiredService<T>();
    }

    private static void Initialize()
    {
        var host = Host.CreateDefaultBuilder()
            .UseEnvironment("Development")
            .ConfigureAppConfiguration((context, config) =>
            {
                var env = context.HostingEnvironment;

                config
                    .AddJsonFile("appsettings.test.json", false, true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                    .AddEnvironmentVariables();
            })
            .ConfigureServices(s =>
            {
                s.AddTransient<IDatabaseConfiguration, TestDatabaseConfiguration>();
                s.AddTransient<IChurchBulletinItemByDateHandler, ChurchBulletinItemByDateHandler>();
                s.AddTransient<ChurchBulletinItemByDateHandler>();
                s.AddScoped<DbContext, DataContext>();
                s.AddDbContextFactory<DataContext>();
                s.AddDbContextFactory<DbContext>();
            })
            .Build();


        _host = host;
        
        ConfigureBogus();
    }

    private static void EnsureDependenciesRegistered()
    {
        if (!_dependenciesRegistered)
            lock (Lock)
            {
                if (!_dependenciesRegistered)
                {
                    Initialize();
                    _dependenciesRegistered = true;
                }
            }
    }

    public static DataContext NewDbContext()
    {
        return TestHost.GetRequiredService<DataContext>();
    }

    public static TK Faker<TK>()
    {
        EnsureDependenciesRegistered();
        return AutoFaker.Generate<TK>();
    }

    private static void ConfigureBogus()
    {
        AutoFaker.Configure(builder =>
        {
            builder.WithConventions()
                .WithSkip<WorkOrder>(wo => wo.AuditEntries)
                .WithOverride(new DefaultOverrides());
        });
    }
}

internal class DefaultOverrides : AutoGeneratorOverride
{
    public override bool CanOverride(AutoGenerateContext context)
    {
        return true;
    }

    public override void Generate(AutoGenerateOverrideContext context)
    {
        switch (context.Instance)
        {
            case WorkOrder order:
                order.Description = order.Description.ClampLength(1, 2000);
                order.Number = order.Number.ClampLength(1, 5);
                // order.Status = context.Faker.PickRandom<WorkOrderStatus>(WorkOrderStatus.GetAllItems());
                break;
            case WorkOrderStatus:
                context.Instance = context.Faker.PickRandom<WorkOrderStatus>(WorkOrderStatus.GetAllItems());
                break;
        }
    }
}