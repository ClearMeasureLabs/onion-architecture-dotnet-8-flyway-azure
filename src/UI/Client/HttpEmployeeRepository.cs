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
            return GetByUserNameAsync(userName).GetAwaiter().GetResult();
        }

        public async Task<Employee> GetByUserNameAsync(string userName)
        {
            var employee = await _httpClient.GetFromJsonAsync<Employee>($"employee/by-username/{userName}");
            return employee!;
        }

        public Employee[] GetEmployees(EmployeeSpecification spec)
        {
            var employees = _httpClient.GetFromJsonAsync<Employee[]>($"employee").GetAwaiter().GetResult();
            return employees!;
        }
    }
}
