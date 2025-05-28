using CompanyEmployees.Core.Domain.Entities;
using CompanyEmployees.Core.Domain.Repositories;

namespace CompanyEmployees.Infrastructure.Persistence.Repositories;

public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
{
    public EmployeeRepository(RepositoryContext repositoryContext) : base(repositoryContext)
    {
    }

    public IEnumerable<Employee> GetEmployees(Guid companyId, bool trackChanges)
    {
        return FindByCondition(e => e.CompanyId.Equals(companyId), trackChanges)
            .OrderBy(e => e.Name).ToList();
    }

    public Employee GetEmployee(Guid companyId, Guid employeeId, bool trackChanges)
    {
        return FindByCondition(e => e.CompanyId.Equals(companyId) && e.Id.Equals(employeeId), trackChanges)
            .SingleOrDefault()!;
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
    public void DeleteEmployeeAndCompany(Company company, Employee employee)
    {
        using var transaction = RepositoryContext.Database.BeginTransaction();

        Delete(employee);

        RepositoryContext.SaveChanges();

        if (!FindByCondition(e => e.CompanyId == company.Id, false).Any())
        {
            throw new InvalidOperationException("Cannot delete company with employees still present.");

            RepositoryContext.Companies!.Remove(company);

            RepositoryContext.SaveChanges();
        }

        transaction.Commit();
    }
}