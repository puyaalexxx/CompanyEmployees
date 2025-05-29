using AutoMapper;
using CompanyEmployees.Core.Domain.Entities;
using CompanyEmployees.Core.Domain.Exceptions;
using CompanyEmployees.Core.Domain.Repositories;
using CompanyEmployees.Core.Services.Abstractions;
using LoggingService;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;

namespace CompanyEmployees.Core.Services;

internal sealed class EmployeeService : IEmployeeService
{
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;
    private readonly IRepositoryManager _repository;

    public EmployeeService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<(IEnumerable<EmployeeDto> employees, MetaData metaData)> GetEmployeesAsync(Guid companyId, EmployeeParameters employeeParameters,
        bool trackChanges, CancellationToken ct = default)
    {
        await CheckIfCompanyExists(companyId, trackChanges, ct);

        var employeesWithMetadata = await _repository.Employee.GetEmployeesAsync(companyId, employeeParameters, trackChanges, ct);

        var employeeDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesWithMetadata);

        return (employees: employeeDto, metaData: employeesWithMetadata.MetaData);
    }


    public async Task<EmployeeDto> GetEmployeeAsync(Guid companyId, Guid employeeId, bool trackChanges, CancellationToken ct = default)
    {
        await CheckIfCompanyExists(companyId, trackChanges, ct);

        var employee = await GetEmployeeForCompanyAndCheckIfItExists(companyId, employeeId, trackChanges, ct);

        return _mapper.Map<EmployeeDto>(employee);
    }

    public async Task<EmployeeDto> CreateEmployeeForCompanyAsync(Guid companyId, EmployeeForCreationDto employeeForCreation,
        bool trackChanges, CancellationToken ct = default)
    {
        await CheckIfCompanyExists(companyId, trackChanges, ct);

        var employee = _mapper.Map<Employee>(employeeForCreation);

        _repository.Employee.CreatEmployeeForCompany(companyId, employee);

        await _repository.SaveAsync(ct);

        return _mapper.Map<EmployeeDto>(employee);
    }

    public async Task DeleteEmployeeForCompanyAsync(Guid companyId, Guid employeeId, bool trackChanges, CancellationToken ct = default)
    {
        await CheckIfCompanyExists(companyId, trackChanges, ct);

        var employee = await GetEmployeeForCompanyAndCheckIfItExists(companyId, employeeId, trackChanges, ct);

        _repository.Employee.DeleteEmployee(employee);

        await _repository.SaveAsync(ct);
    }

    /// <summary>
    /// Delete Company if no employees are inside the company
    /// </summary>
    /// <param name="companyId"></param>
    /// <param name="employeeId"></param>
    /// <param name="trackChanges"></param>
    /// <exception cref="CompanyNotFoundException"></exception>
    /// <exception cref="EmployeeNotFoundException"></exception>
    public async Task DeleteEmployeeForCompanyAndCompanyAsync(Guid companyId, Guid employeeId, bool trackChanges, CancellationToken ct = default)
    {
        var company = await CheckIfCompanyExists(companyId, trackChanges, ct);

        var employeeForCompany = await GetEmployeeForCompanyAndCheckIfItExists(companyId, employeeId, trackChanges, ct);

        await _repository.Employee.DeleteEmployeeAndCompanyAsync(company, employeeForCompany, ct);
    }

    public async Task UpdateEmployeeForCompanyAsync(Guid companyId, Guid employeeId, EmployeeForManipulationDto employeeForUpdate,
        bool compTrackChanges, bool empTrackChanges, CancellationToken ct = default)
    {
        await CheckIfCompanyExists(companyId, compTrackChanges, ct);

        var employee = await GetEmployeeForCompanyAndCheckIfItExists(companyId, employeeId, empTrackChanges, ct);

        _mapper.Map(employeeForUpdate, employee);

        await _repository.SaveAsync();
    }

    /// <summary>
    /// Helper method to check if the company exists
    /// </summary>
    /// <param name="companyId"></param>
    /// <param name="trackChanges"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="CompanyNotFoundException"></exception>
    private async Task<Company> CheckIfCompanyExists(Guid companyId, bool trackChanges,
            CancellationToken ct = default)
    {
        var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges, ct);

        if (company is null) throw new CompanyNotFoundException(companyId);

        return company;
    }

    /// <summary>
    /// Helper method to get an employee for a specific company and check if it exists.
    /// </summary>
    /// <param name="companyId"></param>
    /// <param name="id"></param>
    /// <param name="trackChanges"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="EmployeeNotFoundException"></exception>
    private async Task<Employee> GetEmployeeForCompanyAndCheckIfItExists(Guid companyId,
        Guid id, bool trackChanges, CancellationToken ct = default)
    {
        var employee = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges, ct);

        if (employee is null) throw new EmployeeNotFoundException(id);

        return employee;
    }
}