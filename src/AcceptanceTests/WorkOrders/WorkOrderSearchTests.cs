using ClearMeasure.Bootcamp.UI.Shared;
using ClearMeasure.Bootcamp.UI.Shared.Pages;
using Spectre.Console;

namespace ClearMeasure.Bootcamp.AcceptanceTests.WorkOrders;

public class WorkOrderSearchTests : AcceptanceTestBase
{
    [SetUp]
    public async Task Setup()
    {
        var username = CurrentUser.UserName;
        if (await Page.Locator($"text=Welcome {username}!").IsVisibleAsync()) return;

        await Page.GotoAsync("/login");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Fill in username only
        await Page.SelectOptionAsync("#employee", username);

        // Submit form
        var loginButton = Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Login" });
        await loginButton.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Assert: Should be redirected to home and see welcome message
        await Expect(Page.Locator($"text=Welcome {username}!")).ToBeVisibleAsync();
    }

    [Test]
    public async Task ShouldLoadDropDownsInitiallyOnLoad()
    {
        // Act
        await Click(nameof(NavMenu.Elements.Search));
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await TakeScreenshotAsync(1, "PageLoaded");

        // Assert
        var creatorSelect = Page.Locator($"#{WorkOrderSearch.Elements.CreatorSelect}");
        var assigneeSelect = Page.Locator($"#{WorkOrderSearch.Elements.AssigneeSelect}");
        var statusSelect = Page.Locator($"#{WorkOrderSearch.Elements.StatusSelect}");

        await Expect(creatorSelect).ToBeVisibleAsync();
        await Expect(assigneeSelect).ToBeVisibleAsync();
        await Expect(statusSelect).ToBeVisibleAsync();

        var creatorOptions = await creatorSelect.Locator("option").AllAsync();
        creatorOptions.Count.ShouldBeGreaterThan(3);

        var firstCreatorOption = await creatorOptions[0].TextContentAsync();
        firstCreatorOption.ShouldBe("All");

        var assigneeOptions = await assigneeSelect.Locator("option").AllAsync();
        assigneeOptions.Count.ShouldBeGreaterThan(3);

        var firstAssigneeOption = await assigneeOptions[0].TextContentAsync();
        firstAssigneeOption.ShouldBe("All");

        // Verify status options are loaded (5 statuses + "All" option = 6 options)
        var statusOptions = await statusSelect.Locator("option").AllAsync();
        statusOptions.Count.ShouldBe(WorkOrderStatus.GetAllItems().Length + 1);

        var firstStatusOption = await statusOptions[0].TextContentAsync();
        firstStatusOption.ShouldBe("All");
    }

    [Test]
    public async Task ShouldLoadWorkOrderTableWithAllFiltersSetToAllOnInitialLoad()
    {
        // Arrange
        var creator = Faker<Employee>();
        var assignee = Faker<Employee>();
        var order1 = Faker<WorkOrder>();
        var order2 = Faker<WorkOrder>();
        order1.Creator = creator;
        order1.Assignee = assignee;
        order2.Creator = creator;
        order2.Assignee = assignee;

        await using var context = TestHost.NewDbContext();
        context.Add(creator);
        context.Add(assignee);
        context.Add(order1);
        context.Add(order2);
        await context.SaveChangesAsync();

        // Act
        await Click(nameof(NavMenu.Elements.Search));
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await TakeScreenshotAsync(1, "InitialLoad");

        // Assert
        var workOrderTable = Page.Locator(".grid-data");
        await Expect(workOrderTable).ToBeVisibleAsync();

        var workOrderRows = workOrderTable.Locator("tbody tr");
        var rowCount = await workOrderRows.CountAsync();
        rowCount.ShouldBeGreaterThanOrEqualTo(2);
    }

