using ClearMeasure.Bootcamp.Core;
using ClearMeasure.Bootcamp.Core.Model.StateCommands;
using ClearMeasure.Bootcamp.IntegrationTests;
using ClearMeasure.Bootcamp.UI.Shared;
using ClearMeasure.Bootcamp.UI.Shared.Pages;

namespace ClearMeasure.Bootcamp.AcceptanceTests;

public abstract class AcceptanceTestBase : PageTest
{
    public Employee CurrentUser { get; set; }
    protected virtual bool? Headless { get; set; } = true;
    protected new IPage Page { get; private set; }
    public IBus Bus => TestHost.GetRequiredService<IBus>();


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
        context.SetDefaultTimeout(10_000);
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
        var welcomeText = Page.Locator($"text=Welcome {username}!");
        if (await welcomeText.IsVisibleAsync())
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
        await Expect(welcomeText).ToBeVisibleAsync();
        await welcomeText.DblClickAsync(); // causes the browser to finish DOM loading - HACK
    }

    protected Task Click(string buttonTestId)
    {
        return Page.GetByTestId(buttonTestId).ClickAsync();
    }

    protected async Task Input(string elementTestId, string? value)
    {
        await Page.GetByTestId(elementTestId).FillAsync(value ?? "");
    }
    protected async Task Select(string elementTestId, string? value)
    {
        await Page.GetByTestId(elementTestId).SelectOptionAsync(value ?? "");
    }

    protected async Task<WorkOrder> CreateAndSaveNewWorkOrder()
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
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        return order;
    }
}