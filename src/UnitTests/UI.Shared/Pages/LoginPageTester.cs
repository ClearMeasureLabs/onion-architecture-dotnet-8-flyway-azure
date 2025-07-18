using System.ComponentModel.DataAnnotations;
using Bunit;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Shouldly;
using UI.Shared.Authentication;
using UI.Shared.Pages;

namespace UnitTests.UI.Shared.Pages
{
    [TestFixture]
    public class LoginPageTester
    {
        [Test]
        public void ShouldOnlyRequireUsername()
        {
            var loginPage = new Login();
            var loginModel = new Login.LoginModel { Username = "hsimpson" };

            var validationContext = new ValidationContext(loginModel);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(loginModel, validationContext, validationResults, true);

            isValid.ShouldBeTrue();
            validationResults.ShouldBeEmpty();
        }

        [Test]
        public void ShouldRequireUsername()
        {
            var loginPage = new Login();
            var loginModel = new Login.LoginModel { Username = "" };

            var validationContext = new ValidationContext(loginModel);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(loginModel, validationContext, validationResults, true);

            isValid.ShouldBeFalse();
            validationResults.ShouldContain(r => r.MemberNames.Contains("Username"));
        }

        [Test]
        public void ShouldSuccessfullyLogInWithHsimpson()
        {
            using var ctx = new Bunit.TestContext();
            
            var authStateProvider = new CustomAuthenticationStateProvider();
            ctx.Services.AddSingleton(authStateProvider);
            ctx.Services.AddSingleton<Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider>(authStateProvider);
            
            // Mock the IUiBus dependency
            var mockUiBus = new StubUiBus();
            ctx.Services.AddSingleton<Palermo.BlazorMvc.IUiBus>(mockUiBus);
            
            var component = ctx.RenderComponent<Login>();
            
            var usernameInput = component.Find("#username");
            var submitButton = component.Find("button[type='submit']");
            
            usernameInput.Change("hsimpson");
            submitButton.Click();
            
            authStateProvider.IsAuthenticated().ShouldBeTrue();
            authStateProvider.GetUsername().ShouldBe("hsimpson");
        }
        
        private class StubUiBus : Palermo.BlazorMvc.IUiBus
        {
            public void Notify(object eventObject)
            {
                // Mock implementation - do nothing
            }
            
            public void Register(Palermo.BlazorMvc.IListener listener)
            {
                // Mock implementation - do nothing
            }
            
            public void UnRegister(Palermo.BlazorMvc.IListener listener)
            {
                // Mock implementation - do nothing
            }
            
            public Palermo.BlazorMvc.IListener<T>[] GetListeners<T>() where T : Palermo.BlazorMvc.IUiBusEvent
            {
                return Array.Empty<Palermo.BlazorMvc.IListener<T>>();
            }
            
            public void Notify<T>(T eventObject) where T : Palermo.BlazorMvc.IUiBusEvent
            {
                // Mock implementation - do nothing
            }
            
            public void UnRegisterAll()
            {
                // Mock implementation - do nothing
            }
        }
    }
}