    [Test]
    public async Task ShouldLoadWorkOrderTableWithCreatorFilterFromQueryString()
    {
        // Arrange
        var creator = CurrentUser;
        var order = Faker<WorkOrder>();
        order.Creator = creator;
        await using var context = TestHost.NewDbContext();
        context.Attach(creator);
        context.Add(order);
        await context.SaveChangesAsync();

        // Act
        await Click(nameof(NavMenu.Elements.MyWorkOrders));
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await TakeScreenshotAsync(1, "CreatorFiltered");

        // Assert
        var creatorSelect = Page.Locator($"#{WorkOrderSearch.Elements.CreatorSelect}");
        var selectedValue = await creatorSelect.InputValueAsync();
        selectedValue.ShouldBe(creator.UserName);

        var workOrderTable = Page.Locator(".grid-data");
        await Expect(workOrderTable).ToBeVisibleAsync();

        var workOrderRows = workOrderTable.Locator("tbody tr");
        var rowCount = await workOrderRows.CountAsync();
        rowCount.ShouldBe(1);
        workOrderRows.First.Locator("td:nth-child(2)").InnerTextAsync().Result.ShouldContain(creator.GetFullName());
    }

    [Test]
    public async Task ShouldLoadWorkOrderTableWithAssigneeFilterFromQueryString()
    {
        // Arrange
        var creator = Faker<Employee>();
        var assignee = CurrentUser;
        var order = Faker<WorkOrder>();
        order.Creator = creator;
        order.Assignee = assignee;

        await using var context = TestHost.NewDbContext();
        context.Add(creator);
        context.Attach(assignee);
        context.Add(order);
        await context.SaveChangesAsync();

        // Act
        await Click(nameof(NavMenu.Elements.WorkOrdersAssignedToMe));
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await TakeScreenshotAsync(1, "AssigneeFiltered");

        // Assert
        var assigneeSelect = Page.Locator($"#{WorkOrderSearch.Elements.AssigneeSelect}");
        var selectedValue = await assigneeSelect.InputValueAsync();
        selectedValue.ShouldBe(assignee.UserName);

        var workOrderTable = Page.Locator(".grid-data");
        await Expect(workOrderTable).ToBeVisibleAsync();

        var workOrderRows = workOrderTable.Locator("tbody tr");
        var rowCount = await workOrderRows.CountAsync();
        rowCount.ShouldBe(1);
        workOrderRows.First.Locator("td:nth-child(3)").InnerTextAsync().Result.ShouldContain(assignee.GetFullName());
    }

    [Test]
    public async Task ShouldLoadWorkOrderTableWithStatusFilterFromQueryString()
    {
        // Arrange
        var creator = Faker<Employee>();
        var status = WorkOrderStatus.Assigned;
        var order = Faker<WorkOrder>();
        order.Creator = creator;
        order.Status = status;

        await using var context = TestHost.NewDbContext();
        context.Add(creator);
        context.Add(order);
        await context.SaveChangesAsync();

        // Act
        await Click(nameof(NavMenu.Elements.AllAssignedWorkOrders));
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await TakeScreenshotAsync(1, "StatusFiltered");

        // Assert
        var statusSelect = Page.Locator($"#{WorkOrderSearch.Elements.StatusSelect}");
        var selectedValue = await statusSelect.InputValueAsync();
        selectedValue.ShouldBe(status.Key);

        var workOrderTable = Page.Locator(".grid-data");
        await Expect(workOrderTable).ToBeVisibleAsync();

        var workOrderRows = workOrderTable.Locator("tbody tr");
        var rowCount = await workOrderRows.CountAsync();
        rowCount.ShouldBeGreaterThanOrEqualTo(1);
        workOrderRows.First.Locator("td:nth-child(4)").InnerTextAsync().Result.ShouldContain(status.FriendlyName);
    }

    [Test]
    public async Task ShouldSearchWithAllThreeFiltersSelected()
    {
        // Arrange
        var creator = Faker<Employee>();
        var assignee = Faker<Employee>();
        var status = Faker<WorkOrderStatus>();
        var order = Faker<WorkOrder>();
        order.Creator = creator;
        order.Assignee = assignee;
        order.Status = status;

        await using var context = TestHost.NewDbContext();
        context.Add(creator);
        context.Add(assignee);
        context.Add(order);
        await context.SaveChangesAsync();

        // Act
        await Click(nameof(NavMenu.Elements.Search));
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await TakeScreenshotAsync(1, "BeforeFiltering");

        var creatorSelect = Page.Locator($"#{WorkOrderSearch.Elements.CreatorSelect}");
        var assigneeSelect = Page.Locator($"#{WorkOrderSearch.Elements.AssigneeSelect}");
        var statusSelect = Page.Locator($"#{WorkOrderSearch.Elements.StatusSelect}");
        var searchButton = Page.Locator($"#{WorkOrderSearch.Elements.SearchButton}");

        await creatorSelect.SelectOptionAsync(creator.UserName);
        await assigneeSelect.SelectOptionAsync(assignee.UserName);
        await statusSelect.SelectOptionAsync(status.Key);
        await TakeScreenshotAsync(2, "FiltersSet");

        await searchButton.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await TakeScreenshotAsync(3, "SearchCompleted");

        // Assert
        var workOrderTable = Page.Locator(".grid-data");
        await Expect(workOrderTable).ToBeVisibleAsync();

        var workOrderRows = workOrderTable.Locator("tbody tr");
        var rowCount = await workOrderRows.CountAsync();
        rowCount.ShouldBeGreaterThanOrEqualTo(1);
    }

