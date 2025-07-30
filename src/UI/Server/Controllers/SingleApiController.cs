using ClearMeasure.Bootcamp.Core;
using Microsoft.AspNetCore.Mvc;
using ClearMeasure.Bootcamp.UI.Client;

namespace ClearMeasure.Bootcamp.UI.Server.Controllers;

[ApiController]
[Route(PublisherGateway.ApiRelativeUrl)]
public class SingleApiController(IBus bus) : ControllerBase
{
    [HttpPost]
    public async Task<string> Post(WebServiceMessage webServiceMessage)
    {
        var result = await bus.Send(webServiceMessage.GetBodyObject()) ?? throw new InvalidOperationException();
        return new WebServiceMessage(result).GetJson();
    }
}