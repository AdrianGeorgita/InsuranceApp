using FluentAssertions;
using InsuranceApp.Application.Common.Constants;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Policies.DTOs;
using InsuranceApp.Domain.Enums;
using InsuranceApp.IntegrationTests.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;

namespace InsuranceApp.IntegrationTests.Brokers.Policies;

public class PoliciesTests : IntegrationTestBase
{
    [Fact]
    public async Task CreateClient_ThenRegisterBuilding_ThenCreatePolicy_ThenActivatePolicy_ShouldAddClientAndBuildingAndPolicyToDatabase()
    {
        HttpClient.AuthenticateAs(AppRoles.Broker);
        var createClientRequest = new
        {
            Type = "Individual",
            Name = "John Does",
            IdentificationNumber = "3020202123129",
            Email = "john.does@mail.com",
            Phone = "0798786537",
            Address = "John Does Residence Nr.7"
        };

        var clientResponse = await HttpClient.PostAsJsonAsync("/api/brokers/clients", createClientRequest);
        var clientId = await clientResponse.Content.ReadFromJsonAsync<Guid>();

        clientResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        clientId.Should().NotBeEmpty();
        clientResponse.Headers.Location.Should().NotBeNull();
        clientResponse.Headers.Location.AbsolutePath.Should().Be($"/api/brokers/clients/{clientId}");

        var createBuildingRequest = new
        {
            Address = "Some random street Nr.7",
            CityId = "a0040001-4444-4444-8444-000000000001",
            ConstructionYear = 1970,
            BuildingType = "Industrial",
            NumberOfFloors = 3,
            SurfaceArea = 135.23M,
            InsuredValue = 125236235,
            RiskIndicators = new List<int> { 1, 2 }
        };

        var createBuildingResponse =
            await HttpClient.PostAsJsonAsync($"/api/brokers/clients/{clientId}/buildings", createBuildingRequest);
        var buildingId = await createBuildingResponse.Content.ReadFromJsonAsync<Guid>();

        createBuildingResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        buildingId.Should().NotBeEmpty();
        createBuildingResponse.Headers.Location.Should().NotBeNull();
        createBuildingResponse.Headers.Location.AbsolutePath.Should().Be($"/api/brokers/buildings/{buildingId}");

        var createPolicyRequest = new
        {
            BrokerId = "c8b9d0e1-9999-4999-8999-999999999999",
            BasePremium = 50_000M,
            BuildingId = buildingId,
            ClientId = clientId,
            CurrencyCode = "RON",
            StartDate = DateTime.UtcNow.AddDays(5).ToString("yyyy-MM-dd"),
            EndDate = DateTime.UtcNow.AddDays(5).AddYears(1).ToString("yyyy-MM-dd")
        };

        var createPolicyResponse =
            await HttpClient.PostAsJsonAsync($"/api/brokers/policies", createPolicyRequest);
        var policyNumber = await createPolicyResponse.Content.ReadAsStringAsync();

        createPolicyResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        policyNumber.Should().NotBeEmpty();
        createPolicyResponse.Headers.Location.Should().NotBeNull();
        createPolicyResponse.Headers.Location.AbsolutePath.Should().Be($"/api/brokers/policies/{policyNumber.ToLower()}");

        var location = createPolicyResponse.Headers.Location.ToString();

        var getPolicyResponse = await HttpClient.GetAsync(location);
        var json = await getPolicyResponse.Content.ReadAsStringAsync();

        getPolicyResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var getBody = JsonConvert.DeserializeObject<DetailedPolicyDto>(json);

        getBody.Should().NotBeNull();
        getBody.Should().BeEquivalentTo(new
        {
            PolicyNumber = policyNumber,
            Client = new
            {
                Id = clientId,
                Email = "john.does@mail.com",
                Address = "John Does Residence Nr.7",
                IdentificationNumber = "3020202123129",
                Phone = "0798786537",
                Name = "John Does",
                Type = "Individual"
            },
            Building = new
            {
                Id = buildingId,
                Address = "Some random street Nr.7",
                ConstructionYear = 1970,
                BuildingType = "Industrial",
                NumberOfFloors = 3,
                SurfaceArea = 135.23M,
                InsuredValue = 125236235,
            },
            BrokerId = new Guid("c8b9d0e1-9999-4999-8999-999999999999"),
            Status = PolicyStatus.Draft,
            CurrencyCode = "RON",
            BasePremium = 50000.0000M,
            FinalPremium = 99750.0000M,
        });

        var activatePolicyResponse =
            await HttpClient.PostAsJsonAsync($"/api/brokers/policies/{policyNumber}/activate", new { });
        policyNumber = await activatePolicyResponse.Content.ReadAsStringAsync();

        activatePolicyResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        policyNumber.Should().NotBeEmpty();
        activatePolicyResponse.Headers.Location.Should().NotBeNull();
        activatePolicyResponse.Headers.Location.AbsolutePath.Should().Be($"/api/brokers/policies/{policyNumber.ToLower()}");

        location = createPolicyResponse.Headers.Location.ToString();

        getPolicyResponse = await HttpClient.GetAsync(location);
        json = await getPolicyResponse.Content.ReadAsStringAsync();

        getPolicyResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        getBody = JsonConvert.DeserializeObject<DetailedPolicyDto>(json);

        getBody.Should().NotBeNull();
        getBody.Status.Should().Be(PolicyStatus.Active);
    }

