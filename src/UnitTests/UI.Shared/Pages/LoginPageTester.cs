using System.ComponentModel.DataAnnotations;
using Bunit;
using Core.Model;
using Core.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Palermo.BlazorMvc;
using ProgrammingWithPalermo.ChurchBulletin.Core.Model;
using Shouldly;
using UI.Shared.Authentication;
using UI.Shared.Pages;
using TestContext = Bunit.TestContext;

namespace ProgrammingWithPalermo.ChurchBulletin.UnitTests.UI.Shared.Pages;

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
    public void ShouldDisplayDropdownWithEmployees()
    {
        using var ctx = new TestContext();

        var provider = new CustomAuthenticationStateProvider();
        ctx.Services.AddSingleton(provider);
        ctx.Services.AddSingleton<AuthenticationStateProvider>(provider);
        ctx.Services.AddSingleton<IEmployeeRepository>(new MockEmployeeRepository());
        ctx.Services.AddSingleton<IUiBus>(new StubUiBus());

        var component = ctx.RenderComponent<Login>();

        var employeeSelect = component.Find("#employee");
        employeeSelect.ShouldNotBeNull();

        var options = component.FindAll("option");
        options.Count.ShouldBe(4);
    }

    [Test]
    public void ShouldLoginWithSelectedEmployee()
    {
        using var ctx = new TestContext();

        var provider = new CustomAuthenticationStateProvider();
        ctx.Services.AddSingleton(provider);
        ctx.Services.AddSingleton<AuthenticationStateProvider>(provider);
        ctx.Services.AddSingleton<IEmployeeRepository>(new MockEmployeeRepository());
        ctx.Services.AddSingleton<IUiBus>(new StubUiBus());

        var component = ctx.RenderComponent<Login>();

        var employeeSelect = component.Find("#employee");
        var submitButton = component.Find("button[type='submit']");

        employeeSelect.Change("hsimpson");
        submitButton.Click();

        provider.IsAuthenticated().ShouldBeTrue();
        provider.GetUsername().ShouldBe("hsimpson");
    }

    private class MockEmployeeRepository : IEmployeeRepository
    {
        public Task<Employee> GetByUserNameAsync(string? userName)
        {
            var employee = new Employee(userName!, "Homer", "Simpson", "homer@springfield.com");
            return Task.FromResult(employee);
        }

        public Task<Employee[]> GetEmployeesAsync(EmployeeSpecification spec)
        {
            var employees = new[]
            {
                new Employee("hsimpson", "Homer", "Simpson", "homer@springfield.com"),
                new Employee("mburns", "Montgomery", "Burns", "burns@plant.com"),
                new Employee("nflanders", "Ned", "Flanders", "ned@flanders.com")
            };
            return Task.FromResult(employees);
        }
    }

    private class StubUiBus : IUiBus
    {
        public void Notify(object eventObject)
        {
            // Mock implementation - do nothing
        }

        public void Register(IListener listener)
        {
            // Mock implementation - do nothing
        }

        public void UnRegister(IListener listener)
        {
            // Mock implementation - do nothing
        }

        public IListener<T>[] GetListeners<T>() where T : IUiBusEvent
        {
            return Array.Empty<IListener<T>>();
        }

        public void Notify<T>(T eventObject) where T : IUiBusEvent
        {
            // Mock implementation - do nothing
        }

        public void UnRegisterAll()
        {
            // Mock implementation - do nothing
        }
    }
}