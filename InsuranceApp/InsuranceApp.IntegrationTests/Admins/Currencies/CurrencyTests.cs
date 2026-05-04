using FluentAssertions;
using InsuranceApp.Application.Common.Constants;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Metadata.Currencies.DTOs;
using InsuranceApp.IntegrationTests.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;

namespace InsuranceApp.IntegrationTests.Admins.Currencies;

public class CurrencyTests : IntegrationTestBase
{
    [Fact]
    public async Task ListAllCurrenciesAsync_ShouldReturnPagedListOfCurrencies()
    {
        HttpClient.AuthenticateAs(AppRoles.Admin);
        var pageRequest = new
        {
            PageSize = 2,
            PageNumber = 1,
        };

        var api = ApiV1($"/admin/currencies?pageSize={pageRequest.PageSize}&pageNumber={pageRequest.PageNumber}");

        var response = await HttpClient.GetAsync(api);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadAsStringAsync();
        var body = JsonConvert.DeserializeObject<PagedResult<CurrencyDto>>(json);

        body.Should().NotBeNull();
        body.PageSize.Should().Be(pageRequest.PageSize);
        body.PageNumber.Should().Be(pageRequest.PageNumber);
        body.TotalCount.Should().Be(20);
        body.Items.Should().HaveCount(2);
        body.Items.Should().BeEquivalentTo(new List<CurrencyDto>
        {
            new()
            {
                Code = "AUD",
                Name = "Australian Dollar",
                ExchangeRateToBase = 3.02M,
                IsActive = true,
                Deprecated = false
            },
            new()
            {
                Code = "BGN",
                Name = "Bulgarian Lev",
                ExchangeRateToBase = 2.54M,
                IsActive = true,
                Deprecated = false
            }
        });
    }

    [Fact]
    public async Task GetCurrencyByCodeAsync_GivenValidCurrencyCode_ShouldReturnCurrency()
    {
        HttpClient.AuthenticateAs(AppRoles.Admin);
        const string currencyCode = "RON";

        var api = ApiV1($"/admin/currencies/{currencyCode}");
        var response = await HttpClient.GetAsync(api);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadAsStringAsync();
        var body = JsonConvert.DeserializeObject<CurrencyDto>(json);

        body.Should().NotBeNull();
        body.Should().BeEquivalentTo(new
        {
            Code = "RON",
            Name = "Romanian Leu",
            ExchangeRateToBase = 1.0M,
            IsActive = true
        });
    }

    [Fact]
    public async Task GetCurrencyByCodeAsync_GivenNonExistingCurrencyCode_ShouldReturnNotFound()
    {
        HttpClient.AuthenticateAs(AppRoles.Admin);
        const string currencyCode = "XYZ";
        var api = ApiV1($"/admin/currencies/{currencyCode}");

        var response = await HttpClient.GetAsync(api);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var json = await response.Content.ReadAsStringAsync();
        var problem = JsonConvert.DeserializeObject<ProblemDetails>(json);

        problem.Should().NotBeNull();
        problem?.Title.Should().Be("Not Found");
        problem?.Status.Should().Be(404);
        problem?.Detail.Should().Be($"Currency '{currencyCode}' not found.");
        problem?.Instance.Should().Be(api);
    }

    [Fact]
    public async Task CreateCurrency_GivenValidRequest_ShouldAddCurrencyToDatabase()
    {
        HttpClient.AuthenticateAs(AppRoles.Admin);
        var createCurrencyRequest = new CreateCurrencyRequest()
        {
            Code = "RBX",
            Name = "Robux",
            ExchangeRateToBase = 0.0163M,
            IsActive = true
        };

        var api = ApiV1("/admin/currencies");
        var createCurrencyResponse = await HttpClient.PostAsJsonAsync(api, createCurrencyRequest);
        var currencyCode = await createCurrencyResponse.Content.ReadAsStringAsync();

        createCurrencyResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        currencyCode.Should().NotBeEmpty();
        currencyCode.Should().Be("RBX");
        createCurrencyResponse.Headers.Location.Should().NotBeNull();

        var location = createCurrencyResponse.Headers.Location.ToString();

        var getCurrencyResponse = await HttpClient.GetAsync(location);
        var json = await getCurrencyResponse.Content.ReadAsStringAsync();

        getCurrencyResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var getBody = JsonConvert.DeserializeObject<CurrencyDto>(json);

        getBody.Should().NotBeNull();
        getBody.Should().BeEquivalentTo(new
        {
            Code = "RBX",
            Name = "Robux",
            ExchangeRateToBase = 0.0163M,
            IsActive = true,
            Deprecated = false
        });
    }

