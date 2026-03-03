using FluentAssertions;
using InsuranceApp.Application.Buildings.DTOs;
using InsuranceApp.Application.Common.Pagination;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;

namespace InsuranceApp.IntegrationTests.Brokers.Clients;

public class ClientBuildingTests : IntegrationTestBase
{
    [Fact]
    public async Task ListAllClientBuildingsAsync_GivenExistingClientId_ShouldReturnPagedListOfBuildings()
    {
        var pageRequest = new
        {
            PageSize = 2,
            PageNumber = 1,
        };

        var clientId = new Guid("a1d4f1a7-1f2e-4b5b-9a2d-01f1e8a3c001");

        var api = $"/api/brokers/clients/{clientId}/buildings?pageSize={pageRequest.PageSize}&pageNumber={pageRequest.PageNumber}";

        var getBuildingResponse = await HttpClient.GetAsync(api);

        getBuildingResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await getBuildingResponse.Content.ReadAsStringAsync();
        var body = JsonConvert.DeserializeObject<PagedResult<BuildingDto>>(json);

        body.Should().NotBeNull();
        body.PageSize.Should().Be(pageRequest.PageSize);
        body.PageNumber.Should().Be(pageRequest.PageNumber);
        body.TotalCount.Should().Be(2);
        body.Items.Should().HaveCount(2);
        body.Items.Should().BeEquivalentTo(new List<BuildingDto>
        {
            new()
            {
                Id = new Guid("a1000004-0000-4000-8000-000000000004"),
                OwnerId = new Guid("a1d4f1a7-1f2e-4b5b-9a2d-01f1e8a3c001"),
                Address = "Str. Avram Iancu 22",
                ConstructionYear = 2005,
                BuildingType = "Office",
                NumberOfFloors = 5,
                SurfaceArea = 780,
                InsuredValue = 3500000,
                City = "Cluj-Napoca",
                County = "Cluj",
                Country = "Romania",
                RiskIndicators = new List<RiskIndicatorDto>()
                {
                    new()
                    {
                        Id = 2,
                        Name = "Earthquake risk zone"
                    }
                }
            },
            new()
            {
                Id = new Guid("a1000034-0000-4000-8000-000000000034"),
                OwnerId = new Guid("a1d4f1a7-1f2e-4b5b-9a2d-01f1e8a3c001"),
                Address = "Str. Constructorilor 2",
                ConstructionYear = 1997,
                BuildingType = "Residential",
                NumberOfFloors = 7,
                SurfaceArea = 760,
                InsuredValue = 3000000,
                City = "Bucharest",
                County = "Bucharest",
                Country = "Romania",
                RiskIndicators = new List<RiskIndicatorDto>() { }
            }
        });
    }


    [Fact]
    public async Task ListAllClientBuildingsAsync_GivenOutOfRangePage_ShouldReturnPagedEmptyListOfBuildings()
    {
        var pageRequest = new
        {
            PageSize = 10,
            PageNumber = 12512,
        };

        var clientId = new Guid("a1d4f1a7-1f2e-4b5b-9a2d-01f1e8a3c001");

        var api = $"/api/brokers/clients/{clientId}/buildings?pageSize={pageRequest.PageSize}&pageNumber={pageRequest.PageNumber}";

        var getBuildingsResponse = await HttpClient.GetAsync(api);

        getBuildingsResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await getBuildingsResponse.Content.ReadAsStringAsync();
        var body = JsonConvert.DeserializeObject<PagedResult<BuildingDto>>(json);

        body.Should().NotBeNull();
        body.PageSize.Should().Be(pageRequest.PageSize);
        body.PageNumber.Should().Be(pageRequest.PageNumber);
        body.TotalCount.Should().Be(2);
        body.Items.Should().HaveCount(0);
        body.Items.Should().BeEquivalentTo(new List<BuildingDto> { });
    }

    [Fact]
    public async Task ListAllClientBuildingsAsync_GivenInvalidPageRequest_ShouldReturnBadRequest()
    {
        var pageRequest = new
        {
            PageSize = 160,
            PageNumber = 1,
        };

        var clientId = new Guid("a1d4f1a7-1f2e-4b5b-9a2d-01f1e8a3c001");

        var api = $"/api/brokers/clients/{clientId}/buildings?pageSize={pageRequest.PageSize}&pageNumber={pageRequest.PageNumber}";

        var getBuildingsResponse = await HttpClient.GetAsync(api);

        getBuildingsResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var json = await getBuildingsResponse.Content.ReadAsStringAsync();
        var problem = JsonConvert.DeserializeObject<ProblemDetails>(json);

        problem.Should().NotBeNull();
        problem?.Title.Should().Be("One or more validation errors occurred.");
        problem?.Status.Should().Be(400);
    }

    [Fact]
    public async Task CreateClient_ThenRegisterBuilding_ShouldAddClientAndBuildingToDatabase()
    {
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

        var location = createBuildingResponse.Headers.Location.ToString();

        var getBuildingResponse = await HttpClient.GetAsync(location);
        var json = await getBuildingResponse.Content.ReadAsStringAsync();

        getBuildingResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var getBody = JsonConvert.DeserializeObject<BuildingDto>(json);

        getBody.Should().NotBeNull();
        getBody.Should().BeEquivalentTo(new
        {
            Id = buildingId,
            Address = "Some random street Nr.7",
            City = "Iasi",
            County = "Iasi",
            Country = "Romania",
            ConstructionYear = 1970,
            BuildingType = "Industrial",
            NumberOfFloors = 3,
            SurfaceArea = 135.23M,
            InsuredValue = 125236235
        });
    }
}