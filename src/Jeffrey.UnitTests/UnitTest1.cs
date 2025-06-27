using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Core.Model;
using Core.Services;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using UI.Client;
using Shouldly;

namespace Jeffrey.UnitTests;

[TestFixture]
public class HttpEmployeeRepositoryTests
{
    private HttpClient CreateMockHttpClient(HttpResponseMessage response)
    {
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response)
            .Verifiable();
        return new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("http://localhost/")
        };
    }

    [Test]
    public void GetByUserName_ReturnsEmployee_ForValidUserName()
    {
        var employee = new Employee("jdoe", "John", "Doe", "jdoe@email.com");
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(employee)
        };
        var httpClient = CreateMockHttpClient(response);
        var repo = new HttpEmployeeRepository(httpClient);
        var result = repo.GetByUserName("jdoe");
        result.UserName.ShouldBe("jdoe");
        result.FirstName.ShouldBe("John");
        result.LastName.ShouldBe("Doe");
        result.EmailAddress.ShouldBe("jdoe@email.com");
    }

    [Test]
    public void GetByUserName_Throws_ForNotFound()
    {
        var response = new HttpResponseMessage(HttpStatusCode.NotFound);
        var httpClient = CreateMockHttpClient(response);
        var repo = new HttpEmployeeRepository(httpClient);
        Should.Throw<InvalidOperationException>(() => repo.GetByUserName("notfound"));
    }

    [Test]
    public void GetEmployees_ReturnsEmployees_ForAllSpecification()
    {
        var employees = new[]
        {
            new Employee("jdoe", "John", "Doe", "jdoe@email.com"),
            new Employee("asmith", "Alice", "Smith", "asmith@email.com")
        };
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(employees)
        };
        var httpClient = CreateMockHttpClient(response);
        var repo = new HttpEmployeeRepository(httpClient);
        var result = repo.GetEmployees(EmployeeSpecification.All);
        result.Length.ShouldBe(2);
        result[0].UserName.ShouldBe("jdoe");
        result[1].UserName.ShouldBe("asmith");
    }

    [Test]
    public void GetEmployees_ReturnsEmpty_ForEmptyResponse()
    {
        var employees = Array.Empty<Employee>();
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(employees)
        };
        var httpClient = CreateMockHttpClient(response);
        var repo = new HttpEmployeeRepository(httpClient);
        var result = repo.GetEmployees(EmployeeSpecification.All);
        result.ShouldBeEmpty();
    }

    [Test]
    public void GetEmployees_Throws_ForErrorResponse()
    {
        var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
        var httpClient = CreateMockHttpClient(response);
        var repo = new HttpEmployeeRepository(httpClient);
        Should.Throw<InvalidOperationException>(() => repo.GetEmployees(EmployeeSpecification.All));
    }

    [Test]
    public void GetEmployees_WithCanFulfillSpecification_ReturnsEmployees()
    {
        var employees = new[]
        {
            new Employee("jdoe", "John", "Doe", "jdoe@email.com"),
            new Employee("asmith", "Alice", "Smith", "asmith@email.com")
        };
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(employees)
        };
        var httpClient = CreateMockHttpClient(response);
        var repo = new HttpEmployeeRepository(httpClient);
        var spec = new EmployeeSpecification(true);
        var result = repo.GetEmployees(spec);
        result.Length.ShouldBe(2);
    }
}
