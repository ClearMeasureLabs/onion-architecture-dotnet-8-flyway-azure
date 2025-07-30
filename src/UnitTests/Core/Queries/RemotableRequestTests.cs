using MediatR;
using ProgrammingWithPalermo.ChurchBulletin.Core;
using ProgrammingWithPalermo.ChurchBulletin.Core.Model;
using ProgrammingWithPalermo.ChurchBulletin.Core.Queries;
using Shouldly;
using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using UI.Client;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ProgrammingWithPalermo.ChurchBulletin.UnitTests.Core.Queries;

public class RemotableRequestTests
{
    [Test]
    public void ShouldBeRemotableCompatible()
    {
        AssertRemotable(new ForecastQuery());
        AssertRemotable(new WeatherForecast[1]{ObjectMother.Faker<WeatherForecast>()});
        AssertRemotable(new HealthCheckRemotableRequest());
        AssertRemotable(HealthStatus.Degraded);
        AssertRemotable(ObjectMother.Faker<WorkOrderSpecificationQuery>());
        AssertRemotable(new ServerHealthCheckQuery());
    }

    private void AssertRemotable(object theObject)
    {
        var json = new WebServiceMessage(theObject).GetJson();
        var message = JsonSerializer.Deserialize<WebServiceMessage>(json);
        var rehydratedQuery = message!.GetBodyObject();

        ObjectMother.AssertAllProperties(theObject, rehydratedQuery);
        rehydratedQuery.ShouldBe(theObject);
    }
}