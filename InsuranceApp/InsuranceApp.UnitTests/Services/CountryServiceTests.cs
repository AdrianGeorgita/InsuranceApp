using AutoMapper;
using FluentAssertions;
using InsuranceApp.Application.Common.Errors;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Common.Repository;
using InsuranceApp.Application.Geography.Countries;
using InsuranceApp.Application.Geography.DTOs;
using InsuranceApp.Domain.Entities;
using Moq;

namespace InsuranceApp.UnitTests.Services;

public class CountryServiceTests
{
    private readonly Mock<IRepository<Country, Guid>> _repository = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly ICountryService _countryService;

    public CountryServiceTests()
    {
        _countryService = new CountryService(_repository.Object, _mapper.Object);
    }

    [Fact]
    public async Task ListAllCountriesAsync_ShouldReturnPagedListOfCountries()
    {
        var pageRequest = new PageRequest { PageNumber = 1, PageSize = 10 };

        var pagedCountries = new PagedResult<CountryDto>
        {
            Items = new List<CountryDto>
            {
                new() { Id = Guid.NewGuid(), Name = "Romania", Iso2 = "RO", Iso3 = "ROU" },
                new() { Id = Guid.NewGuid(), Name = "Germany", Iso2 = "DE", Iso3 = "DEU" },
                new() { Id = Guid.NewGuid(), Name = "Albania", Iso2 = "AL", Iso3 = "ALB" }
            },
            TotalCount = 3,
            PageNumber = 1,
            PageSize = 10
        };

        _repository.Setup(r => r.GetAllAsync<CountryDto>(pageRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedCountries);

        var result = await _countryService.ListAllCountriesAsync(pageRequest, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(pagedCountries);

        _repository.Verify(r => r.GetAllAsync<CountryDto>(pageRequest, It.IsAny<CancellationToken>()), Times.Once);
    }
    [Fact]
    public async Task GetCountryByIdAsync_GivenExistingCountryId_ShouldReturnCountry()
    {
        var countryId = Guid.NewGuid();

        var country = new Country() { Id = countryId, Name = "Romania", Iso2 = "RO", Iso3 = "ROU" };
        var countryDto = new CountryDto { Id = country.Id, Name = country.Name, Iso2 = country.Iso2, Iso3 = country.Iso3};

        _repository.Setup(r => r.GetAsync(countryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(country);

        _mapper.Setup(m => m.Map<CountryDto>(country)).Returns(countryDto);

        var result = await _countryService.GetCountryByIdAsync(countryId, CancellationToken.None);

        result.IsSuccess.Should().BeTrue("A valid CountryId should return a successful result");
        result.Errors.Should().BeEmpty("There should be no errors when passing an existing country id");
        result.Value.Should().NotBeNull("A CountryDto should be returned");
        result.Value.Should().BeEquivalentTo(countryDto);

        _mapper.Verify(m => m.Map<CountryDto>(country), Times.Once);
        _repository.Verify(r => r.GetAsync(countryId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetCountryByIdAsync_GivenNonExistingCountryId_ShouldReturnNotFound()
    {
        var countryId = Guid.NewGuid();
        Country? country = null;

        _repository.Setup(r => r.GetAsync(countryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(country);

        var result = await _countryService.GetCountryByIdAsync(countryId, CancellationToken.None);

        result.IsFailed.Should().BeTrue("An invalid CountryId should return a fail result");
        result.Errors.Should().ContainSingle(e => e is NotFoundError
                                                  && e.Message == $"Country '{countryId}' not found.");

        _mapper.Verify(m => m.Map<CountryDto>(country), Times.Never);
        _repository.Verify(r => r.GetAsync(countryId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
