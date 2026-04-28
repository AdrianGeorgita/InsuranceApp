using FluentAssertions;
using InsuranceApp.Application.Brokers.DTOs;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;
using InsuranceApp.Application.Common.Constants;
using InsuranceApp.IntegrationTests.Extensions;

namespace InsuranceApp.IntegrationTests.Admins.Brokers;

public class BrokerTests : IntegrationTestBase
{
    [Fact]
    public async Task ListAllBrokersAsync_ShouldReturnPagedListOfBrokers()
    {
        HttpClient.AuthenticateAs(AppRoles.Admin);
        var pageRequest = new
        {
            PageSize = 2,
            PageNumber = 1,
        };

        var api = $"/api/admin/brokers?pageSize={pageRequest.PageSize}&pageNumber={pageRequest.PageNumber}";

        var response = await HttpClient.GetAsync(api);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadAsStringAsync();
        var body = JsonConvert.DeserializeObject<PagedResult<BrokerDto>>(json);

        body.Should().NotBeNull();
        body.PageSize.Should().Be(pageRequest.PageSize);
        body.PageNumber.Should().Be(pageRequest.PageNumber);
        body.TotalCount.Should().Be(14);
        body.Items.Should().HaveCount(2);
        body.Items.Should().BeEquivalentTo(new List<BrokerDto>
        {
            new()
            {
                Id = new Guid("a0f1b2c3-1111-4111-8111-111111111111"),
                Code = "BRK-RO-001",
                Name = "Alpha Insurance Brokers",
                Email = "contact@alpha-brokers.ro",
                Phone = "+40-721-000-001",
                Status = BrokerStatus.Active,
                CommissionPercentage = 1.2M
            },
            new()
            {
                Id = new Guid("b1a2c3d4-2222-4222-8222-222222222222"),
                Code = "BRK-RO-002",
                Name = "Blue Shield Brokers",
                Email = "office@blueshield.ro",
                Phone = "+40-721-000-002",
                Status = BrokerStatus.Active,
                CommissionPercentage = 1M
            }
        });
    }

