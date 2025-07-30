using ClearMeasure.Bootcamp.UI.Shared;
using ClearMeasure.Bootcamp.UI.Shared.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace ClearMeasure.Bootcamp.AcceptanceTests.Authentication;

public class LogoutTests : AcceptanceTestBase
{
    [SetUp]
    public async Task Setup()
    {
        await LoginAsCurrentUser();
    }

    protected override bool? Headless { get; set; } = false;

    [Test]
    public async Task ShouldLogout()
    {
        (await Page.GetByTestId(nameof(NavMenu.Elements.NewWorkOrder)).IsVisibleAsync()).ShouldBe(true);

        await Page.GetByTestId(nameof(Logout.Elements.LogoutLink)).ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        (await Page.GetByTestId(nameof(Login.Elements.LoginLink)).IsVisibleAsync()).ShouldBe(true);
        (await Page.GetByTestId(nameof(NavMenu.Elements.NewWorkOrder)).IsVisibleAsync()).ShouldBe(false);
    }
}