using CompanyEmployees.Core.Domain.Entities;
using CompanyEmployees.Core.Domain.Repositories;
using CompanyEmployees.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Shared.RequestFeatures;

namespace CompanyEmployees.Infrastructure.Persistence.Repositories;

public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
{
    public EmployeeRepository(RepositoryContext repositoryContext) : base(repositoryContext)
    {
    }

    public async Task<Employee?> GetEmployeeAsync(Guid companyId, Guid employeeId,
        bool trackChanges, CancellationToken ct = default)
    {
        return await FindByCondition(e => e.CompanyId.Equals(companyId) && e.Id.Equals(employeeId), trackChanges)
            .SingleOrDefaultAsync(ct);
    }

    public async Task<PageList<Employee>> GetEmployeesAsync(Guid companyId, EmployeeParameters employeeParameters,
        bool trackChanges, CancellationToken ct = default)
    {
        var employeesQuery = FindByCondition(e => e.CompanyId.Equals(companyId), trackChanges)
            .FilterEmployees(employeeParameters.MinAge, employeeParameters.MaxAge)
            .Search(employeeParameters.SearchTerm ?? String.Empty)
            .OrderBy(e => e.Name);

        var count = await employeesQuery.CountAsync(ct);

        var employees = await employeesQuery
            .Skip((employeeParameters.PageNumber - 1) * employeeParameters.PageSize)
            .Take(employeeParameters.PageSize)
            .ToListAsync(ct);

        return PageList<Employee>
            .ToPageList(employees, count, employeeParameters.PageNumber, employeeParameters.PageSize);
    }

    public void CreatEmployeeForCompany(Guid companyId, Employee employee)
    {
        employee.CompanyId = companyId;

        Create(employee);
    }

    public void DeleteEmployee(Employee employee) => Delete(employee);

    /// <summary>
    /// Delete Company if no employees are inside the company
    /// </summary>
    /// <param name="company"></param>
    /// <param name="employee"></param>
    public async Task DeleteEmployeeAndCompanyAsync(Company company, Employee employee, CancellationToken ct = default)
    {
        using var transaction = await RepositoryContext.Database.BeginTransactionAsync(ct);

        Delete(employee);

        await RepositoryContext.SaveChangesAsync(ct);

        if (!(await FindByCondition(e => e.CompanyId == company.Id, false).AnyAsync(ct)))
        {
            //throw new InvalidOperationException("Cannot delete company with employees still present.");

            RepositoryContext.Companies!.Remove(company);

            await RepositoryContext.SaveChangesAsync(ct);
        }

        await transaction.CommitAsync(ct);
    }
}