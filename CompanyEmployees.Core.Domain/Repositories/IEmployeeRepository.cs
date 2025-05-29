using CompanyEmployees.Core.Domain.Entities;

namespace CompanyEmployees.Core.Domain.Repositories;

public interface IEmployeeRepository
{
    Task<IEnumerable<Employee>> GetEmployeesAsync(Guid companyId, bool trackChanges, CancellationToken ct = default);

    Task<Employee> GetEmployeeAsync(Guid companyId, Guid employeeId, bool trackChanges, CancellationToken ct = default);

    void CreatEmployeeForCompany(Guid companyId, Employee employee);

    void DeleteEmployee(Employee employee);

    /// <summary>
    /// Delete Company if no employees are inside the company
    /// </summary>
    /// <param name="company"></param>
    /// <param name="employee"></param>
    Task DeleteEmployeeAndCompanyAsync(Company company, Employee employee, CancellationToken ct = default);
}