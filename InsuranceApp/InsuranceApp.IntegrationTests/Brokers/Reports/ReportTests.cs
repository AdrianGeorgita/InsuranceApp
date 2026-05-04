using FluentAssertions;
using InsuranceApp.Application.Common.Constants;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Reports.DTOs;
using InsuranceApp.Domain.Enums;
using InsuranceApp.IntegrationTests.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;

namespace InsuranceApp.IntegrationTests.Brokers.Reports;

public class ReportTests : IntegrationTestBase
{
    [Fact]
    public async Task ListAllReportsAsync_GivenCountryGroupingAndOnlyDateRangeFilter_ShouldReturnPagedListOfReports()
    {
        HttpClient.AuthenticateAs(AppRoles.Admin);
        const ReportGroup groupingBy = ReportGroup.Country;
        var pageRequest = GetValidPageRequest();
        var filter = GetValidFilter();
        var api = GetRequestUrl(pageRequest, groupingBy, filter);
        const int totalCount = 14;
        var returnedItems = new List<ReportDto>()
        {
            new()
            {
                GroupingKey = "Germany",
                Currency = "EUR",
                PolicyCount = 2,
                TotalFinalPremium = 12646.58M,
                TotalFinalPremiumInBaseCurrency = 62853.5M
            },
            new()
            {
                GroupingKey = "Germany",
                Currency = "GBP",
                PolicyCount = 1,
                TotalFinalPremium = 9996.94M,
                TotalFinalPremiumInBaseCurrency = 58182.19M
            }
        };

        var response = await HttpClient.GetAsync(api);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        await AssertBodyContainsItems(response, pageRequest, returnedItems, totalCount);
    }

    [Fact]
    public async Task ListAllReportsAsync_GivenBrokerGroupingAndOnlyDateRangeFilter_ShouldReturnPagedListOfReports()
    {
        HttpClient.AuthenticateAs(AppRoles.Admin);
        const ReportGroup groupingBy = ReportGroup.Broker;
        var pageRequest = GetValidPageRequest();
        var filter = GetValidFilter();
        var api = GetRequestUrl(pageRequest, groupingBy, filter);
        const int totalCount = 23;
        var returnedItems = new List<ReportDto>()
        {
            new()
            {
                GroupingKey = "A0F1B2C3-1111-4111-8111-111111111111",
                Currency = "RON",
                PolicyCount = 2,
                TotalFinalPremium = 7414.8M,
                TotalFinalPremiumInBaseCurrency = 7414.8M
            },
            new()
            {
                GroupingKey = "A0F1B2C3-1111-4111-8111-111111111111",
                Currency = "USD",
                PolicyCount = 1,
                TotalFinalPremium = 17820.11M,
                TotalFinalPremiumInBaseCurrency = 81616.1M
            }
        };

        var response = await HttpClient.GetAsync(api);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        await AssertBodyContainsItems(response, pageRequest, returnedItems, totalCount);
    }

    [Fact]
    public async Task ListAllReportsAsync_GivenCountyGroupingAndOnlyDateRangeFilter_ShouldReturnPagedListOfReports()
    {
        HttpClient.AuthenticateAs(AppRoles.Admin);
        const ReportGroup groupingBy = ReportGroup.County;
        var pageRequest = GetValidPageRequest();
        var filter = GetValidFilter();
        var api = GetRequestUrl(pageRequest, groupingBy, filter);
        const int totalCount = 24;
        var returnedItems = new List<ReportDto>()
        {
            new()
            {
                GroupingKey = "Bavaria",
                Currency = "EUR",
                PolicyCount = 1,
                TotalFinalPremium = 7331.56M,
                TotalFinalPremiumInBaseCurrency = 36437.85M
            },
            new()
            {
                GroupingKey = "Bavaria",
                Currency = "USD",
                PolicyCount = 1,
                TotalFinalPremium = 6568.51M,
                TotalFinalPremiumInBaseCurrency = 30083.78M
            }
        };

        var response = await HttpClient.GetAsync(api);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        await AssertBodyContainsItems(response, pageRequest, returnedItems, totalCount);
    }

