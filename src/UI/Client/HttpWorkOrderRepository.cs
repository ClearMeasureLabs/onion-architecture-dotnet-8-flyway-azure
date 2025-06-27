using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Core.Model;
using Core.Services;

namespace UI.Client
{
    public class HttpWorkOrderRepository : IWorkOrderRepository
    {
        private readonly HttpClient _httpClient;
        public HttpWorkOrderRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task SaveAsync(WorkOrder workOrder)
        {
            await _httpClient.PostAsJsonAsync("workorder", workOrder);
        }

        public async Task<WorkOrder?> GetWorkOrderAsync(string number)
        {
            return await _httpClient.GetFromJsonAsync<WorkOrder>($"workorder/{number}");
        }

        public async Task<WorkOrder[]> GetWorkOrdersAsync(WorkOrderSearchSpecification specification)
        {
            // Call the correct API endpoint for search
            var url = "api/workorder/search";
            var result = await _httpClient.GetFromJsonAsync<WorkOrder[]>(url);
            return result ?? Array.Empty<WorkOrder>();
        }
    }
}
