using AutoMapper;
using CompanyEmployees.Core.Domain.Entities;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Mapppers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        /*CreateMap<Company, CompanyDto>()
            .ForCtorParam("FullAddress",
                opts => opts.MapFrom(x => $"{x.Address} {x.Country}"));*/
        
        //for content negotiation
        CreateMap<Company, CompanyDto>()
            .ForMember(c => c.FullAddress,
                opts => opts.MapFrom(x => $"{x.Address} {x.Country}"));

        CreateMap<Employee, EmployeeDto>();
    }
}