    [Fact]
    public async Task ListAllReportsAsync_GivenCityGroupingAndOnlyDateRangeFilter_ShouldReturnPagedListOfReports()
    {
        HttpClient.AuthenticateAs(AppRoles.Admin);
        const ReportGroup groupingBy = ReportGroup.City;
        var pageRequest = GetValidPageRequest();
        var filter = GetValidFilter();
        var api = GetRequestUrl(pageRequest, groupingBy, filter);
        const int totalCount = 24;
        var returnedItems = new List<ReportDto>()
        {
            new()
            {
                GroupingKey = "Berlin",
                Currency = "GBP",
                PolicyCount = 1,
                TotalFinalPremium = 9996.94M,
                TotalFinalPremiumInBaseCurrency = 58182.19M
            },
            new()
            {
                GroupingKey = "Birmingham",
                Currency = "EUR",
                PolicyCount = 1,
                TotalFinalPremium = 6289.15M,
                TotalFinalPremiumInBaseCurrency = 31257.08M
            }
        };

        var response = await HttpClient.GetAsync(api);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        await AssertBodyContainsItems(response, pageRequest, returnedItems, totalCount);
    }

    [Fact]
    public async Task ListAllReportsAsync_GivenOutOfRangePage_ShouldReturnPagedEmptyListOfPolicies()
    {
        HttpClient.AuthenticateAs(AppRoles.Admin);
        const ReportGroup groupingBy = ReportGroup.Country;
        var pageRequest = GetValidPageRequest();
        pageRequest.PageSize = 10;
        pageRequest.PageNumber = 12512;
        var filter = GetValidFilter();
        var api = GetRequestUrl(pageRequest, groupingBy, filter);
        const int totalCount = 14;
        var returnedItems = new List<ReportDto>();

        var response = await HttpClient.GetAsync(api);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        await AssertBodyContainsItems(response, pageRequest, returnedItems, totalCount);
    }

    [Fact]
    public async Task ListAllReportsAsync_GivenBrokerGroupingWithCurrencyFilter_ShouldReturnPagedListOfReports()
    {
        HttpClient.AuthenticateAs(AppRoles.Admin);
        const ReportGroup groupingBy = ReportGroup.Broker;
        var pageRequest = GetValidPageRequest();
        var filter = GetValidFilter();
        filter.Currency = "RON";
        var api = GetRequestUrl(pageRequest, groupingBy, filter);
        const int totalCount = 6;
        var returnedItems = new List<ReportDto>()
        {
            new()
            {
                GroupingKey = "A0F1B2C3-1111-4111-8111-111111111111",
                Currency = "RON",
                PolicyCount = 2,
                TotalFinalPremium = 7414.8M,
                TotalFinalPremiumInBaseCurrency = 7414.8M
            },
            new()
            {
                GroupingKey = "A6F7B8C9-7777-4777-8777-777777777777",
                Currency = "RON",
                PolicyCount = 2,
                TotalFinalPremium = 4582.45M,
                TotalFinalPremiumInBaseCurrency = 4582.45M
            }
        };

        var response = await HttpClient.GetAsync(api);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        await AssertBodyContainsItems(response, pageRequest, returnedItems, totalCount);
    }

    [Fact]
    public async Task ListAllReportsAsync_GivenBrokerGroupingWithStatusFilter_ShouldReturnPagedListOfReports()
    {
        HttpClient.AuthenticateAs(AppRoles.Admin);
        const ReportGroup groupingBy = ReportGroup.Broker;
        var pageRequest = GetValidPageRequest();
        var filter = GetValidFilter();
        filter.Status = PolicyStatus.Expired;
        var api = GetRequestUrl(pageRequest, groupingBy, filter);
        const int totalCount = 6;
        var returnedItems = new List<ReportDto>()
        {
            new()
            {
                GroupingKey = "A0F1B2C3-1111-4111-8111-111111111111",
                Currency = "RON",
                PolicyCount = 1,
                TotalFinalPremium = 4352.61M,
                TotalFinalPremiumInBaseCurrency = 4352.61M
            },
            new()
            {
                GroupingKey = "A6F7B8C9-7777-4777-8777-777777777777",
                Currency = "RON",
                PolicyCount = 2,
                TotalFinalPremium = 4582.45M,
                TotalFinalPremiumInBaseCurrency = 4582.45M
            }
        };

        var response = await HttpClient.GetAsync(api);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        await AssertBodyContainsItems(response, pageRequest, returnedItems, totalCount);
    }