    [Fact]
    public async Task CreateCurrency_ThenUpdateCurrency_ShouldUpdateCurrencyInDatabase()
    {
        HttpClient.AuthenticateAs(AppRoles.Admin);
        var createCurrencyRequest = new CreateCurrencyRequest()
        {
            Code = "RBX",
            Name = "Robux",
            ExchangeRateToBase = 0.0163M,
            IsActive = true
        };

        var api = ApiV1("/admin/currencies");
        var createCurrencyResponse = await HttpClient.PostAsJsonAsync(api, createCurrencyRequest);
        var currencyCode = await createCurrencyResponse.Content.ReadAsStringAsync();

        createCurrencyResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        currencyCode.Should().NotBeEmpty();
        currencyCode.Should().Be("RBX");
        createCurrencyResponse.Headers.Location.Should().NotBeNull();

        var updateCurrencyRequest = new
        {
            IsActive = false,
        };

        api = ApiV1($"/admin/currencies/{currencyCode}");
        var updateCurrencyResponse = await HttpClient.PatchAsJsonAsync(api, updateCurrencyRequest);
        var updatedCurrencyCode = await updateCurrencyResponse.Content.ReadAsStringAsync();

        updateCurrencyResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        updatedCurrencyCode.Should().NotBeEmpty();
        updatedCurrencyCode.Should().Be("RBX");

        api = ApiV1($"/admin/currencies/{updatedCurrencyCode}");
        var getCurrencyResponse = await HttpClient.GetAsync(api);
        var json = await getCurrencyResponse.Content.ReadAsStringAsync();

        getCurrencyResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var getBody = JsonConvert.DeserializeObject<CurrencyDto>(json);

        getBody.Should().NotBeNull();
        getBody.Should().BeEquivalentTo(new
        {
            Code = "RBX",
            Name = "Robux",
            ExchangeRateToBase = 0.0163M,
            IsActive = false,
            Deprecated = true
        });
    }

    [Fact]
    public async Task UpdateCurrency_SetInactive_ThenCreatePolicy_ShouldSetAsDeprecatedAndReturnValidationError()
    {
        HttpClient.AuthenticateAs(AppRoles.Admin);
        const string currencyCode = "HUF";
        var updateCurrencyRequest = new
        {
            IsActive = false,
        };

        var api = ApiV1($"/admin/currencies/{currencyCode}");
        var updateCurrencyResponse = await HttpClient.PatchAsJsonAsync(api, updateCurrencyRequest);
        var updatedCurrencyCode = await updateCurrencyResponse.Content.ReadAsStringAsync();

        updateCurrencyResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        updatedCurrencyCode.Should().NotBeEmpty();
        updatedCurrencyCode.Should().Be(currencyCode);

        api = ApiV1($"/admin/currencies/{updatedCurrencyCode}");
        var getCurrencyResponse = await HttpClient.GetAsync(api);
        var json = await getCurrencyResponse.Content.ReadAsStringAsync();

        getCurrencyResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var getBody = JsonConvert.DeserializeObject<CurrencyDto>(json);

        getBody.Should().NotBeNull();
        getBody.Should().BeEquivalentTo(new
        {
            Code = "HUF",
            Name = "Hungarian Forint",
            ExchangeRateToBase = 0.0127M,
            IsActive = true,
            Deprecated = true
        });

        HttpClient.AuthenticateAs(AppRoles.Broker);
        var createPolicyRequest = new
        {
            BrokerId = "c8b9d0e1-9999-4999-8999-999999999999",
            BasePremium = -50_000M,
            BuildingId = "a1000031-0000-4000-8000-000000000031",
            ClientId = "6a8f1f0d-9a8c-4c42-8bfa-3d4a5b7e9c22",
            CurrencyCode = "HUF",
            StartDate = new DateTime(2026, 04, 02, 0, 0, 0, DateTimeKind.Utc),
            EndDate = new DateTime(2027, 04, 02, 0, 0, 0, DateTimeKind.Utc)
        };

        api = ApiV1($"/brokers/policies");
        var createPolicyResponse =
            await HttpClient.PostAsJsonAsync(api, createPolicyRequest);
        var policyNumber = await createPolicyResponse.Content.ReadAsStringAsync();
        var bodyJson = await createPolicyResponse.Content.ReadAsStringAsync();
        var body = JsonConvert.DeserializeObject<ValidationProblemDetails>(bodyJson);

        createPolicyResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        policyNumber.Should().NotBeEmpty();
        body.Should().NotBeNull();
        body.Detail.Should().Be("Validation failed.");
        body.Errors.Should().ContainKey("CurrencyCode")
            .WhoseValue.Should().Equal("Currency does not exist or is deprecated.");
    }

    [Fact]
    public async Task UpdateCurrency_SetInactive_GivenUnusedCurrency_ShouldSetAsInactive()
    {
        HttpClient.AuthenticateAs(AppRoles.Admin);
        const string currencyCode = "BGN";
        var updateCurrencyRequest = new
        {
            IsActive = false,
        };

        var api = ApiV1($"/admin/currencies/{currencyCode}");
        var updateCurrencyResponse = await HttpClient.PatchAsJsonAsync(api, updateCurrencyRequest);
        var updatedCurrencyCode = await updateCurrencyResponse.Content.ReadAsStringAsync();

        updateCurrencyResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        updatedCurrencyCode.Should().NotBeEmpty();
        updatedCurrencyCode.Should().Be(currencyCode);

        api = ApiV1($"/admin/currencies/{updatedCurrencyCode}");
        var getCurrencyResponse = await HttpClient.GetAsync(api);
        var json = await getCurrencyResponse.Content.ReadAsStringAsync();

        getCurrencyResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var getBody = JsonConvert.DeserializeObject<CurrencyDto>(json);

        getBody.Should().NotBeNull();
        getBody.Should().BeEquivalentTo(new
        {
            Code = "BGN",
            Name = "Bulgarian Lev",
            ExchangeRateToBase = 2.5400M,
            IsActive = false,
            Deprecated = true
        });
    }
}