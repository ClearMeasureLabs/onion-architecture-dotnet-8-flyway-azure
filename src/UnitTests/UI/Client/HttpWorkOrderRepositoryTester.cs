using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Core.Model;
using Core.Services;
using NUnit.Framework;
using Shouldly;
using UI.Client;

namespace UnitTests.UI.Client
{
    [TestFixture]
    public class HttpWorkOrderRepositoryTester
    {
        [Test, Ignore()]
        public async Task GetWorkOrderAsync_WithValidNumber_ReturnsWorkOrder()
        {
            var handler = new FakeHttpMessageHandler((request) =>
            {
                if (request.RequestUri!.ToString().Contains("workorder/123"))
                {
                    var workOrder = new WorkOrder { Number = "123", Title = "Test WO" };
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = JsonContent.Create(workOrder)
                    };
                }
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            });
            var httpClient = new HttpClient(handler);
            var repo = new HttpWorkOrderRepository(httpClient);
            var result = await repo.GetWorkOrderAsync("123");
            result.ShouldNotBeNull();
            result!.Number.ShouldBe("123");
            result.Title.ShouldBe("Test WO");
        }
    }

    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _handler;
        public FakeHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handler)
        {
            _handler = handler;
        }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_handler(request));
        }
    }
}
