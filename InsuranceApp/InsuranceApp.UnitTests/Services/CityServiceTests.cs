using AutoMapper;
using FluentAssertions;
using InsuranceApp.Application.Common.Errors;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Common.Repository;
using InsuranceApp.Application.Geography.Cities;
using InsuranceApp.Application.Geography.DTOs;
using InsuranceApp.Domain.Entities;
using Moq;

namespace InsuranceApp.UnitTests.Services;

public class CityServiceTests
{
    private readonly Mock<ICityRepository> _cityRepository = new();
    private readonly Mock<ICountyRepository> _countyRepository = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly ICityService _cityService;

    public CityServiceTests()
    {
        _cityService = new CityService(_cityRepository.Object, _countyRepository.Object, _mapper.Object);
    }

    [Fact]
    public async Task ListAllCountyCitiesAsync_GivenExistingCountyId_ShouldReturnPagedListOfCities()
    {
        var countyId = Guid.NewGuid();
        var pageRequest = new PageRequest { PageNumber = 1, PageSize = 10 };
        var county = new County
        {
            Id = countyId,
            Name = "Iasi",
        };
        var pagedCities = new PagedResult<CityDto>
        {
            Items = new List<CityDto>
            {
                new() { Id = Guid.NewGuid(), Name = "Iasi" },
                new() { Id = Guid.NewGuid(), Name = "Pascani" },
                new() { Id = Guid.NewGuid(), Name = "Targu Frumos" }
            },
            TotalCount = 3,
            PageNumber = 1,
            PageSize = 10
        };
        _countyRepository.Setup(r => r.GetAsync(countyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(county);
        _cityRepository.Setup(r => r.GetAllCountyCitiesAsync(pageRequest, countyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedCities);

        var result = await _cityService.ListAllCountyCitiesAsync(pageRequest, countyId, CancellationToken.None);

        result.IsSuccess.Should().BeTrue("A valid CountyId should return a paged list of cities");
        result.Errors.Should().BeEmpty("There should be no errors when passing an existing county id");
        result.Value.Should().NotBeNull("A PagedResult should be returned");
        result.Value.Should().BeEquivalentTo(pagedCities);

        _countyRepository.Verify(r => r.GetAsync(countyId, It.IsAny<CancellationToken>()), Times.Once);
        _cityRepository.Verify(r => r.GetAllCountyCitiesAsync(pageRequest, countyId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ListAllCountyCitiesAsync_GivenNonExistingCountyId_ShouldReturnNotFound()
    {
        var countyId = Guid.NewGuid();
        var pageRequest = new PageRequest { PageNumber = 1, PageSize = 10 };
        County? county = null;
        _countyRepository.Setup(r => r.GetAsync(countyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(county);

        var result = await _cityService.ListAllCountyCitiesAsync(pageRequest, countyId, CancellationToken.None);

        result.IsFailed.Should().BeTrue("An invalid CountyId should return a fail result");
        result.Errors.Should().ContainSingle(e => e is NotFoundError
                                                  && e.Message == $"County '{countyId}' not found.");

        _countyRepository.Verify(r => r.GetAsync(countyId, It.IsAny<CancellationToken>()), Times.Once);
        _cityRepository.Verify(r => r.GetAllCountyCitiesAsync(pageRequest, countyId, It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetCityByIdAsync_GivenExistingCityId_ShouldReturnCity()
    {
        var cityId = Guid.NewGuid();
        var city = new City
        {
            Id = cityId,
            Name = "Iasi",
            CountyId = Guid.NewGuid()
        };
        var cityDto = new CityDto { CountyId = city.CountyId, Id = city.Id, Name = city.Name };
        _cityRepository.Setup(r => r.GetAsync(cityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(city);
        _mapper.Setup(m => m.Map<CityDto>(city)).Returns(cityDto);

        var result = await _cityService.GetCityByIdAsync(cityId, CancellationToken.None);

        result.IsSuccess.Should().BeTrue("A valid CityId should return a successful result");
        result.Errors.Should().BeEmpty("There should be no errors when passing an existing city id");
        result.Value.Should().NotBeNull("A CityDto should be returned");
        result.Value.Should().BeEquivalentTo(cityDto);

        _mapper.Verify(m => m.Map<CityDto>(city), Times.Once);
        _cityRepository.Verify(r => r.GetAsync(cityId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetCityByIdAsync_GivenNonExistingCityId_ShouldReturnNotFound()
    {
        var cityId = Guid.NewGuid();
        City? city = null;
        _cityRepository.Setup(r => r.GetAsync(cityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(city);

        var result = await _cityService.GetCityByIdAsync(cityId, CancellationToken.None);

        result.IsFailed.Should().BeTrue("An invalid CityId should return a fail result");
        result.Errors.Should().ContainSingle(e => e is NotFoundError
                                                  && e.Message == $"City '{cityId}' not found.");

        _mapper.Verify(m => m.Map<CityDto>(city), Times.Never);
        _cityRepository.Verify(r => r.GetAsync(cityId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
