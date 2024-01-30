﻿using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace ProgrammingWithPalermo.ChurchBulletin.AcceptanceTests
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class CounterPageTests : PageTest
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

        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(5, 5)]
        [TestCase(9, 9)]
        public async Task ShouldIncrementOnClick(int numberOfClicks, int expectedCount)
        {
            // Arrange
            await Page.GotoAsync($"/counter");

            var button = Page.GetByRole(AriaRole.Button, new() { Name = "Click me" });

            // Act
            for (int i = 0; i < numberOfClicks; i++)
            {
                await button.ClickAsync();
            }

            // Assert
            var totalCount = Page.GetByRole(AriaRole.Status);

            await Expect(totalCount).ToContainTextAsync($"{expectedCount}");
        }

        [TearDown]
        public async Task TearDownAsync()
        {
            await Context.Tracing.StopAsync(new()
            {
                Path = Path.Combine(TestContext.CurrentContext.WorkDirectory, "playwright-traces", $"{TestContext.CurrentContext.Test.ClassName}.{TestContext.CurrentContext.Test.Name}.zip")
            });
        }

        public override BrowserNewContextOptions ContextOptions()
        {
            return new BrowserNewContextOptions()
            {
                BaseURL = $"https://{Environment.GetEnvironmentVariable("containerAppURL", EnvironmentVariableTarget.User)}"
            };
        }
    }
}
