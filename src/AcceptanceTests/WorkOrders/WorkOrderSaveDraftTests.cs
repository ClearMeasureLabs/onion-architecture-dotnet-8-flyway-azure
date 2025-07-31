using ClearMeasure.Bootcamp.Core.Model;
using ClearMeasure.Bootcamp.Core.Model.StateCommands;
using ClearMeasure.Bootcamp.UI.Shared;
using ClearMeasure.Bootcamp.UI.Shared.Pages;

namespace ClearMeasure.Bootcamp.AcceptanceTests.WorkOrders;

public class WorkOrderSaveDraftTests : AcceptanceTestBase
{
    [Test]
    public async Task ShouldLoadScreenForNewWorkOrder()
    {
        await LoginAsCurrentUser();
        await Page.GetByTestId(nameof(NavMenu.Elements.NewWorkOrder)).ClickAsync();
        await Page.WaitForURLAsync("**/workorder/manage?mode=New");
    }

    protected override bool? Headless { get; set; } = false;

    [Test]
    public async Task ShouldCreateNewWorkOrderAndVerifyOnSearchScreen()
    {
        await LoginAsCurrentUser();

        WorkOrder order = await CreateAndSaveNewWorkOrder();
        
        await Page.WaitForURLAsync("**/workorder/search");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await TakeScreenshotAsync(3, "WorkOrderSearchAfterSave");

        await Click(nameof(WorkOrderSearch.Elements.WorkOrderLink) + order.Number);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        Page.Url.ShouldContain($"/workorder/manage/{order.Number}");
        await TakeScreenshotAsync(5, "WorkOrderManagePage");

        (await Page.GetByTestId(nameof(WorkOrderManage.Elements.WorkOrderNumber)).InnerTextAsync())
            .ShouldBe(order.Number);
        var titleField = await Page.GetByTestId(nameof(WorkOrderManage.Elements.Title)).InputValueAsync();
        var descriptionField = await Page.GetByTestId(nameof(WorkOrderManage.Elements.Description)).InputValueAsync();
        var roomNumberField = await Page.GetByTestId(nameof(WorkOrderManage.Elements.RoomNumber)).InputValueAsync();

        titleField.ShouldBe(order.Title);
        descriptionField.ShouldBe(order.Description);
        roomNumberField.ShouldBe(order.RoomNumber);
    }

    private async Task<WorkOrder> CreateAndSaveNewWorkOrder()
    {
        var order = Faker<WorkOrder>();
        order.Number = null;
        var testTitle = order.Title;
        var testDescription = order.Description;
        var testRoomNumber = order.RoomNumber;

        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await Click(nameof(NavMenu.Elements.NewWorkOrder));
        await Page.WaitForURLAsync("**/workorder/manage?mode=New");
        await TakeScreenshotAsync(1, "NewWorkOrderPage");

        var newWorkOrderNumber = await Page.GetByTestId(nameof(WorkOrderManage.Elements.WorkOrderNumber)).InnerTextAsync();
        order.Number = newWorkOrderNumber;
        await Input(nameof(WorkOrderManage.Elements.Title), testTitle);
        await Input(nameof(WorkOrderManage.Elements.Description), testDescription);
        await Input(nameof(WorkOrderManage.Elements.RoomNumber), testRoomNumber);
        await TakeScreenshotAsync(2, "FormFilled");

        var saveButtonTestId = nameof(WorkOrderManage.Elements.CommandButton) + SaveDraftCommand.Name;
        await Click(saveButtonTestId);

        return order;
    }
}