    [Test]
    public async Task ShouldNavigateToWorkOrderDetailsWhenClickingWorkOrderNumber()
    {
        // Arrange
        var creator = Faker<Employee>();
        var workOrder = Faker<WorkOrder>();
        workOrder.Creator = creator;

        await using var context = TestHost.NewDbContext();
        context.Add(creator);
        context.Add(workOrder);
        await context.SaveChangesAsync();

        // Act
        await Click(nameof(NavMenu.Elements.Search));
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await TakeScreenshotAsync(1, "SearchPageLoaded");

        var workOrderTable = Page.Locator(".grid-data");
        await Expect(workOrderTable).ToBeVisibleAsync();

        var firstWorkOrderLink = workOrderTable.Locator("tbody tr").First.Locator("td").First.Locator("a");
        var workOrderNumber = await firstWorkOrderLink.TextContentAsync();

        if (!string.IsNullOrEmpty(workOrderNumber))
        {
            await firstWorkOrderLink.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await TakeScreenshotAsync(2, "WorkOrderDetailsPage");

            // Assert
            Page.Url.ShouldContain($"/workorder/manage/{workOrderNumber}");
        }
    }

    [Test]
    public async Task ShouldClearFiltersWhenSelectingAllOption()
    {
        // Arrange
        var creator = Faker<Employee>();
        var order = Faker<WorkOrder>();
        order.Creator = creator;

        await using var context = TestHost.NewDbContext();
        context.Add(creator);
        context.Add(order);
        await context.SaveChangesAsync();

        // Act
        await Click(nameof(NavMenu.Elements.Search));
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var creatorSelect = Page.Locator($"#{WorkOrderSearch.Elements.CreatorSelect}");
        var searchButton = Page.Locator($"#{WorkOrderSearch.Elements.SearchButton}");

        // First set a filter
        await creatorSelect.SelectOptionAsync(creator.UserName);
        await searchButton.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await TakeScreenshotAsync(1, "FilterSet");

        // Then clear it by selecting "All"
        await creatorSelect.SelectOptionAsync("");
        await searchButton.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await TakeScreenshotAsync(2, "FilterCleared");

        // Assert
        var selectedValue = await creatorSelect.InputValueAsync();
        selectedValue.ShouldBe("");

        var workOrderTable = Page.Locator(".grid-data");
        await Expect(workOrderTable).ToBeVisibleAsync();
    }

    [Test]
    public async Task ShouldDisplayWorkOrderInformationInTable()
    {
        // Arrange
        var creator = Faker<Employee>();
        var assignee = Faker<Employee>();
        var order = Faker<WorkOrder>();
        order.Creator = creator;
        order.Assignee = assignee;

        await using var context = TestHost.NewDbContext();
        context.Add(creator);
        context.Add(assignee);
        context.Add(order);
        await context.SaveChangesAsync();

        // Act
        await Click(nameof(NavMenu.Elements.Search));
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await TakeScreenshotAsync(1, "TableLoaded");

        // Assert
        var workOrderTable = Page.Locator(".grid-data");
        await Expect(workOrderTable).ToBeVisibleAsync();

        // Check table headers
        var headers = workOrderTable.Locator("thead th");
        var headerCount = await headers.CountAsync();
        headerCount.ShouldBe(5);

        var headerTexts = new List<string>();
        for (var i = 0; i < headerCount; i++)
        {
            var headerText = await headers.Nth(i).TextContentAsync();
            headerTexts.Add(headerText ?? "");
        }

        headerTexts.ShouldContain("WO #");
        headerTexts.ShouldContain("Creator");
        headerTexts.ShouldContain("Assignee");
        headerTexts.ShouldContain("Status");
        headerTexts.ShouldContain("Title");

        // Check if there are data rows
        var dataRows = workOrderTable.Locator("tbody tr");
        var rowCount = await dataRows.CountAsync();

        if (rowCount > 0)
        {
            // Verify first row has the expected number of columns
            var firstRowCells = dataRows.First.Locator("td");
            var cellCount = await firstRowCells.CountAsync();
            cellCount.ShouldBe(5);
        }
    }

