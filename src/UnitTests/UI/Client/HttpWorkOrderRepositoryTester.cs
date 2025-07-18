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
        [Test,  Ignore("not ready")]
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

        [Test]
        public async Task GetWorkOrdersAsync_WithSearchSpecification_ShouldIncludeParametersInUrl()
        {
            var creator = new Employee("creator1", "John", "Doe", "john@example.com");
            var assignee = new Employee("assignee1", "Jane", "Smith", "jane@example.com");
            
            var handler = new FakeHttpMessageHandler((request) =>
            {
                if (request.RequestUri!.ToString().Contains("api/workorder/search"))
                {
                    var url = request.RequestUri.ToString();
                    url.ShouldContain("status=Assigned");
                    url.ShouldContain($"creator={creator.UserName}");
                    url.ShouldContain($"assignee={assignee.UserName}");

                    var workOrders = new WorkOrder[]
                    {
                        new WorkOrder { Number = "WO001", Title = "Test Work Order", Status = WorkOrderStatus.Assigned }
                    };
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = JsonContent.Create(workOrders)
                    };
                }
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            });

            var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://localhost:5001/")
            };
            var repo = new HttpWorkOrderRepository(httpClient);
            
            var specification = new WorkOrderSearchSpecification();
            specification.MatchStatus(WorkOrderStatus.Assigned);
            specification.MatchCreator(creator);
            specification.MatchAssignee(assignee);

            var result = await repo.GetWorkOrdersAsync(specification);
            
            result.ShouldNotBeNull();
            result.Length.ShouldBe(1);
            result[0].Number.ShouldBe("WO001");
        }

        [Test]
        public async Task GetWorkOrdersAsync_ShouldReturnWorkOrdersWithCompleteEmployeeProperties()
        {
            var creatorId = Guid.NewGuid();
            var assigneeId = Guid.NewGuid();
            var creator = new Employee("creator1", "John", "Doe", "john@example.com") { Id = creatorId };
            var assignee = new Employee("assignee1", "Jane", "Smith", "jane@example.com") { Id = assigneeId };
            
            var handler = new FakeHttpMessageHandler((request) =>
            {
                if (request.RequestUri!.ToString().Contains("api/workorder/search"))
                {
                    var workOrders = new WorkOrder[]
                    {
                        new WorkOrder 
                        { 
                            Number = "WO001", 
                            Title = "Test Work Order", 
                            Status = WorkOrderStatus.Assigned,
                            Creator = creator,
                            Assignee = assignee
                        }
                    };
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = JsonContent.Create(workOrders)
                    };
                }
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            });

            var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://localhost:5001/")
            };
            var repo = new HttpWorkOrderRepository(httpClient);
            
            var specification = new WorkOrderSearchSpecification();
            var result = await repo.GetWorkOrdersAsync(specification);
            
            result.ShouldNotBeNull();
            result.Length.ShouldBe(1);
            
            var workOrder = result[0];
            workOrder.Creator.ShouldNotBeNull();
            workOrder.Creator!.UserName.ShouldBe("creator1");
            workOrder.Creator.FirstName.ShouldBe("John");
            workOrder.Creator.LastName.ShouldBe("Doe");
            workOrder.Creator.EmailAddress.ShouldBe("john@example.com");
            workOrder.Creator.Id.ShouldBe(creatorId);
            
            workOrder.Assignee.ShouldNotBeNull();
            workOrder.Assignee!.UserName.ShouldBe("assignee1");
            workOrder.Assignee.FirstName.ShouldBe("Jane");
            workOrder.Assignee.LastName.ShouldBe("Smith");
            workOrder.Assignee.EmailAddress.ShouldBe("jane@example.com");
            workOrder.Assignee.Id.ShouldBe(assigneeId);
        }

        [Test]
        public async Task GetWorkOrdersAsync_WithSearchSpecification_ShouldUseUserNameInQueryParams()
        {
            var creator = new Employee("johndoe", "John", "Doe", "john@example.com");
            var assignee = new Employee("janesmith", "Jane", "Smith", "jane@example.com");
            
            var handler = new FakeHttpMessageHandler((request) =>
            {
                if (request.RequestUri!.ToString().Contains("api/workorder/search"))
                {
                    var url = request.RequestUri.ToString();
                    url.ShouldContain("status=Assigned");
                    url.ShouldContain($"creator={creator.UserName}");
                    url.ShouldContain($"assignee={assignee.UserName}");
                    url.ShouldNotContain($"creatorId");
                    url.ShouldNotContain($"assigneeId");

                    var workOrders = new WorkOrder[]
                    {
                        new WorkOrder { Number = "WO001", Title = "Test Work Order", Status = WorkOrderStatus.Assigned }
                    };
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = JsonContent.Create(workOrders)
                    };
                }
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            });

            var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://localhost:5001/")
            };
            var repo = new HttpWorkOrderRepository(httpClient);
            
            var specification = new WorkOrderSearchSpecification();
            specification.MatchStatus(WorkOrderStatus.Assigned);
            specification.MatchCreator(creator);
            specification.MatchAssignee(assignee);

            var result = await repo.GetWorkOrdersAsync(specification);
            
            result.ShouldNotBeNull();
            result.Length.ShouldBe(1);
            result[0].Number.ShouldBe("WO001");
        }

        [Test]
        public async Task GetWorkOrdersAsync_WithSearchSpecification_ShouldUseSimplifiedParameterNames()
        {
            var creator = new Employee("johndoe", "John", "Doe", "john@example.com");
            var assignee = new Employee("janesmith", "Jane", "Smith", "jane@example.com");
            
            var handler = new FakeHttpMessageHandler((request) =>
            {
                if (request.RequestUri!.ToString().Contains("api/workorder/search"))
                {
                    var url = request.RequestUri.ToString();
                    url.ShouldContain("status=Assigned");
                    url.ShouldContain($"creator={creator.UserName}");
                    url.ShouldContain($"assignee={assignee.UserName}");
                    url.ShouldNotContain("creatorUserName");
                    url.ShouldNotContain("assigneeUserName");

                    var workOrders = new WorkOrder[]
                    {
                        new WorkOrder { Number = "WO001", Title = "Test Work Order", Status = WorkOrderStatus.Assigned }
                    };
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = JsonContent.Create(workOrders)
                    };
                }
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            });

            var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://localhost:5001/")
            };
            var repo = new HttpWorkOrderRepository(httpClient);
            
            var specification = new WorkOrderSearchSpecification();
            specification.MatchStatus(WorkOrderStatus.Assigned);
            specification.MatchCreator(creator);
            specification.MatchAssignee(assignee);

            var result = await repo.GetWorkOrdersAsync(specification);
            
            result.ShouldNotBeNull();
            result.Length.ShouldBe(1);
            result[0].Number.ShouldBe("WO001");
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
