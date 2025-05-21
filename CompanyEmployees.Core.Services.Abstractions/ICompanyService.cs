using System.Collections;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Core.Services.Abstractions;

public interface ICompanyService
{
    IEnumerable<CompanyDto> GetAllCompanies(bool trackChanges);
    
    CompanyDto GetCompany(Guid companyId, bool trackChanges);
    
    IEnumerable<CompanyDto> GetByIds(IEnumerable<Guid> ids, bool trackChanges);
    
    CompanyDto CreateCompany(CompanyForCreationDto company);

    (IEnumerable<CompanyDto> companies, string ids) CreateCompanyCollection(IEnumerable<CompanyForCreationDto> companyCollection);
}