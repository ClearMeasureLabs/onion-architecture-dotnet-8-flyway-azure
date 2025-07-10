using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System.Threading.Tasks;

namespace ProgrammingWithPalermo.ChurchBulletin.AcceptanceTests.ChurchBulletin
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class ChurchBulletinItemTester : PageTest
    {
        [SetUp]
        public async Task SetUpAsync()
        {
            await Context.Tracing.StartAsync(new()
            {
                Title = $"{TestContext.CurrentContext.Test.ClassName}.{TestContext.CurrentContext.Test.Name}",
                Screenshots = true,
                Snapshots = true,
                Sources = true
            });
            // Seed the database with known bulletin items

            new ProgrammingWithPalermo.ChurchBulletin.IntegrationTests.ZDataLoader().LoadData();
        }

        [TearDown]
        public async Task TearDownAsync()
        {
            await Context.Tracing.StopAsync(new()
            {
                Path = Path.Combine(TestContext.CurrentContext.WorkDirectory, "playwright-traces", $"{TestContext.CurrentContext.Test.ClassName}.{TestContext.CurrentContext.Test.Name}.zip")
            });
        }

        [Test]
        public async Task ShouldDisplayFriendlyPlaceInBulletinTable()
        {
            await Page.GotoAsync("/fetchchurchbulletin");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Assert: Table contains @ Sanctuary in the Place column
            var placeCell = Page.Locator("#bulletinTable td", new PageLocatorOptions { HasText = "@ Sanctuary" });
            await Expect(placeCell).ToBeVisibleAsync();
        }

        public override BrowserNewContextOptions ContextOptions()
        {
            return new BrowserNewContextOptions
            {
                BaseURL = ServerFixture.ApplicationLocalBaseURL
            };
        }
    }
}
