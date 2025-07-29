using Microsoft.Extensions.Diagnostics.HealthChecks;
using ProgrammingWithPalermo.ChurchBulletin.AcceptanceTests;
using Shouldly;
using UI.Client.Pages;
using UI.Shared.Components;

namespace ProgrammingWithPalermo.ChurchBulletin.AcceptanceTests.App;

[TestFixture]
public class HealthCheckLinkPlaywrightTests : AcceptanceTestBase
{
    [Test]
    public async Task Should_NavigateToHealthCheck_WhenGearIconClicked()
    {
        // Arrange
        await Page.GotoAsync("/");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Act
        await Page.ClickAsync($"#{HealthCheckLink.Elements.HealthCheckLink}");

        // Assert
        await Page.WaitForURLAsync("**/_clienthealthcheck");
        Page.Url.ShouldContain("/_clienthealthcheck");
        
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        var statusSpan = Page.GetByTestId(nameof(ClientHealthCheck.Elements.Status));
        var innerTextAsync = await statusSpan.InnerTextAsync(); 
        innerTextAsync.ShouldBe(nameof(HealthStatus.Healthy));
    }
}