    [Fact]
    public async Task GetBrokerByIdAsync_GivenValidBrokerId_ShouldReturnBroker()
    {
        HttpClient.AuthenticateAs(AppRoles.Admin);
        var brokerId = new Guid("a0f1b2c3-1111-4111-8111-111111111111");

        var response = await HttpClient.GetAsync($"/api/admin/brokers/{brokerId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadAsStringAsync();
        var body = JsonConvert.DeserializeObject<BrokerDto>(json);

        body.Should().NotBeNull();
        body.Should().BeEquivalentTo(new
        {
            Id = new Guid("a0f1b2c3-1111-4111-8111-111111111111"),
            Code = "BRK-RO-001",
            Name = "Alpha Insurance Brokers",
            Email = "contact@alpha-brokers.ro",
            Phone = "+40-721-000-001",
            Status = BrokerStatus.Active,
            CommissionPercentage = 1.2M
        });
    }

    [Fact]
    public async Task GetBrokerByIdAsync_GivenNonExistingBrokerId_ShouldReturnNotFound()
    {
        HttpClient.AuthenticateAs(AppRoles.Admin);
        var brokerId = new Guid("f6b9e0a7-8d55-4c6f-8b8e-56a2d7e8f009");
        var api = $"/api/admin/brokers/{brokerId}";

        var response = await HttpClient.GetAsync(api);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var json = await response.Content.ReadAsStringAsync();
        var problem = JsonConvert.DeserializeObject<ProblemDetails>(json);

        problem.Should().NotBeNull();
        problem?.Title.Should().Be("Not Found");
        problem?.Status.Should().Be(404);
        problem?.Detail.Should().Be($"Broker '{brokerId}' not found.");
        problem?.Instance.Should().Be(api);
    }

    [Fact]
    public async Task CreateBroker_GivenValidRequest_ShouldAddBrokerToDatabase()
    {
        HttpClient.AuthenticateAs(AppRoles.Admin);
        var createBrokerRequest = new
        {
            Code = "BRK-RO-999",
            CommissionPercentage = 0.025M,
            Email = "john.johnson@mail.com",
            Name = "John Johnson",
            Phone = "0783652121",
            Status = BrokerStatus.Active
        };

        var createBrokerResponse = await HttpClient.PostAsJsonAsync("/api/admin/brokers", createBrokerRequest);
        var brokerId = await createBrokerResponse.Content.ReadFromJsonAsync<Guid>();

        createBrokerResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        brokerId.Should().NotBeEmpty();
        createBrokerResponse.Headers.Location.Should().NotBeNull();

        var location = createBrokerResponse.Headers.Location.ToString();

        var getBrokerResponse = await HttpClient.GetAsync(location);
        var json = await getBrokerResponse.Content.ReadAsStringAsync();

        getBrokerResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var getBody = JsonConvert.DeserializeObject<BrokerDto>(json);

        getBody.Should().NotBeNull();
        getBody.Should().BeEquivalentTo(new
        {
            Id = brokerId,
            Code = "BRK-RO-999",
            CommissionPercentage = 0.025M,
            Email = "john.johnson@mail.com",
            Name = "John Johnson",
            Phone = "0783652121",
            Status = BrokerStatus.Active
        });
    }

    [Fact]
    public async Task CreateBroker_ThenUpdateBroker_ShouldUpdateBrokerInDatabase()
    {
        HttpClient.AuthenticateAs(AppRoles.Admin);
        var createBrokerRequest = new
        {
            Code = "BRK-RO-999",
            CommissionPercentage = 0.025M,
            Email = "john.johnson@mail.com",
            Name = "John Johnson",
            Phone = "0783652121",
            Status = BrokerStatus.Active
        };

        var createBrokerResponse = await HttpClient.PostAsJsonAsync("/api/admin/brokers", createBrokerRequest);
        var brokerId = await createBrokerResponse.Content.ReadFromJsonAsync<Guid>();

        createBrokerResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        brokerId.Should().NotBeEmpty();
        createBrokerResponse.Headers.Location.Should().NotBeNull();

        var updateBrokerRequest = new
        {
            Phone = "0783652127",
        };

        var updateBrokerResponse = await HttpClient.PatchAsJsonAsync($"/api/admin/brokers/{brokerId}", updateBrokerRequest);
        var updatedBrokerId = await updateBrokerResponse.Content.ReadFromJsonAsync<Guid>();

        updateBrokerResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        updatedBrokerId.Should().NotBeEmpty();

        var getBrokerResponse = await HttpClient.GetAsync($"/api/admin/Brokers/{updatedBrokerId}");
        var json = await getBrokerResponse.Content.ReadAsStringAsync();

        getBrokerResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var getBody = JsonConvert.DeserializeObject<BrokerDto>(json);

        getBody.Should().NotBeNull();
        getBody.Should().BeEquivalentTo(new
        {
            Id = brokerId,
            Code = "BRK-RO-999",
            CommissionPercentage = 0.025M,
            Email = "john.johnson@mail.com",
            Name = "John Johnson",
            Phone = "0783652127",
            Status = BrokerStatus.Active
        });
    }

    [Fact]
    public async Task DeactivateBroker_GivenValidBrokerId_ShouldUpdateBrokerStatusInDatabase()
    {
        HttpClient.AuthenticateAs(AppRoles.Admin);
        var brokerId = new Guid("a0f1b2c3-1111-4111-8111-111111111111");
        var activateBrokerResponse = await HttpClient.PostAsJsonAsync($"/api/admin/brokers/{brokerId}/deactivate", new { });
        var updatedBrokerId = await activateBrokerResponse.Content.ReadFromJsonAsync<Guid>();

        activateBrokerResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        updatedBrokerId.Should().NotBeEmpty();
        updatedBrokerId.Should().Be(updatedBrokerId);
        activateBrokerResponse.Headers.Location.Should().NotBeNull();

        var location = activateBrokerResponse.Headers.Location.ToString();

        var getBrokerResponse = await HttpClient.GetAsync(location);
        var json = await getBrokerResponse.Content.ReadAsStringAsync();

        getBrokerResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var getBody = JsonConvert.DeserializeObject<BrokerDto>(json);

        getBody.Should().NotBeNull();
        getBody.Should().BeEquivalentTo(new
        {
            Id = new Guid("a0f1b2c3-1111-4111-8111-111111111111"),
            Code = "BRK-RO-001",
            Name = "Alpha Insurance Brokers",
            Email = "contact@alpha-brokers.ro",
            Phone = "+40-721-000-001",
            Status = BrokerStatus.Inactive,
            CommissionPercentage = 1.2M
        });
    }

    [Fact]
    public async Task ActivateBroker_GivenValidInactiveBrokerId_ShouldUpdateBrokerStatusInDatabase()
    {
        HttpClient.AuthenticateAs(AppRoles.Admin);
        var brokerId = new Guid("b3a4c5d6-eeee-4eee-8eee-eeeeeeeeeeee");
        var activateBrokerResponse = await HttpClient.PostAsJsonAsync($"/api/admin/brokers/{brokerId}/activate", new { });
        var updatedBrokerId = await activateBrokerResponse.Content.ReadFromJsonAsync<Guid>();

        activateBrokerResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        updatedBrokerId.Should().NotBeEmpty();
        updatedBrokerId.Should().Be(updatedBrokerId);
        activateBrokerResponse.Headers.Location.Should().NotBeNull();

        var location = activateBrokerResponse.Headers.Location.ToString();

        var getBrokerResponse = await HttpClient.GetAsync(location);
        var json = await getBrokerResponse.Content.ReadAsStringAsync();

        getBrokerResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var getBody = JsonConvert.DeserializeObject<BrokerDto>(json);

        getBody.Should().NotBeNull();
        getBody.Should().BeEquivalentTo(new
        {
            Id = new Guid("b3a4c5d6-eeee-4eee-8eee-eeeeeeeeeeee"),
            Code = "BRK-RO-014",
            Name = "NorthStar Brokers",
            Email = "office@northstar.ro",
            Phone = "+40-721-000-014",
            Status = BrokerStatus.Active,
        });
    }

    [Fact]
    public async Task DeactivateBroker_ThenCreatePolicy_GivenValidInactiveBrokerId_ShoulReturnValidationError()
    {
        HttpClient.AuthenticateAs(AppRoles.Admin);
        var brokerId = new Guid("a0f1b2c3-1111-4111-8111-111111111111");
        var activateBrokerResponse = await HttpClient.PostAsJsonAsync($"/api/admin/brokers/{brokerId}/deactivate", new { });
        var updatedBrokerId = await activateBrokerResponse.Content.ReadFromJsonAsync<Guid>();

        activateBrokerResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        updatedBrokerId.Should().NotBeEmpty();
        updatedBrokerId.Should().Be(updatedBrokerId);
        activateBrokerResponse.Headers.Location.Should().NotBeNull();

        var location = activateBrokerResponse.Headers.Location.ToString();

        var getBrokerResponse = await HttpClient.GetAsync(location);
        var json = await getBrokerResponse.Content.ReadAsStringAsync();

        getBrokerResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var getBody = JsonConvert.DeserializeObject<BrokerDto>(json);

        getBody.Should().NotBeNull();
        getBody.Should().BeEquivalentTo(new
        {
            Id = new Guid("a0f1b2c3-1111-4111-8111-111111111111"),
            Code = "BRK-RO-001",
            Name = "Alpha Insurance Brokers",
            Email = "contact@alpha-brokers.ro",
            Phone = "+40-721-000-001",
            Status = BrokerStatus.Inactive,
            CommissionPercentage = 1.2M
        });

        HttpClient.AuthenticateAs(AppRoles.Broker);
        var createPolicyRequest = new
        {
            BrokerId = brokerId,
            BasePremium = 50_000M,
            BuildingId = "a1000031-0000-4000-8000-000000000031",
            ClientId = "6a8f1f0d-9a8c-4c42-8bfa-3d4a5b7e9c22",
            CurrencyCode = "RON",
            StartDate = "2026-04-01",
            EndDate = "2027-04-01"
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
        body.Errors.Should().ContainKey("BrokerId")
            .WhoseValue.Should().Equal("Broker does not exist or is inactive.");
    }
}