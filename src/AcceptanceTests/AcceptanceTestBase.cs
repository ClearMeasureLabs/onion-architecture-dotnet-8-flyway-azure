using ClearMeasure.Bootcamp.IntegrationTests;

namespace ClearMeasure.Bootcamp.AcceptanceTests;

public abstract class AcceptanceTestBase : PageTest
{
    public Employee CurrentUser { get; set; }
    protected virtual bool? Headless { get; set; } = true;
    protected new IPage Page { get; private set; }


    [SetUp]
    public async Task SetUpAsync()
    {
        new ZDataLoader().LoadData();
        CurrentUser = new ZDataLoader().CreateUser();
        await Context.Tracing.StartAsync(new TracingStartOptions
        {
            Title = $"{TestContext.CurrentContext.Test.ClassName}.{TestContext.CurrentContext.Test.Name}",
            Screenshots = true,
            Snapshots = true,
            Sources = true
        });

        var playwright = Playwright;
        var browser = await GetBrowserTypeInstance(playwright).LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = Headless
        });

        var context = await browser.NewContextAsync(ContextOptions());
        Page = await context.NewPageAsync().ConfigureAwait(false);
    }

    protected virtual IBrowserType GetBrowserTypeInstance(IPlaywright playwright)
    {
        return playwright.Chromium;
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

    public override BrowserNewContextOptions ContextOptions()
    {
        return new BrowserNewContextOptions
        {
            BaseURL = ServerFixture.ApplicationLocalBaseURL,
        };
    }

    protected async Task TakeScreenshotAsync(int stepNumber, string? stepName = null)
    {
        var test = TestContext.CurrentContext.Test;
        var testName = test.ClassName + "-" + test.Name;
        var fileName = $"{testName}-{stepNumber}{stepName}.png";
        await Page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = fileName
        });
        TestContext.AddTestAttachment(Path.GetFullPath(fileName));
    }
    
    protected TK Faker<TK>()
    {
        return TestHost.Faker<TK>();
    }

    protected async Task LoginAsCurrentUser()
    {
        var username = CurrentUser.UserName;
        if (await Page.Locator($"text=Welcome {username}!").IsVisibleAsync())
        {
            return;
        }

        await Page.GotoAsync("/login");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Fill in username only
        await Page.SelectOptionAsync("#employee", username);

        // Submit form
        var loginButton = Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Login" });
        await loginButton.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Assert: Should be redirected to home and see welcome message
        await Expect(Page.Locator($"text=Welcome {username}!")).ToBeVisibleAsync();
    }
}