    [Fact]
    public async Task CreatePolicy_GivenInvalidRequest_ShouldNotCreatePolicyAndReturnValidationError()
    {
        HttpClient.AuthenticateAs(AppRoles.Broker);
        var createPolicyRequest = new
        {
            BrokerId = "c8b9d0e1-9999-4999-8999-999999999999",
            BasePremium = -50_000M,
            BuildingId = Guid.NewGuid(),
            ClientId = "4b04d5f2-3c0a-4b1e-9a3d-a1f7c2d3e011",
            CurrencyCode = "RON",
            StartDate = DateTime.UtcNow.AddDays(5).ToString("yyyy-MM-dd"),
            EndDate = DateTime.UtcNow.AddDays(4).ToString("yyyy-MM-dd")
        };

        var createPolicyResponse =
            await HttpClient.PostAsJsonAsync($"/api/brokers/policies", createPolicyRequest);
        var policyNumber = await createPolicyResponse.Content.ReadAsStringAsync();
        var bodyJson = await createPolicyResponse.Content.ReadAsStringAsync();
        var body = JsonConvert.DeserializeObject<ValidationProblemDetails>(bodyJson);

        createPolicyResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        policyNumber.Should().NotBeEmpty();
        body.Should().NotBeNull();
        body.Detail.Should().Be("Validation failed.");
        body.Errors.Should().ContainKey("BuildingId")
            .WhoseValue.Should().Equal("Building does not exist.");
        body.Errors.Should().ContainKey("EndDate")
            .WhoseValue.Should().Equal("EndDate must be after the StartDate.");
        body.Errors.Should().ContainKey("BasePremium")
            .WhoseValue.Should().Equal("BasePremium is required and must be greater than 0.");
    }

    [Fact]
    public async Task ListAllPoliciesAsync_GivenNoFilter_ShouldReturnPagedListOfPolicies()
    {
        HttpClient.AuthenticateAs(AppRoles.Broker);
        var pageRequest = new
        {
            PageSize = 2,
            PageNumber = 1,
        };

        var api = $"/api/brokers/policies?pageSize={pageRequest.PageSize}&pageNumber={pageRequest.PageNumber}";

        var response = await HttpClient.GetAsync(api);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadAsStringAsync();
        var body = JsonConvert.DeserializeObject<PagedResult<PolicyDto>>(json);

        body.Should().NotBeNull();
        body.PageSize.Should().Be(pageRequest.PageSize);
        body.PageNumber.Should().Be(pageRequest.PageNumber);
        body.TotalCount.Should().Be(25);
        body.Items.Should().HaveCount(2);
        body.Items.Should().BeEquivalentTo(new List<PolicyDto>
        {
            new()
            {
                PolicyNumber = "POL-00001",
                ClientId = new Guid("7fc17474-00f8-424a-9a0c-58fbddd1db73"),
                BuildingId = new Guid("a1000001-0000-4000-8000-000000000001"),
                BrokerId = new Guid("a0f1b2c3-1111-4111-8111-111111111111"),
                Status = PolicyStatus.Expired,
                StartDate = new DateTime(2020,02,01,00,00,00, DateTimeKind.Utc),
                EndDate = new DateTime(2021,02,01,00,00,00, DateTimeKind.Utc),
                BasePremium = 4250M,
                CurrencyCode = "RON",
                FinalPremium = 4352.61M
            },
            new()
            {
                PolicyNumber = "POL-00002",
                ClientId = new Guid("1b2d7b9f-3c9e-4c6f-9f63-1f6f5f9c2a01"),
                BuildingId = new Guid("a1000002-0000-4000-8000-000000000002"),
                BrokerId = new Guid("b1a2c3d4-2222-4222-8222-222222222222"),
                Status = PolicyStatus.Active,
                StartDate = new DateTime(2022,06,15,00,00,00, DateTimeKind.Utc),
                EndDate = new DateTime(2027,06,14,00,00,00, DateTimeKind.Utc),
                BasePremium = 2150M,
                CurrencyCode = "EUR",
                FinalPremium = 2197.56M
            }
        });
    }

