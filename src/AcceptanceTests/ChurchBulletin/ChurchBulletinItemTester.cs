namespace ClearMeasure.Bootcamp.AcceptanceTests.ChurchBulletin;

[TestFixture]
public class ChurchBulletinItemTester : AcceptanceTestBase
{
    [Test]
    public async Task ShouldDisplayFriendlyPlaceInBulletinTable()
    {
        await Page.GotoAsync("/fetchchurchbulletin");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Assert: Table contains @ Sanctuary in the Place column
        var placeCell = Page.Locator("#bulletinTable td", new PageLocatorOptions { HasText = "@ Sanctuary" });
        await Expect(placeCell).ToBeVisibleAsync();
    }
}