using FluentAssertions;
using InsuranceApp.Application.Clients.DTOs;
using InsuranceApp.Application.Common.Constants;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.IntegrationTests.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;

namespace InsuranceApp.IntegrationTests.Brokers.Clients;

public class ClientTests : IntegrationTestBase
{
    [Fact]
    public async Task ListAllClientsAsync_GivenNoFilter_ShouldReturnPagedListOfClients()
    {
        HttpClient.AuthenticateAs(AppRoles.Broker);
        var pageRequest = new
        {
            PageSize = 2,
            PageNumber = 1,
        };

        var api = $"/api/brokers/clients?pageSize={pageRequest.PageSize}&pageNumber={pageRequest.PageNumber}";

        var clientResponse = await HttpClient.GetAsync(api);

        clientResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await clientResponse.Content.ReadAsStringAsync();
        var body = JsonConvert.DeserializeObject<PagedResult<ClientDto>>(json);

        body.Should().NotBeNull();
        body.PageSize.Should().Be(pageRequest.PageSize);
        body.PageNumber.Should().Be(pageRequest.PageNumber);
        body.TotalCount.Should().Be(26);
        body.Items.Should().HaveCount(2);
        body.Items.Should().BeEquivalentTo(new List<ClientDto>
        {
            new()
            {
                Id = new Guid("a1d4f1a7-1f2e-4b5b-9a2d-01f1e8a3c001"),
                Type = "Individual",
                Name = "Andrei Marinescu",
                IdentificationNumber = "1950305123456",
                Email = "andrei.marinescu@mail.com",
                Phone = "0721112233",
                Address = "Str. Mihai Eminescu Nr. 12, Bucharest"
            },
            new()
            {
                Id = new Guid("a16a3b58-9260-4b5e-9a93-07f3c8d9e017"),
                Type = "Company",
                Name = "Smart Retail Group SA",
                IdentificationNumber = "12349876",
                Email = "office@smartretail.ro",
                Phone = "0212349876",
                Address = "Bd. Comerciala Nr. 56, Bucharest"
            }
        });
    }

    [Fact]
    public async Task ListAllClientsAsync_GivenOutOfRangePage_ShouldReturnPagedEmptyListOfClients()
    {
        HttpClient.AuthenticateAs(AppRoles.Broker);
        var pageRequest = new
        {
            PageSize = 10,
            PageNumber = 12512,
        };

        var api = $"/api/brokers/clients?pageSize={pageRequest.PageSize}&pageNumber={pageRequest.PageNumber}";

        var clientResponse = await HttpClient.GetAsync(api);

        clientResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await clientResponse.Content.ReadAsStringAsync();
        var body = JsonConvert.DeserializeObject<PagedResult<ClientDto>>(json);

        body.Should().NotBeNull();
        body.PageSize.Should().Be(pageRequest.PageSize);
        body.PageNumber.Should().Be(pageRequest.PageNumber);
        body.TotalCount.Should().Be(26);
        body.Items.Should().HaveCount(0);
        body.Items.Should().BeEquivalentTo(new List<ClientDto> { });
    }

    [Fact]
    public async Task ListAllClientsAsync_GivenInvalidPageRequest_ShouldReturnBadRequest()
    {
        HttpClient.AuthenticateAs(AppRoles.Broker);
        var pageRequest = new
        {
            PageSize = 160,
            PageNumber = 1,
        };

        var api = $"/api/brokers/clients?pageSize={pageRequest.PageSize}&pageNumber={pageRequest.PageNumber}";

        var clientResponse = await HttpClient.GetAsync(api);

        clientResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var json = await clientResponse.Content.ReadAsStringAsync();
        var problem = JsonConvert.DeserializeObject<ProblemDetails>(json);

        problem.Should().NotBeNull();
        problem?.Title.Should().Be("One or more validation errors occurred.");
        problem?.Status.Should().Be(400);
    }

