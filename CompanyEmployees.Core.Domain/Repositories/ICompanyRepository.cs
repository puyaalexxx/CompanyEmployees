using CompanyEmployees.Core.Domain.Entities;
using Shared.RequestFeatures;

namespace CompanyEmployees.Core.Domain.Repositories;

public interface ICompanyRepository
{
    Task<PageList<Company>> GetAllCompaniesAsync(CompanyParameters companyParameters, bool trackChanges, CancellationToken ct = default);

    Task<Company?> GetCompanyAsync(Guid companyId, bool trackChanges, CancellationToken ct = default);

    Task<IEnumerable<Company>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges, CancellationToken ct = default);

    void CreateCompany(Company company);

    void DeleteCompany(Company company);


    // These methods are added to use Api Responses in the service layer examples
    IEnumerable<Company> GetAllCompanies(bool trackChanges);

    Company GetCompany(Guid companyId, bool trackChanges);
}