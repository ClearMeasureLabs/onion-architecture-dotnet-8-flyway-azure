using ClearMeasure.Bootcamp.Core.Model;
using ClearMeasure.Bootcamp.UI.Shared;
using ClearMeasure.Bootcamp.UI.Shared.Pages;

namespace ClearMeasure.Bootcamp.AcceptanceTests.WorkOrders;

public class WorkOrderManageTests : AcceptanceTestBase
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
        
        var testTitle = "Test Work Order Title";
        var testDescription = "Test work order description for automation";
        var testRoomNumber = "Room-101";

        await Page.GetByTestId(nameof(NavMenu.Elements.NewWorkOrder)).ClickAsync();
        await Page.WaitForURLAsync("**/workorder/manage?mode=New");
        await TakeScreenshotAsync(1, "NewWorkOrderPage");

        var newWorkOrderNumber = await Page.GetByTestId(nameof(WorkOrderManage.Elements.WorkOrderNumber)).InnerTextAsync();
        await Page.GetByTestId(nameof(WorkOrderManage.Elements.Title)).FillAsync(testTitle);
        await Page.GetByTestId(nameof(WorkOrderManage.Elements.Description)).FillAsync(testDescription);
        await Page.GetByTestId(nameof(WorkOrderManage.Elements.RoomNumber)).FillAsync(testRoomNumber);
        await TakeScreenshotAsync(2, "FormFilled");

        var saveButtonTestId = nameof(WorkOrderManage.Elements.CommandButton) + "Save";
        await Page.GetByTestId(saveButtonTestId).ClickAsync();
        await Page.WaitForURLAsync("**/workorder/search");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await TakeScreenshotAsync(3, "WorkOrderSearchAfterSave");

        await Page.GetByTestId(nameof(WorkOrderSearch.Elements.WorkOrderLink) + newWorkOrderNumber).ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        Page.Url.ShouldContain($"/workorder/manage/{newWorkOrderNumber}");
        await TakeScreenshotAsync(5, "WorkOrderManagePage");

        (await Page.GetByTestId(nameof(WorkOrderManage.Elements.WorkOrderNumber)).InnerTextAsync())
            .ShouldBe(newWorkOrderNumber);
        var titleField = await Page.GetByTestId(nameof(WorkOrderManage.Elements.Title)).InputValueAsync();
        var descriptionField = await Page.GetByTestId(nameof(WorkOrderManage.Elements.Description)).InputValueAsync();
        var roomNumberField = await Page.GetByTestId(nameof(WorkOrderManage.Elements.RoomNumber)).InputValueAsync();

        titleField.ShouldBe(testTitle);
        descriptionField.ShouldBe(testDescription);
        roomNumberField.ShouldBe(testRoomNumber);
    }
}