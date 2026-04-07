using AutoMapper;
using InsuranceApp.Application.Brokers.DTOs;
using InsuranceApp.Application.Buildings.DTOs;
using InsuranceApp.Application.Clients.DTOs;
using InsuranceApp.Application.Common.Audit;
using InsuranceApp.Application.Geography.DTOs;
using InsuranceApp.Application.Metadata.Currencies.DTOs;
using InsuranceApp.Application.Metadata.FeeConfigurations.DTOs;
using InsuranceApp.Application.Metadata.RiskFactorConfigurations.DTOs;
using InsuranceApp.Application.Policies.DTOs;
using InsuranceApp.Domain.Entities;

namespace InsuranceApp.Application.Common.Mapping;
public class InsuranceMapper : Profile
{
    public InsuranceMapper()
    {
        CreateMap<Country, CountryDto>().ReverseMap();
        CreateMap<County, CountyDto>().ReverseMap();
        CreateMap<City, CityDto>().ReverseMap();
        CreateMap<Client, ClientDto>().ReverseMap();
        CreateMap<Client, CreateClientRequest>().ReverseMap();
        CreateMap<Client, UpdateClientRequest>().ReverseMap();
        CreateMap<RiskIndicator, RiskIndicatorDto>().ReverseMap();
        CreateMap<Building, BuildingDto>()
            .ForMember(b => b.City, opt =>
                opt.MapFrom(s => s.City.Name))
            .ForMember(b => b.County, opt =>
                opt.MapFrom(s => s.City.County.Name))
            .ForMember(b => b.Country, opt =>
                opt.MapFrom(s => s.City.County.Country.Name));
        CreateMap<CreateBuildingRequest, Building>();
        CreateMap<PolicyChangedAuditEvent, PolicyAuditLog>();
        CreateMap<AuditEvent, AuditLog>();
        CreateMap<Broker, BrokerDto>().ReverseMap();
        CreateMap<CreateBrokerRequest, Broker>();
        CreateMap<UpdateBrokerRequest, Broker>();
        CreateMap<Currency, CurrencyDto>().ReverseMap();
        CreateMap<CreateCurrencyRequest, Currency>();
        CreateMap<UpdateCurrencyRequest, Currency>();
        CreateMap<RiskFactorConfiguration, RiskFactorConfigurationDto>().ReverseMap();
        CreateMap<CreateRiskFactorConfigurationRequest, RiskFactorConfiguration>();
        CreateMap<UpdateRiskFactorConfigurationRequest, RiskFactorConfiguration>();
        CreateMap<FeeConfiguration, FeeConfigurationDto>().ReverseMap();
        CreateMap<CreateFeeConfigurationRequest, FeeConfiguration>();
        CreateMap<UpdateFeeConfigurationRequest, FeeConfiguration>();
        CreateMap<Policy, PolicyDto>().ReverseMap();
        CreateMap<CreatePolicyRequest, Policy>();
        CreateMap<Building, PolicyBuildingDto>()
            .ForMember(b => b.CityId, opt =>
                opt.MapFrom(s => s.CityId))
            .ForMember(b => b.CountyId, opt =>
                opt.MapFrom(s => s.City.CountyId))
            .ForMember(b => b.CountryId, opt =>
                opt.MapFrom(s => s.City.County.CountryId));
        CreateMap<Policy, DetailedPolicyDto>();
    }
}
