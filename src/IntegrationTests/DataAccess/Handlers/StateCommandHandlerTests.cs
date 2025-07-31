using ClearMeasure.Bootcamp.Core.Model;
using ClearMeasure.Bootcamp.Core.Model.StateCommands;
using ClearMeasure.Bootcamp.Core.Services;
using ClearMeasure.Bootcamp.DataAccess.Handlers;
using Shouldly;

namespace ClearMeasure.Bootcamp.IntegrationTests.DataAccess.Handlers;

public class StateCommandHandlerTests : IntegratedTestBase
{
    [Test]
    public async Task ShouldSaveWorkOrderAfterSavingDraft()
    {
        var workOrder = Faker<WorkOrder>();
        workOrder.CreatedDate = null; // Ensure CreatedDate is null to test setting it;
        var currentUser = Faker<Employee>();
        var command = new SaveDraftCommand(workOrder, currentUser);
        
        var handler = TestHost.GetRequiredService<StateCommandHandler>();
        
        var result = await handler.Handle(command);
        
        result.Command.ShouldBe(command);
        result.Order.ShouldBe(workOrder);
        result.Order.CreatedDate.ShouldBe(TestHost.TestTime.DateTime);
    }
}