    [Fact]
    public async Task ListAllReportsAsync_GivenBrokerGroupingWithBuildingTypeFilter_ShouldReturnPagedListOfReports()
    {
        HttpClient.AuthenticateAs(AppRoles.Admin);
        const ReportGroup groupingBy = ReportGroup.Broker;
        var pageRequest = GetValidPageRequest();
        var filter = GetValidFilter();
        filter.BuildingType = BuildingType.Residential;
        var api = GetRequestUrl(pageRequest, groupingBy, filter);
        const int totalCount = 4;
        var returnedItems = new List<ReportDto>()
        {
            new()
            {
                GroupingKey = "A6F7B8C9-7777-4777-8777-777777777777",
                Currency = "RON",
                PolicyCount = 1,
                TotalFinalPremium = 430.57M,
                TotalFinalPremiumInBaseCurrency = 430.57M
            },
            new()
            {
                GroupingKey = "B1A2C3D4-2222-4222-8222-222222222222",
                Currency = "EUR",
                PolicyCount = 1,
                TotalFinalPremium = 2197.56M,
                TotalFinalPremiumInBaseCurrency = 10921.87M
            }
        };

        var response = await HttpClient.GetAsync(api);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        await AssertBodyContainsItems(response, pageRequest, returnedItems, totalCount);
    }

    [Fact]
    public async Task ListAllReportsAsync_GivenBrokerGroupingWithAllFilters_ShouldReturnPagedListOfReports()
    {
        HttpClient.AuthenticateAs(AppRoles.Admin);
        const ReportGroup groupingBy = ReportGroup.Broker;
        var pageRequest = GetValidPageRequest();
        var filter = GetValidFilter();
        filter.Status = PolicyStatus.Active;
        filter.Currency = "EUR";
        filter.BuildingType = BuildingType.Residential;
        var api = GetRequestUrl(pageRequest, groupingBy, filter);
        const int totalCount = 1;
        var returnedItems = new List<ReportDto>()
        {
            new()
            {
                GroupingKey = "B1A2C3D4-2222-4222-8222-222222222222",
                Currency = "EUR",
                PolicyCount = 1,
                TotalFinalPremium = 2197.56M,
                TotalFinalPremiumInBaseCurrency = 10921.87M
            }
        };

        var response = await HttpClient.GetAsync(api);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        await AssertBodyContainsItems(response, pageRequest, returnedItems, totalCount);
    }

    [Fact]
    public async Task ListAllReportsAsync_ThenCreatePolicyForBroker_ThenListReportsAgain_ShouldReturnPagedListOfReportsWithNewlyAddedPolicy()
    {
        HttpClient.AuthenticateAs(AppRoles.Admin);
        const ReportGroup groupingBy = ReportGroup.Broker;
        var pageRequest = GetValidPageRequest();
        var filter = GetValidFilter();
        filter.Currency = "RON";
        var api = GetRequestUrl(pageRequest, groupingBy, filter);
        const int totalCount = 6;
        var returnedItems = new List<ReportDto>()
        {
            new()
            {
                GroupingKey = "A0F1B2C3-1111-4111-8111-111111111111",
                Currency = "RON",
                PolicyCount = 2,
                TotalFinalPremium = 7414.8M,
                TotalFinalPremiumInBaseCurrency = 7414.8M
            },
            new()
            {
                GroupingKey = "A6F7B8C9-7777-4777-8777-777777777777",
                Currency = "RON",
                PolicyCount = 2,
                TotalFinalPremium = 4582.45M,
                TotalFinalPremiumInBaseCurrency = 4582.45M
            }
        };

        var response = await HttpClient.GetAsync(api);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        await AssertBodyContainsItems(response, pageRequest, returnedItems, totalCount);

        HttpClient.AuthenticateAs(AppRoles.Broker);
        await CreateAndAssertPolicy(returnedItems[0].GroupingKey);
        const decimal expectedFinalPremium = 63125M;
        await Task.Delay(1000);

        returnedItems[0].PolicyCount++;
        returnedItems[0].TotalFinalPremium += expectedFinalPremium;
        returnedItems[0].TotalFinalPremiumInBaseCurrency += expectedFinalPremium;

        HttpClient.AuthenticateAs(AppRoles.Admin);
        response = await HttpClient.GetAsync(api);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        await AssertBodyContainsItems(response, pageRequest, returnedItems, totalCount);
    }

