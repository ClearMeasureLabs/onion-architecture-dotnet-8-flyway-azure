using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System.Threading.Tasks;

namespace ProgrammingWithPalermo.ChurchBulletin.AcceptanceTests.Counter
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class CounterIncrementsTester : PageTest
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
        }

        [TearDown]
        public async Task TearDownAsync()
        {
            await Context.Tracing.StopAsync(new()
            {
                Path = Path.Combine(TestContext.CurrentContext.WorkDirectory, "playwright-traces", $"{TestContext.CurrentContext.Test.ClassName}.{TestContext.CurrentContext.Test.Name}.zip")
            });
        }

        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(5, 5)]
        [TestCase(9, 9)]
        public async Task ShouldIncrementOnPress(int numberOfButtonPresses, int expectedFinalCount)
        {
            await Page.GotoAsync("/counter");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var button = Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Click me" });
            var status = Page.GetByRole(AriaRole.Status);

            await Expect(status).ToContainTextAsync("0");

            for (int i = 0; i < numberOfButtonPresses; i++)
            {
                await button.ClickAsync();
            }

            await Expect(status).ToContainTextAsync($"{expectedFinalCount}");
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