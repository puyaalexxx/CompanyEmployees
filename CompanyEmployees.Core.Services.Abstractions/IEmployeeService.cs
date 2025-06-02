using CompanyEmployees.Core.Domain.LinkModels;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;

namespace CompanyEmployees.Core.Services.Abstractions;

public interface IEmployeeService
{
    Task<(LinkResponse linkResponse, MetaData metaData)> GetEmployeesAsync(Guid companyId, LinkParameters linkParameters,
        bool trackChanges, CancellationToken ct = default);

    Task<EmployeeDto> GetEmployeeAsync(Guid companyId, Guid employeeId, bool trackChanges, CancellationToken ct = default);

    Task<EmployeeDto> CreateEmployeeForCompanyAsync(Guid companyId, EmployeeForCreationDto employeeForCreation,
        bool trackChanges, CancellationToken ct = default);

    Task DeleteEmployeeForCompanyAsync(Guid companyId, Guid employeeId, bool trackChanges, CancellationToken ct = default);

    Task UpdateEmployeeForCompanyAsync(Guid companyId, Guid employeeId, EmployeeForManipulationDto employeeForUpdate, bool compTrackChanges,
        bool empTrackChanges, CancellationToken ct = default);
}