    [Fact]
    public async Task ListAllPoliciesAsync_GivenOutOfRangePage_ShouldReturnPagedEmptyListOfPolicies()
    {
        HttpClient.AuthenticateAs(AppRoles.Broker);
        var pageRequest = new
        {
            PageSize = 10,
            PageNumber = 12512,
        };

        var api = $"/api/brokers/policies?pageSize={pageRequest.PageSize}&pageNumber={pageRequest.PageNumber}";

        var response = await HttpClient.GetAsync(api);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadAsStringAsync();
        var body = JsonConvert.DeserializeObject<PagedResult<PolicyDto>>(json);

        body.Should().NotBeNull();
        body.PageSize.Should().Be(pageRequest.PageSize);
        body.PageNumber.Should().Be(pageRequest.PageNumber);
        body.TotalCount.Should().Be(25);
        body.Items.Should().HaveCount(0);
        body.Items.Should().BeEquivalentTo(new List<PolicyDto> { });
    }

    [Fact]
    public async Task ListAllPoliciesAsync_GivenFilter_ShouldReturnPagedFilteredListOfPolicies()
    {
        HttpClient.AuthenticateAs(AppRoles.Broker);
        var pageRequest = new
        {
            PageSize = 1,
            PageNumber = 1,
        };

        var filter = new
        {
            Status = PolicyStatus.Active,
            StartDate = new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc)
        };

        var api = $"/api/brokers/policies?pageSize={pageRequest.PageSize}&pageNumber=" +
                  $"{pageRequest.PageNumber}&status={filter.Status}&startDate={filter.StartDate}";

        var response = await HttpClient.GetAsync(api);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadAsStringAsync();
        var body = JsonConvert.DeserializeObject<PagedResult<PolicyDto>>(json);

