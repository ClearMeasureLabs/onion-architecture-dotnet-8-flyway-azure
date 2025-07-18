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
            var url = BuildSearchUrl(specification);
            var result = await _httpClient.GetFromJsonAsync<WorkOrder[]>(url);
            return result ?? Array.Empty<WorkOrder>();
        }

        private string BuildSearchUrl(WorkOrderSearchSpecification specification)
        {
            var url = "api/workorder/search";
            var queryParams = new List<string>();

            if (specification.Status != null)
            {
                queryParams.Add($"status={specification.Status.Key}");
            }

            if (specification.Creator != null)
            {
                queryParams.Add($"creator={specification.Creator.UserName}");
            }

            if (specification.Assignee != null)
            {
                queryParams.Add($"assignee={specification.Assignee.UserName}");
            }

            if (queryParams.Count > 0)
            {
                url += "?" + string.Join("&", queryParams);
            }

            return url;
        }
    }
}
