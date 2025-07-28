using MediatR;
using ProgrammingWithPalermo.ChurchBulletin.Core;

namespace UI.Client;

public class RemoteableBus(IBus bus, PublisherGateway gateway) : IBus
{
    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request)
    {
        if (request is IRemoteableRequest remotableRequest)
        {
            WebServiceMessage result = await gateway.Publish(remotableRequest) ?? throw new InvalidOperationException();
            TResponse returnEvent = result.GetBodyObject<TResponse>();
            return returnEvent;
        }

        return await bus.Send(request);
    }

    public Task<object?> Send(object request)
    {
        return bus.Send(request);
    }

    public void Publish<TNotification>(TNotification notification) where TNotification : INotification
    {
        bus.Publish(notification);
    }
}