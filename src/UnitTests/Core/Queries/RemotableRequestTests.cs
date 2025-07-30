using MediatR;
using Shouldly;
using System.Text.Json;
using ClearMeasure.Bootcamp.Core;
using ClearMeasure.Bootcamp.Core.Model;
using ClearMeasure.Bootcamp.Core.Queries;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ClearMeasure.Bootcamp.UI.Client;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ClearMeasure.Bootcamp.UnitTests.Core.Queries;

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
        AssertRemotable(WorkOrderStatus.Draft);
        AssertRemotable(ObjectMother.Faker<WorkOrder>());
        AssertRemotable(ObjectMother.Faker<Employee>());
        AssertRemotable(new ServerHealthCheckQuery());
    }

    public static void AssertRemotable(object theObject)
    {
        var json = new WebServiceMessage(theObject).GetJson();
        var message = JsonSerializer.Deserialize<WebServiceMessage>(json);
        var rehydratedQuery = message!.GetBodyObject();

        ObjectMother.AssertAllProperties(theObject, rehydratedQuery);
        rehydratedQuery.ShouldBe(theObject);
    }
}