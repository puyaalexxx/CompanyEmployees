using System.Collections;
using CompanyEmployees.Core.Domain.Entities;

namespace CompanyEmployees.Core.Domain.Repositories;

public interface ICompanyRepository
{
    IEnumerable<Company> GetAllCompanies(bool trackChanges);
    
    Company GetCompany(Guid companyId, bool trackChanges);
    
    IEnumerable<Company> GetByIds(IEnumerable<Guid> ids, bool trackChanges);

    void CreateCompany(Company company);
}