using ProgrammingWithPalermo.ChurchBulletin.Core;

namespace UI.Client
{
    public interface IPublisherGateway
    {
        Task<WebServiceMessage?> Publish(IRemotableRequest request);
    }
}
