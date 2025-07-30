using ProgrammingWithPalermo.ChurchBulletin.Core.Model;

namespace ProgrammingWithPalermo.ChurchBulletin.AcceptanceTests.Authentication;

[TestFixture]
public class LoginTests : AcceptanceTestBase
{
    [Test]
    public void VerifySetup()
    {
        var homer = TestHost.NewDbContext().Set<Employee>().Single(employee =>
            employee.UserName == "hsimpson");

        homer.ShouldNotBeNull();
    }
    
    [Test, Repeat(2)]
    public async Task LoginWithUsernameOnlyForwardsToHomePage()
    {
        // Act: Go to home page
        await Page.GotoAsync("/");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await TakeScreenshotAsync(1);

        var logoutLink = Page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Logout" });
        if (await logoutLink.CountAsync() > 0)
        {
            await logoutLink.ClickAsync();
            await Page.WaitForURLAsync("**/");
        }

        // Click Login link in top bar
        var loginLink = Page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Login" });
        await loginLink.ClickAsync();
        await Page.WaitForURLAsync("**/login");
        await TakeScreenshotAsync(2);

        // Fill in username only
        await Page.SelectOptionAsync("#employee", "hsimpson");
        await TakeScreenshotAsync(3);

        // Submit form
        var loginButton = Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Login" });
        await loginButton.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await TakeScreenshotAsync(4);

        // Assert: Should be redirected to home and see welcome message
        await Expect(Page.Locator("text=Welcome hsimpson!")).ToBeVisibleAsync();
    }
}