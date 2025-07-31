using ClearMeasure.Bootcamp.UI.Shared;
using ClearMeasure.Bootcamp.UI.Shared.Components;

namespace ClearMeasure.Bootcamp.AcceptanceTests.Authentication;

public class LogoutTests : AcceptanceTestBase
{
    [SetUp]
    public async Task Setup()
    {
        await LoginAsCurrentUser();
    }

    [Test]
    public async Task ShouldLogout()
    {
        var newLink = Page.GetByTestId(nameof(NavMenu.Elements.NewWorkOrder));
        (await newLink.IsVisibleAsync()).ShouldBe(true);

        await Page.GetByTestId(nameof(Logout.Elements.LogoutLink)).ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        (await Page.GetByTestId(nameof(Login.Elements.LoginLink)).IsVisibleAsync()).ShouldBe(true);
        (await Page.GetByTestId(nameof(NavMenu.Elements.NewWorkOrder)).IsVisibleAsync()).ShouldBe(false);
    }
}