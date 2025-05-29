using Shared.DataTransferObjects;

namespace CompanyEmployees.Core.Services.Abstractions;

public interface IEmployeeService
{
    Task<IEnumerable<EmployeeDto>> GetEmployeesAsync(Guid companyId, bool trackChanges, CancellationToken ct = default);

    Task<EmployeeDto> GetEmployeeAsync(Guid companyId, Guid employeeId, bool trackChanges, CancellationToken ct = default);

    Task<EmployeeDto> CreateEmployeeForCompanyAsync(Guid companyId, EmployeeForCreationDto employeeForCreation,
        bool trackChanges, CancellationToken ct = default);

    Task DeleteEmployeeForCompanyAsync(Guid companyId, Guid employeeId, bool trackChanges, CancellationToken ct = default);

    Task UpdateEmployeeForCompanyAsync(Guid companyId, Guid employeeId, EmployeeForManipulationDto employeeForUpdate, bool compTrackChanges,
        bool empTrackChanges, CancellationToken ct = default);
}