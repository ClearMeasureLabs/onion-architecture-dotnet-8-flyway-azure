namespace ProgrammingWithPalermo.ChurchBulletin.AcceptanceTests.ChurchBulletin;

[TestFixture]
public class ChurchBulletinTester : AcceptanceTestBase
{
    [Test]
    public async Task ShouldLoadChurchBulletin()
    {
        await Page.GotoAsync("/fetchchurchbulletin");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await TakeScreenshotAsync(10, "Arrange");

        await TakeScreenshotAsync(20, "Act");
        await Page.WaitForTimeoutAsync(10000);
        
        await TakeScreenshotAsync(30, "Assert");
        await Expect(Page.Locator("h1")).ToContainTextAsync("Church Bulletin",
            new LocatorAssertionsToContainTextOptions { Timeout = 10000 });
    }
}