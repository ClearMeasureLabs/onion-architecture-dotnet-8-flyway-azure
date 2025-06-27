using Microsoft.AspNetCore.Mvc;
using Core.Model;
using Core.Services;

namespace ProgrammingWithPalermo.ChurchBulletin.UI.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkOrderController : ControllerBase
    {
        private readonly IWorkOrderRepository _repository;
        public WorkOrderController(IWorkOrderRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("search")]
        public async Task<ActionResult<WorkOrder[]>> Search()
        {
            // For demo, return all work orders. Adjust to accept filters as needed.
            var spec = new WorkOrderSearchSpecification();
            var result = await _repository.GetWorkOrdersAsync(spec);
            return Ok(result);
        }
    }
}
