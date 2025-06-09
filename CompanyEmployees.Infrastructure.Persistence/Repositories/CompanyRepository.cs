using CompanyEmployees.Core.Domain.Entities;
using CompanyEmployees.Core.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Shared.RequestFeatures;

namespace CompanyEmployees.Infrastructure.Persistence.Repositories;

public class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
{
    public CompanyRepository(RepositoryContext repositoryContext) : base(repositoryContext)
    {
    }

    public async Task<PageList<Company>> GetAllCompaniesAsync(CompanyParameters companyParameters, bool trackChanges, CancellationToken ct = default)
    {
        var companies = await FindAll(trackChanges)
           .OrderBy(c => c.Name)
           .Skip((companyParameters.PageNumber - 1) * companyParameters.PageSize)
           .Take(companyParameters.PageSize)
           .ToListAsync(ct);

        var count = await FindAll(trackChanges).CountAsync(ct);

        return PageList<Company>
            .ToPageList(companies, count, companyParameters.PageNumber, companyParameters.PageSize);
    }

    public async Task<Company?> GetCompanyAsync(Guid companyId, bool trackChanges, CancellationToken ct = default) =>
        await FindByCondition(c => c.Id.Equals(companyId), trackChanges).SingleOrDefaultAsync(ct);

    public async Task<IEnumerable<Company>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges, CancellationToken ct = default) =>
        await FindByCondition(x => ids.Contains(x.Id), trackChanges).ToListAsync(ct);

    public void CreateCompany(Company company) => Create(company);

    public void DeleteCompany(Company company) => Delete(company);

    // These methods are added to use Api Responses in the service layer examples
    public IEnumerable<Company> GetAllCompanies(bool trackChanges) =>
        FindAll(trackChanges).OrderBy(c => c.Name).ToList();

    public Company GetCompany(Guid companyId, bool trackChanges) =>
        FindByCondition(c => c.Id.Equals(companyId), trackChanges).SingleOrDefault()!;
}