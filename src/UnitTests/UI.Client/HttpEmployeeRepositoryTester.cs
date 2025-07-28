using System.Net;
using System.Net.Http.Json;
using Core.Model;
using Core.Services;
using Shouldly;
using UI.Client;

namespace ProgrammingWithPalermo.ChurchBulletin.UnitTests.UI.Client;

[TestFixture]
public class HttpEmployeeRepositoryTester
{
    private class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage _response;

        public MockHttpMessageHandler(HttpResponseMessage response)
        {
            _response = response;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(_response);
        }
    }

    [Test]
    public async Task GetByUserNameAsync_ReturnsEmployee()
    {
        var expected = new Employee("jdoe", "John", "Doe", "jdoe@email.com");
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(expected)
        };
        var handler = new MockHttpMessageHandler(response);
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("http://localhost/") };
        var repo = new HttpEmployeeRepository(httpClient);

        var result = await repo.GetByUserNameAsync("jdoe");
        result.UserName.ShouldBe(expected.UserName);
        result.FirstName.ShouldBe(expected.FirstName);
        result.LastName.ShouldBe(expected.LastName);
        result.EmailAddress.ShouldBe(expected.EmailAddress);
    }

    [Test]
    public async Task GetEmployeesAsync_ReturnsEmployees()
    {
        var expected = new[]
        {
            new Employee("jdoe", "John", "Doe", "jdoe@email.com"),
            new Employee("asmith", "Alice", "Smith", "asmith@email.com")
        };
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(expected)
        };
        var handler = new MockHttpMessageHandler(response);
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("http://localhost/") };
        var repo = new HttpEmployeeRepository(httpClient);

        var result = await repo.GetEmployeesAsync(new EmployeeSpecification());
        result.Length.ShouldBe(2);
        result[0].UserName.ShouldBe("jdoe");
        result[1].UserName.ShouldBe("asmith");
    }
}