        body.Should().NotBeNull();
        body.PageSize.Should().Be(pageRequest.PageSize);
        body.PageNumber.Should().Be(pageRequest.PageNumber);
        body.TotalCount.Should().Be(2);
        body.Items.Should().HaveCount(1);
        body.Items.Should().BeEquivalentTo(new List<PolicyDto>
        {
            new()
            {
                PolicyNumber = "POL-00009",
                ClientId = new Guid("f6b9e0a7-8d55-4c6f-8b8e-56a2d7e8f006"),
                BuildingId = new Guid("a1000009-0000-4000-8000-000000000009"),
                BrokerId = new Guid("c8b9d0e1-9999-4999-8999-999999999999"),
                Status = PolicyStatus.Active,
                StartDate = new DateTime(2025,01,01,00,00,00, DateTimeKind.Utc),
                EndDate = new DateTime(2026,12,31,00,00,00, DateTimeKind.Utc),
                BasePremium = 2450M,
                CurrencyCode = "GBP",
                FinalPremium = 2496.76M
            }
        });
    }

    [Fact]
    public async Task GetPolicyByIdAsync_GivenValidPolicyNumber_ShouldReturnDetailedPolicy()
    {
        HttpClient.AuthenticateAs(AppRoles.Broker);
        const string policyNumber = "POL-00001";

        var response = await HttpClient.GetAsync($"/api/brokers/policies/{policyNumber}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadAsStringAsync();
        var body = JsonConvert.DeserializeObject<DetailedPolicyDto>(json);

        body.Should().NotBeNull();
        body.Should().BeEquivalentTo(new
        {
            PolicyNumber = policyNumber,
            Client = new
            {
                Id = new Guid("7fc17474-00f8-424a-9a0c-58fbddd1db73"),
                Email = "john.doe@mail.com",
                Address = "Some random street No.1",
                IdentificationNumber = "3021212123123",
                Phone = "0712345678",
                Name = "John Doe",
                Type = "Individual"
            },
            Building = new
            {
                Id = new Guid("a1000001-0000-4000-8000-000000000001"),
                Address = "Str. Industriilor 10",
                ConstructionYear = 1985,
                BuildingType = "Industrial",
                NumberOfFloors = 2,
                SurfaceArea = 850M,
                InsuredValue = 4200000M,
            },
            BrokerId = new Guid("a0f1b2c3-1111-4111-8111-111111111111"),
            Status = PolicyStatus.Expired,
            CurrencyCode = "RON",
            StartDate = new DateTime(2020, 02, 01, 0, 0, 0, DateTimeKind.Utc),
            EndDate = new DateTime(2021, 02, 01, 0, 0, 0, DateTimeKind.Utc),
            BasePremium = 4250M,
            FinalPremium = 4352.61M,
        });
    }

    [Fact]
    public async Task GetPolicyByIdAsync_GivenNonExistingPolicyNumber_ShouldReturnNotFound()
    {
        HttpClient.AuthenticateAs(AppRoles.Broker);
        const string policyNumber = "POLICY-f6b9e0a7-8d55-4c6f-8b8e-56a2d7e8f009";
        var api = $"/api/brokers/policies/{policyNumber}";

        var response = await HttpClient.GetAsync(api);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var json = await response.Content.ReadAsStringAsync();
        var problem = JsonConvert.DeserializeObject<ProblemDetails>(json);

        problem.Should().NotBeNull();
        problem?.Title.Should().Be("Not Found");
        problem?.Status.Should().Be(404);
        problem?.Detail.Should().Be($"Policy '{policyNumber}' not found.");
        problem?.Instance.Should().Be(api);
    }

    [Fact]
    public async Task CancelPolicyByIdAsync_GivenValidActivePolicy_ShouldCancelPolicy()
    {
        HttpClient.AuthenticateAs(AppRoles.Broker);
        const string policyNumber = "POL-00003";

        var cancelPolicyResponse =
            await HttpClient.PostAsJsonAsync($"/api/brokers/policies/{policyNumber}/cancel",
                new { Reason = "Building demolished" });
        var updatedPolicyNumber = await cancelPolicyResponse.Content.ReadAsStringAsync();

        cancelPolicyResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        policyNumber.Should().NotBeEmpty();
        updatedPolicyNumber.Should().Be(policyNumber);
        cancelPolicyResponse.Headers.Location.Should().NotBeNull();
        cancelPolicyResponse.Headers.Location.AbsolutePath.Should().Be($"/api/brokers/policies/{updatedPolicyNumber.ToLower()}");

        var location = cancelPolicyResponse.Headers.Location.ToString();

        var getPolicyResponse = await HttpClient.GetAsync(location);
        var json = await getPolicyResponse.Content.ReadAsStringAsync();

        getPolicyResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var getBody = JsonConvert.DeserializeObject<DetailedPolicyDto>(json);

        getBody.Should().NotBeNull();
        getBody.Status.Should().Be(PolicyStatus.Cancelled);
    }

    [Fact]
    public async Task CancelPolicyByIdAsync_GivenCancelledPolicy_ShouldReturnValidationError()
    {
        HttpClient.AuthenticateAs(AppRoles.Broker);
        const string policyNumber = "POL-00022";

        var cancelPolicyResponse =
            await HttpClient.PostAsJsonAsync($"/api/brokers/policies/{policyNumber}/cancel",
                new { Reason = "Building demolished" });
        var bodyJson = await cancelPolicyResponse.Content.ReadAsStringAsync();
        var body = JsonConvert.DeserializeObject<ProblemDetails>(bodyJson);


        cancelPolicyResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        policyNumber.Should().NotBeEmpty();
        body.Should().NotBeNull();
        body.Detail.Should().Be("Cannot change Policy Status from Cancelled to Cancelled.");
        body.Title.Should().Be("Bad Request");
    }

    [Fact]
    public async Task ActivatePolicyByIdAsync_GivenExpiredPolicy_ShouldReturnValidationError()
    {
        HttpClient.AuthenticateAs(AppRoles.Broker);
        const string policyNumber = "POL-00011";

        var activatePolicyResponse =
            await HttpClient.PostAsJsonAsync($"/api/brokers/policies/{policyNumber}/activate",
                new { Reason = "Building demolished" });
        var bodyJson = await activatePolicyResponse.Content.ReadAsStringAsync();
        var body = JsonConvert.DeserializeObject<ProblemDetails>(bodyJson);


        activatePolicyResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        policyNumber.Should().NotBeEmpty();
        body.Should().NotBeNull();
        body.Detail.Should().Be("Cannot change Policy Status from Draft to Active.");
        body.Title.Should().Be("Bad Request");
    }
}