using AutoMapper;
using FluentAssertions;
using InsuranceApp.Application.Common.Errors;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Common.Repository;
using InsuranceApp.Application.Geography.Counties;
using InsuranceApp.Application.Geography.DTOs;
using InsuranceApp.Domain.Entities;
using Moq;

namespace InsuranceApp.UnitTests.Services;

public class CountyServiceTests
{
    private readonly Mock<IRepository<Country, Guid>> _countryRepository = new();
    private readonly Mock<ICountyRepository> _countyRepository = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly ICountyService _countyService;

    public CountyServiceTests()
    {
        _countyService = new CountyService(_countyRepository.Object, _countryRepository.Object, _mapper.Object);
    }

    [Fact]
    public async Task ListAllCountryCountiesAsync_GivenExistingCountryId_ShouldReturnPagedListOfCounties()
    {
        var countryId = Guid.NewGuid();
        var pageRequest = new PageRequest { PageNumber = 1, PageSize = 10 };

        var country = new Country
        {
            Id = countryId,
            Name = "Romania",
            Iso2 = "RO",
            Iso3 = "ROU"
        };

        var pagedCounties = new PagedResult<CountyDto>
        {
            Items = new List<CountyDto>
            {
                new() { Id = Guid.NewGuid(), Name = "Iasi" },
                new() { Id = Guid.NewGuid(), Name = "Bucharest" },
                new() { Id = Guid.NewGuid(), Name = "Cluj" }
            },
            TotalCount = 3,
            PageNumber = 1,
            PageSize = 10
        };

        _countryRepository.Setup(r => r.GetAsync(countryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(country);

        _countyRepository.Setup(r => r.GetAllCountryCountiesAsync(pageRequest, countryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedCounties);

        var result = await _countyService.ListAllCountryCountiesAsync(pageRequest, countryId, CancellationToken.None);

        result.IsSuccess.Should().BeTrue("A valid CountryId should return a paged list of counties");
        result.Errors.Should().BeEmpty("There should be no errors when passing an existing country id");
        result.Value.Should().NotBeNull("A PagedResult should be returned");
        result.Value.Should().BeEquivalentTo(pagedCounties);

        _countryRepository.Verify(r => r.GetAsync(countryId, It.IsAny<CancellationToken>()), Times.Once);
        _countyRepository.Verify(r => r.GetAllCountryCountiesAsync(pageRequest, countryId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ListAllCountryCountiesAsync_GivenNonExistingCountryId_ShouldReturnNotFound()
    {
        var countryId = Guid.NewGuid();
        var pageRequest = new PageRequest { PageNumber = 1, PageSize = 10 };

        Country? country = null;

        _countryRepository.Setup(r => r.GetAsync(countryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(country);

        var result = await _countyService.ListAllCountryCountiesAsync(pageRequest, countryId, CancellationToken.None);

        result.IsFailed.Should().BeTrue("An invalid CountryId should return a fail result");
        result.Errors.Should().ContainSingle(e => e is NotFoundError
                                                  && e.Message == $"Country '{countryId}' not found.");

        _countryRepository.Verify(r => r.GetAsync(countryId, It.IsAny<CancellationToken>()), Times.Once);
        _countyRepository.Verify(r => r.GetAllCountryCountiesAsync(pageRequest, countryId, It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetCountyByIdAsync_GivenExistingCountyId_ShouldReturnCounty()
    {
        var countyId = Guid.NewGuid();

        var county = GetValidCounty();
        county.Id = countyId;

        var countyDto = new CountyDto { CountryId = county.CountryId, Id = county.Id, Name = county.Name };

        _countyRepository.Setup(r => r.GetAsync(countyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(county);

        _mapper.Setup(m => m.Map<CountyDto>(county)).Returns(countyDto);

        var result = await _countyService.GetCountyByIdAsync(countyId, CancellationToken.None);

        result.IsSuccess.Should().BeTrue("A valid CountyId should return a successful result");
        result.Errors.Should().BeEmpty("There should be no errors when passing an existing county id");
        result.Value.Should().NotBeNull("A CountyDto should be returned");
        result.Value.Should().BeEquivalentTo(countyDto);

        _mapper.Verify(m => m.Map<CountyDto>(county), Times.Once);
        _countyRepository.Verify(r => r.GetAsync(countyId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetCountyByIdAsync_GivenNonExistingCountyId_ShouldReturnNotFound()
    {
        var countyId = Guid.NewGuid();
        County? county = null;

        _countyRepository.Setup(r => r.GetAsync(countyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(county);

        var result = await _countyService.GetCountyByIdAsync(countyId, CancellationToken.None);

        result.IsFailed.Should().BeTrue("An invalid CountyId should return a fail result");
        result.Errors.Should().ContainSingle(e => e is NotFoundError
                                                  && e.Message == $"County '{countyId}' not found.");

        _mapper.Verify(m => m.Map<CountyDto>(county), Times.Never);
        _countyRepository.Verify(r => r.GetAsync(countyId, It.IsAny<CancellationToken>()), Times.Once);
    }

    private County GetValidCounty() => new County()
    {
        Id = Guid.NewGuid(),
        Name = "Iasi",
        CountryId = Guid.NewGuid()
    };
}
