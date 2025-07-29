using MediatR;
using ProgrammingWithPalermo.ChurchBulletin.Core;

namespace UI.Client;

public class RemotableBus(IMediator mediatr, PublisherGateway gateway) : IBus
{
    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request)
    {
        if (request is IRemotableRequest remotableRequest)
        {
            WebServiceMessage result = await gateway.Publish(remotableRequest) ?? throw new InvalidOperationException();
            TResponse returnEvent = result.GetBodyObject<TResponse>();
            return returnEvent;
        }

        return await mediatr.Send(request);
    }

    public Task<object?> Send(object request)
    {
        return mediatr.Send(request);
    }

    public void Publish<TNotification>(TNotification notification) where TNotification : INotification
    {
        mediatr.Publish(notification);
    }
}