    [Fact]
    public async Task ListAllReportsAsync_GivenBrokerGroupingWithInvalidFilters_ShouldReturnValidationError()
    {
        HttpClient.AuthenticateAs(AppRoles.Admin);
        const ReportGroup groupingBy = ReportGroup.Broker;
        var pageRequest = GetValidPageRequest();
        var filter = GetValidFilter();
        filter.From = new DateOnly(2050, 01, 01);
        filter.To = new DateOnly(2040, 01, 01);
        filter.Status = PolicyStatus.Active;
        filter.Currency = "EURO";
        filter.BuildingType = BuildingType.Residential;
        var api = GetRequestUrl(pageRequest, groupingBy, filter);

        var response = await HttpClient.GetAsync(api);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        await AssertBodyContainsValidationErrors(response);
    }

    private PageRequest GetValidPageRequest() => new PageRequest()
    {
        PageNumber = 1,
        PageSize = 2
    };

    private ReportFilter GetValidFilter() => new ReportFilter()
    {
        From = new DateOnly(1000, 1, 1),
        To = new DateOnly(9999, 12, 31)
    };

    private string GetRequestUrl(PageRequest pageRequest, ReportGroup reportGroup, ReportFilter filter)
    {
        var queryParams = new Dictionary<string, string?>
        {
            ["pageSize"] = pageRequest.PageSize.ToString(),
            ["pageNumber"] = pageRequest.PageNumber.ToString(),
            ["groupedBy"] = reportGroup.ToString(),
            ["from"] = filter.From.ToString("yyyy-MM-dd"),
            ["to"] = filter.To.ToString("yyyy-MM-dd"),
            ["currency"] = filter.Currency,
            ["status"] = filter.Status?.ToString(),
            ["buildingType"] = filter.BuildingType?.ToString()
        };

        return QueryHelpers.AddQueryString(ApiV1("/admin/reports"), queryParams);
    }

    private async Task AssertBodyContainsItems(HttpResponseMessage response, PageRequest pageRequest, List<ReportDto> reports, int totalCount)
    {
        var json = await response.Content.ReadAsStringAsync();
        var body = JsonConvert.DeserializeObject<PagedResult<ReportDto>>(json);

        body.Should().NotBeNull();
        body.PageSize.Should().Be(pageRequest.PageSize);
        body.PageNumber.Should().Be(pageRequest.PageNumber);
        body.TotalCount.Should().Be(totalCount);
        body.Items.Should().HaveCount(reports.Count);
        body.Items.Should().BeEquivalentTo(reports);
    }

    private async Task AssertBodyContainsValidationErrors(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        var body = JsonConvert.DeserializeObject<ValidationProblemDetails>(json);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        body.Should().NotBeNull();
        body.Detail.Should().Be("Validation failed.");
        body.Errors.Should().ContainKey("From")
            .WhoseValue.Should().Equal("From Date must be in the past.");
        body.Errors.Should().ContainKey("To")
            .WhoseValue.Should().Equal("From Date must be before the To Date.");
        body.Errors.Should().ContainKey("Currency")
            .WhoseValue.Should().Equal(["Currency must have a length of 3 characters", "Currency does not exist."]);
    }

    private async Task CreateAndAssertPolicy(string brokerId)
    {
        var createPolicyRequest = new
        {
            BrokerId = brokerId,
            BasePremium = 25_000M,
            BuildingId = "a1000044-0000-4000-8000-000000000044",
            ClientId = "b27b4c69-a371-4c4f-8b04-18a4d9e0f018",
            CurrencyCode = "RON",
            StartDate = "2026-06-01",
            EndDate = "2027-06-01"
        };

        var api = ApiV1($"/brokers/policies");
        var response =
            await HttpClient.PostAsJsonAsync(api, createPolicyRequest);
        var policyNumber = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        policyNumber.Should().NotBeEmpty();
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location.AbsolutePath.Should().Be($"{api}/{policyNumber.ToLower()}");
    }
}
