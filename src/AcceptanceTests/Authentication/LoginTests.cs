namespace ProgrammingWithPalermo.ChurchBulletin.AcceptanceTests.Authentication;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class LoginTests : PageTest
{
    [SetUp]
    public async Task SetUpAsync()
    {
        new ZDataLoader().PopulateDatabase();
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

    [Test]
    public void VerifySetup()
    {
        var homer = TestHost.NewDbContext().Set<Employee>().Single(employee =>
            employee.UserName == "hsimpson");

        homer.ShouldNotBeNull();
    }

    [Test]
    public async Task LoginWithUsernameOnlyForwardsToHomePage()
    {
        // Act: Go to home page
        await Page.GotoAsync("/");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Click Login link in top bar
        var loginLink = Page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Login" });
        await loginLink.ClickAsync();
        await Page.WaitForURLAsync("**/login");

        // Fill in username only
        await Page.SelectOptionAsync("#employee", "hsimpson");

        // Submit form
        var loginButton = Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Login" });
        await loginButton.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Assert: Should be redirected to home and see welcome message
        await Expect(Page.Locator("text=Welcome hsimpson!")).ToBeVisibleAsync();
    }

    public override BrowserNewContextOptions ContextOptions()
    {
        return new BrowserNewContextOptions
        {
            BaseURL = ServerFixture.ApplicationLocalBaseURL
        };
    }
}