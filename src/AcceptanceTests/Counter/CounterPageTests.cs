namespace ProgrammingWithPalermo.ChurchBulletin.AcceptanceTests.Counter;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class CounterPageTests : PageTest
{
    [SetUp]
    public async Task SetUpAsync()
    {
        await Context.Tracing.StartAsync(new TracingStartOptions
        {
            Title = $"{TestContext.CurrentContext.Test.ClassName}.{TestContext.CurrentContext.Test.Name}",
            Screenshots = true,
            Snapshots = true,
            Sources = true
        });
    }

    [TearDown]
    public async Task TearDownAsync()
    {
        await Context.Tracing.StopAsync(new TracingStopOptions
        {
            Path = Path.Combine(TestContext.CurrentContext.WorkDirectory, "playwright-traces",
                $"{TestContext.CurrentContext.Test.ClassName}.{TestContext.CurrentContext.Test.Name}.zip")
        });
    }

    [TestCase(1, 1)]
    [TestCase(2, 2)]
    [TestCase(5, 5)]
    [TestCase(9, 9)]
    public async Task ShouldIncrementOnClick(int numberOfClicks, int expectedCount)
    {
        await Page.GotoAsync("/counter");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var button = Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Click me" });
        var status = Page.GetByRole(AriaRole.Status);

        await TakeScreenshotAsync(10, TestContext.CurrentContext.Test.Name, "Arrange");

        for (var i = 0; i < numberOfClicks; i++)
        {
            await button.ClickAsync();
            await TakeScreenshotAsync(20 + i, TestContext.CurrentContext.Test.Name, "Act");
        }

        await TakeScreenshotAsync(30, TestContext.CurrentContext.Test.Name, "Assert");
        await Expect(status).ToContainTextAsync($"{expectedCount}");
    }

    public override BrowserNewContextOptions ContextOptions()
    {
        return new BrowserNewContextOptions
        {
            BaseURL = ServerFixture.ApplicationLocalBaseURL
        };
    }

    private async Task TakeScreenshotAsync(int stepNumber, string testName, string stepName)
    {
        var fileName = $"{testName}-{stepNumber}-{stepName}.png";
        await Page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = fileName
        });
        TestContext.AddTestAttachment(Path.GetFullPath(fileName));
    }
}