    [Fact]
    public async Task ListAllClientsAsync_GivenFilter_ShouldReturnPagedFilteredListOfClients()
    {
        HttpClient.AuthenticateAs(AppRoles.Broker);
        var pageRequest = new
        {
            PageSize = 2,
            PageNumber = 1,
        };

        var filter = new
        {
            Name = "escu"
        };

        var api = $"/api/brokers/clients?pageSize={pageRequest.PageSize}&pageNumber={pageRequest.PageNumber}&name={filter.Name}";

        var clientResponse = await HttpClient.GetAsync(api);

        clientResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await clientResponse.Content.ReadAsStringAsync();
        var body = JsonConvert.DeserializeObject<PagedResult<ClientDto>>(json);

        body.Should().NotBeNull();
        body.PageSize.Should().Be(pageRequest.PageSize);
        body.PageNumber.Should().Be(pageRequest.PageNumber);
        body.TotalCount.Should().Be(4);
        body.Items.Should().HaveCount(2);
        body.Items.Should().BeEquivalentTo(new List<ClientDto>
        {
            new()
            {
                Id = new Guid("a1d4f1a7-1f2e-4b5b-9a2d-01f1e8a3c001"),
                Type = "Individual",
                Name = "Andrei Marinescu",
                IdentificationNumber = "1950305123456",
                Email = "andrei.marinescu@mail.com",
                Phone = "0721112233",
                Address = "Str. Mihai Eminescu Nr. 12, Bucharest"
            },
            new()
            {
                Id = new Guid("b2c5a6e3-4d91-4e1b-8b3a-12c9f3d4a002"),
                Type = "Individual",
                Name = "Elena Dumitrescu",
                IdentificationNumber = "2960829123456",
                Email = "elena.dumitrescu@mail.com",
                Phone = "0732223344",
                Address = "Bd. Unirii Nr. 88, Bucharest"
            }
        });
    }

    [Fact]
    public async Task GetClientByIdAsync_GivenValidClientId_ShouldReturnClient()
    {
        HttpClient.AuthenticateAs(AppRoles.Broker);
        var clientId = new Guid("f6b9e0a7-8d55-4c6f-8b8e-56a2d7e8f006");

        var clientResponse = await HttpClient.GetAsync($"/api/brokers/clients/{clientId}");

        clientResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await clientResponse.Content.ReadAsStringAsync();
        var body = JsonConvert.DeserializeObject<ClientDto>(json);

        body.Should().NotBeNull();
        body.Should().BeEquivalentTo(new
        {
            Id = clientId,
            Type = "Individual",
            Name = "Simona Rusu",
            IdentificationNumber = "2920922123456",
            Email = "simona.rusu@mail.com",
            Phone = "0776667788",
            Address = "Str. Stefan cel Mare Nr. 33, Iasi"
        });
    }

    [Fact]
    public async Task GetClientByIdAsync_GivenNonExistingClientId_ShouldReturnNotFound()
    {
        HttpClient.AuthenticateAs(AppRoles.Broker);
        var clientId = new Guid("f6b9e0a7-8d55-4c6f-8b8e-56a2d7e8f009");
        var api = $"/api/brokers/clients/{clientId}";

        var clientResponse = await HttpClient.GetAsync(api);

        clientResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var json = await clientResponse.Content.ReadAsStringAsync();
        var problem = JsonConvert.DeserializeObject<ProblemDetails>(json);

        problem.Should().NotBeNull();
        problem?.Title.Should().Be("Not Found");
        problem?.Status.Should().Be(404);
        problem?.Detail.Should().Be($"Client '{clientId}' not found.");
        problem?.Instance.Should().Be(api);
    }

    [Fact]
    public async Task CreateClient_GivenValidRequest_ShouldAddClientToDatabase()
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

        var location = clientResponse.Headers.Location.ToString();

        var getClientResponse = await HttpClient.GetAsync(location);
        var json = await getClientResponse.Content.ReadAsStringAsync();

        getClientResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var getBody = JsonConvert.DeserializeObject<ClientDto>(json);

        getBody.Should().NotBeNull();
        getBody.Should().BeEquivalentTo(new
        {
            Id = clientId,
            Type = "Individual",
            Name = "John Does",
            IdentificationNumber = "3020202123129",
            Email = "john.does@mail.com",
            Phone = "0798786537",
            Address = "John Does Residence Nr.7"
        });
    }
}