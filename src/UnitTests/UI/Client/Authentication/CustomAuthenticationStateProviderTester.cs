using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using NUnit.Framework;
using Shouldly;
using UI.Shared.Authentication;

namespace UnitTests.UI.Client.Authentication
{
    [TestFixture]
    public class CustomAuthenticationStateProviderTester
    {
        [Test, Ignore("Temporarily hardcoded identity")]
        public async Task ShouldReturnUnauthenticatedUserWhenNotLoggedIn()
        {
            var authProvider = new CustomAuthenticationStateProvider();
            var authState = await authProvider.GetAuthenticationStateAsync();
            authState.User.Identity!.IsAuthenticated.ShouldBeFalse();
        }

        [Test]
        public async Task ShouldReturnAuthenticatedUserAfterLogin()
        {
            var authProvider = new CustomAuthenticationStateProvider();
            const string username = "hsimpson";
            authProvider.Login(username);
            var authState = await authProvider.GetAuthenticationStateAsync();
            authState.User.Identity!.IsAuthenticated.ShouldBeTrue();
            authState.User.Identity.Name.ShouldBe(username);
        }

        [Test]
        public async Task ShouldReturnUnauthenticatedUserAfterLogout()
        {
            var authProvider = new CustomAuthenticationStateProvider();
            authProvider.Login("hsimpson");
            authProvider.Logout();
            var authState = await authProvider.GetAuthenticationStateAsync();
            authState.User.Identity!.IsAuthenticated.ShouldBeFalse();
        }
    }
}