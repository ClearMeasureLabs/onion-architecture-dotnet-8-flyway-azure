using System.Net.Http;
using System.Net.Http.Json;
using ClearMeasure.Bootcamp.Core.Model;
using ClearMeasure.Bootcamp.Core.Services;

namespace ClearMeasure.Bootcamp.UI.Client
{
    public class HttpEmployeeRepository : IEmployeeRepository
    {
        private readonly HttpClient _httpClient;
        public HttpEmployeeRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Employee> GetByUserNameAsync(string? userName)
        {
            var employee = await _httpClient.GetFromJsonAsync<Employee>($"employee/by-username/{userName}");
            return employee!;
        }

        public async Task<Employee[]> GetEmployeesAsync(EmployeeSpecification spec)
        {
            var employees = await _httpClient.GetFromJsonAsync<Employee[]>($"employee");
            return employees!;
        }
    }
}
