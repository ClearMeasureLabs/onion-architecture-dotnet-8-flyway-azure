using Microsoft.AspNetCore.Components.Forms;
using NUnit.Framework;
using Shouldly;
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
    }
}