using ClearMeasure.Bootcamp.UI.Shared;

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
}