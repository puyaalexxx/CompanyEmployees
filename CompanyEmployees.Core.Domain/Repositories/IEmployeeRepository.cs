using CompanyEmployees.Core.Domain.Entities;

namespace CompanyEmployees.Core.Domain.Repositories;

public interface IEmployeeRepository
{
    IEnumerable<Employee> GetEmployees(Guid companyId, bool trackChanges);

    Employee GetEmployee(Guid companyId, Guid employeeId, bool trackChanges);

    void CreatEmployeeForCompany(Guid companyId, Employee employee);
}