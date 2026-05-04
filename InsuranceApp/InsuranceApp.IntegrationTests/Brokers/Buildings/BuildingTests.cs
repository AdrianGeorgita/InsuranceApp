using FluentAssertions;
using InsuranceApp.Application.Buildings.DTOs;
using InsuranceApp.Application.Common.Constants;
using InsuranceApp.IntegrationTests.Extensions;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;

namespace InsuranceApp.IntegrationTests.Brokers.Buildings;

public class BuildingTests : IntegrationTestBase
{
    [Fact]
    public async Task UpdateBuildingAsync_GivenValidRequest_ThenGetBuildingById_ShouldReturnUpdatedBuilding()
    {
        HttpClient.AuthenticateAs(AppRoles.Broker);
        var buildingId = new Guid("a1000004-0000-4000-8000-000000000004");
        var updateBuildingRequest = new
        {
            Address = "New address",
            InsuredValue = 5000000
        };

        var api = ApiV1($"/brokers/buildings/{buildingId}");

        var updateBuildingResponse = await HttpClient.PatchAsJsonAsync(api, updateBuildingRequest);
        var receivedBuildingId = await updateBuildingResponse.Content.ReadFromJsonAsync<Guid>();

        updateBuildingResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        receivedBuildingId.Should().NotBeEmpty();
        receivedBuildingId.Should().Be(buildingId);

        var getBuildingResponse = await HttpClient.GetAsync(api);
        var json = await getBuildingResponse.Content.ReadAsStringAsync();

        getBuildingResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var getBody = JsonConvert.DeserializeObject<BuildingDto>(json);

        getBody.Should().NotBeNull();
        getBody.Should().BeEquivalentTo(new
        {
            Id = buildingId,
            OwnerId = new Guid("a1d4f1a7-1f2e-4b5b-9a2d-01f1e8a3c001"),
            BuildingType = "Office",
            ConstructionYear = 2005,
            Address = "New address",
            NumberOfFloors = 5,
            SurfaceArea = 780,
            InsuredValue = 5000000,
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
        });
    }
}