    [Test]
    public async Task ShouldMaintainSelectedFiltersAfterSearch()
    {
        // Arrange
        var creator = Faker<Employee>();
        var assignee = Faker<Employee>();
        var status = Faker<WorkOrderStatus>();
        var order = Faker<WorkOrder>();
        order.Creator = creator;
        order.Assignee = assignee;
        order.Status = status;

        await using var context = TestHost.NewDbContext();
        context.Add(creator);
        context.Add(assignee);
        context.Add(order);
        await context.SaveChangesAsync();

        // Act
        await Click(nameof(NavMenu.Elements.Search));
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var creatorSelect = Page.Locator($"#{WorkOrderSearch.Elements.CreatorSelect}");
        var assigneeSelect = Page.Locator($"#{WorkOrderSearch.Elements.AssigneeSelect}");
        var statusSelect = Page.Locator($"#{WorkOrderSearch.Elements.StatusSelect}");
        var searchButton = Page.Locator($"#{WorkOrderSearch.Elements.SearchButton}");

        await creatorSelect.SelectOptionAsync(creator.UserName);
        await assigneeSelect.SelectOptionAsync(assignee.UserName);
        await statusSelect.SelectOptionAsync(status.Key);

        await searchButton.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await TakeScreenshotAsync(1, "AfterSearch");

        // Assert
        var creatorValue = await creatorSelect.InputValueAsync();
        var assigneeValue = await assigneeSelect.InputValueAsync();
        var statusValue = await statusSelect.InputValueAsync();

        creatorValue.ShouldBe(creator.UserName);
        assigneeValue.ShouldBe(assignee.UserName);
        statusValue.ShouldBe(status.Key);
    }

    [Test]
    public async Task ShouldReloadParamsFromQueryStringWithNavigation()
    {
        // Arrange
        var order1 = Faker<WorkOrder>();
        order1.Status = WorkOrderStatus.InProgress;
        var order2 = Faker<WorkOrder>();
        order1.Creator = CurrentUser;
        order1.Assignee = CurrentUser;
        order2.Creator = CurrentUser;
        order2.Assignee = CurrentUser;

        await using var context = TestHost.NewDbContext();
        context.Attach(CurrentUser);
        context.Add(order1);
        context.Add(order2);
        await context.SaveChangesAsync();

        // Act
        await Click(nameof(NavMenu.Elements.Search));
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Assert
        var creatorSelect = Page.Locator($"#{WorkOrderSearch.Elements.CreatorSelect}");
        var assigneeSelect = Page.Locator($"#{WorkOrderSearch.Elements.AssigneeSelect}");
        var statusSelect = Page.Locator($"#{WorkOrderSearch.Elements.StatusSelect}");

        (await creatorSelect.InputValueAsync()).ShouldBe("");
        (await assigneeSelect.InputValueAsync()).ShouldBe("");
        (await statusSelect.InputValueAsync()).ShouldBe("");

        await Click(nameof(NavMenu.Elements.MyWorkOrders));
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await creatorSelect.DblClickAsync();
        await Expect(creatorSelect).ToHaveValueAsync(CurrentUser.UserName);

        await Click(nameof(NavMenu.Elements.WorkOrdersAssignedToMe));
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await assigneeSelect.DblClickAsync();
        await Expect(assigneeSelect).ToHaveValueAsync(CurrentUser.UserName);

        await Click(nameof(NavMenu.Elements.AllWorkOrdersInProgress));
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await statusSelect.DblClickAsync();
        await Expect(statusSelect).ToHaveValueAsync(order1.Status.Key);
    }

    protected override bool? Headless { get; set; } = true;
}