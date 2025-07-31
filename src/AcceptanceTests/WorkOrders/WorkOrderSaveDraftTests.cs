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

    [Test]
    public async Task ShouldCreateNewWorkOrderAndVerifyOnSearchScreen()
    {
        await LoginAsCurrentUser();

        var order = await CreateAndSaveNewWorkOrder();

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

    [Test]
    public async Task ShouldAssignEmployeeAndSave()
    {
        await LoginAsCurrentUser();

        var order = await CreateAndSaveNewWorkOrder();

        await Click(nameof(WorkOrderSearch.Elements.WorkOrderLink) + order.Number);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var woNumberLocator = Page.GetByTestId(nameof(WorkOrderManage.Elements.WorkOrderNumber));
        await woNumberLocator.WaitForAsync();
        (await woNumberLocator.InnerTextAsync())
            .ShouldBe(order.Number);
        await Select(nameof(WorkOrderManage.Elements.Assignee), CurrentUser.UserName);
        await Input(nameof(WorkOrderManage.Elements.Title), "newtitle");
        await Input(nameof(WorkOrderManage.Elements.Description), "newdesc");
        await Click(nameof(WorkOrderManage.Elements.CommandButton) + SaveDraftCommand.Name);

        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await Click(nameof(WorkOrderSearch.Elements.WorkOrderLink) + order.Number);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await woNumberLocator.WaitForAsync();
        (await woNumberLocator.InnerTextAsync()).ShouldBe(order.Number);

        (await Page.GetByTestId(nameof(WorkOrderManage.Elements.Title)).InputValueAsync()).ShouldBe("newtitle");
        (await Page.GetByTestId(nameof(WorkOrderManage.Elements.Description)).InputValueAsync()).ShouldBe("newdesc");
        (await Page.GetByTestId(nameof(WorkOrderManage.Elements.Assignee)).InputValueAsync()).ShouldBe(CurrentUser
            .UserName);
    }
}