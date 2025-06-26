using System.Net.Http;
using System.Net.Http.Json;
using Core.Model;
using Core.Services;

namespace UI.Client
{
    public class HttpEmployeeRepository : IEmployeeRepository
    {
        private readonly HttpClient _httpClient;
        public HttpEmployeeRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Employee GetByUserName(string userName)
        {
            var employee = _httpClient.GetFromJsonAsync<Employee>($"employee/by-username/{userName}").GetAwaiter().GetResult();
            return employee!;
        }

        public Employee[] GetEmployees(EmployeeSpecification spec)
        {
            var employees = _httpClient.GetFromJsonAsync<Employee[]>($"employee").GetAwaiter().GetResult();
            return employees!;
        }
    }
}
