using ClearMeasure.Bootcamp.Core.Model;
using ClearMeasure.Bootcamp.Core.Model.StateCommands;
using ClearMeasure.Bootcamp.DataAccess.Handlers;
using Microsoft.EntityFrameworkCore;
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

        result.TransitionVerbPresentTense.ShouldBe(command.TransitionVerbPresentTense);
        result.WorkOrder.ShouldBe(workOrder);
        result.WorkOrder.CreatedDate.ShouldBe(TestHost.TestTime.DateTime);
    }

    [Test]
    public async Task ShouldSaveWorkOrderWithAssigneeAndCreator()
    {
        new DatabaseTester().Clean();

        var workOrder = Faker<WorkOrder>();
        var currentUser = Faker<Employee>();
        await using (var context = TestHost.GetRequiredService<DbContext>())
        {
            context.Add(currentUser);
            context.Add(workOrder);
            await context.SaveChangesAsync();
        }

        Employee? assignee;
        await using (var context2 = TestHost.GetRequiredService<DbContext>())
        {
            assignee = context2.Find<Employee>(currentUser.Id);
        }

        workOrder.Creator = currentUser;
        workOrder.Assignee = assignee;

        var command = new SaveDraftCommand(workOrder, currentUser);

        var handler = TestHost.GetRequiredService<StateCommandHandler>();

        var result = await handler.Handle(command);
    }
}