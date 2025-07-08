using System.ComponentModel.DataAnnotations;
using Core.Model;

namespace UIWasm.Models
{
    public class WorkOrderSearchModel
    {
        public WorkOrderSearchModel()
        {
            Filters = new SearchFilters();
            Results = new WorkOrder[0];
        }

        public SearchFilters Filters { get; set; }
        public WorkOrder[] Results { get; set; }

        public class SearchFilters
        {
            public string? Creator { get; set; }
            public string? Assignee { get; set; }
            public string? Status { get; set; }
        }
    }
}