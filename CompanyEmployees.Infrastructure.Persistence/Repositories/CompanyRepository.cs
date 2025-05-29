using CompanyEmployees.Core.Domain.Entities;
using CompanyEmployees.Core.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CompanyEmployees.Infrastructure.Persistence.Repositories;

public class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
{
    public CompanyRepository(RepositoryContext repositoryContext) : base(repositoryContext)
    {
    }

    public async Task<IEnumerable<Company>> GetAllCompaniesAsync(bool trackChanges, CancellationToken ct = default) =>
        await FindAll(trackChanges).OrderBy(c => c.Name).ToListAsync(ct);

    public async Task<Company> GetCompanyAsync(Guid companyId, bool trackChanges, CancellationToken ct = default) =>
        await FindByCondition(c => c.Id.Equals(companyId), trackChanges).SingleOrDefaultAsync(ct);

    public async Task<IEnumerable<Company>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges, CancellationToken ct = default) =>
        await FindByCondition(x => ids.Contains(x.Id), trackChanges).ToListAsync(ct);

    public void CreateCompany(Company company) => Create(company);

    public void DeleteCompany(Company company) => Delete(company);
}