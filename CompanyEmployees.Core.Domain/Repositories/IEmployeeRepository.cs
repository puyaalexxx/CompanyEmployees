using CompanyEmployees.Core.Domain.Entities;

namespace CompanyEmployees.Core.Domain.Repositories;

public interface IEmployeeRepository
{
    IEnumerable<Employee> GetEmployees(Guid companyId, bool trackChanges);

    Employee GetEmployee(Guid companyId, Guid employeeId, bool trackChanges);

    void CreatEmployeeForCompany(Guid companyId, Employee employee);

    void DeleteEmployee(Employee employee);

    /// <summary>
    /// Delete Company if no employees are inside the company
    /// </summary>
    /// <param name="company"></param>
    /// <param name="employee"></param>
    void DeleteEmployeeAndCompany(Company company, Employee employee);
}