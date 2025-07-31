namespace ClearMeasure.Bootcamp.AcceptanceTests.Counter;

[TestFixture]
public class CounterPageTests : AcceptanceTestBase
{
    protected override bool LoadDataOnSetup { get; set; } = false;

    [TestCase(1, 1)]
    [TestCase(2, 2)]
    public async Task ShouldIncrementOnClick(int numberOfClicks, int expectedCount)
    {
        await Page.GotoAsync("/counter");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var button = Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Click me" });
        var status = Page.GetByRole(AriaRole.Status);

        await TakeScreenshotAsync(10, "Arrange");

        for (var i = 0; i < numberOfClicks; i++)
        {
            await button.ClickAsync();
            await TakeScreenshotAsync(20 + i, "Act");
        }

        await TakeScreenshotAsync(30, "Assert");
        await Expect(status).ToContainTextAsync($"{expectedCount}");
    }

    protected override IBrowserType GetBrowserTypeInstance(IPlaywright playwright)
    {
        return playwright.Firefox;
    }
}