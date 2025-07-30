using System.CodeDom;
using AutoBogus;
using AutoBogus.Conventions;
using ClearMeasure.Bootcamp.Core;
using ClearMeasure.Bootcamp.Core.Queries;
using ClearMeasure.Bootcamp.DataAccess.Handlers;
using ClearMeasure.Bootcamp.DataAccess.Mappings;
using ClearMeasure.Bootcamp.UnitTests;
using ClearMeasure.Bootcamp.Core.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ClearMeasure.Bootcamp.IntegrationTests;

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
        return ObjectMother.Faker<TK>();
    }
}