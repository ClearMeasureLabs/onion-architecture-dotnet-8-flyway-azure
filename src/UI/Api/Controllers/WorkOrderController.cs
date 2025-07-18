using Core.Model;
using Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace ProgrammingWithPalermo.ChurchBulletin.UI.Api.Controllers
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
            //TODO: Implement single webapi for commands and queries    
            var spec = new WorkOrderSearchSpecification();
            var result = await _repository.GetWorkOrdersAsync(spec);
            return Ok(result);
        }
    }
}
