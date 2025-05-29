using AutoMapper;
using CompanyEmployees.Core.Domain.Entities;
using CompanyEmployees.Core.Domain.Exceptions;
using CompanyEmployees.Core.Domain.Repositories;
using CompanyEmployees.Core.Services.Abstractions;
using LoggingService;
using Shared.DataTransferObjects;

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

    public async Task<IEnumerable<EmployeeDto>> GetEmployeesAsync(Guid companyId, bool trackChanges, CancellationToken ct = default)
    {
        var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges, ct);

        if (company is null) throw new CompanyNotFoundException(companyId);

        var employeesFromDb = await _repository.Employee.GetEmployeesAsync(companyId, trackChanges, ct);

        var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesFromDb);

        return employeesDto;
    }

    public async Task<EmployeeDto> GetEmployeeAsync(Guid companyId, Guid employeeId, bool trackChanges, CancellationToken ct = default)
    {
        var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges, ct);

        if (company is null) throw new CompanyNotFoundException(companyId);

        var employeeFromDb = await _repository.Employee.GetEmployeeAsync(companyId, employeeId, trackChanges, ct);

        if (employeeFromDb is null) throw new EmployeeNotFoundException(employeeId);

        var employee = _mapper.Map<EmployeeDto>(employeeFromDb);

        return employee;
    }

    public async Task<EmployeeDto> CreateEmployeeForCompanyAsync(Guid companyId, EmployeeForCreationDto employeeForCreation,
        bool trackChanges, CancellationToken ct = default)
    {
        var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges, ct);

        if (company is null) throw new CompanyNotFoundException(companyId);

        var employeeEntity = _mapper.Map<Employee>(employeeForCreation);

        _repository.Employee.CreatEmployeeForCompany(companyId, employeeEntity);

        await _repository.SaveAsync(ct);

        var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntity);

        return employeeToReturn;
    }

    public async Task DeleteEmployeeForCompanyAsync(Guid companyId, Guid employeeId, bool trackChanges, CancellationToken ct = default)
    {
        var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges, ct);

        if (company is null) throw new CompanyNotFoundException(companyId);

        var employeeForCompany = await _repository.Employee.GetEmployeeAsync(companyId, employeeId, trackChanges, ct);

        if (employeeForCompany is null) throw new EmployeeNotFoundException(employeeId);

        _repository.Employee.DeleteEmployee(employeeForCompany);

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
        var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges, ct);

        if (company is null) throw new CompanyNotFoundException(companyId);

        var employeeForCompany = await _repository.Employee.GetEmployeeAsync(companyId, employeeId, trackChanges, ct);

        if (employeeForCompany is null) throw new EmployeeNotFoundException(employeeId);

        _repository.Employee.DeleteEmployeeAndCompany(company, employeeForCompany);
    }

    public async Task UpdateEmployeeForCompanyAsync(Guid companyId, Guid employeeId, EmployeeForManipulationDto employeeForUpdate,
        bool compTrackChanges, bool empTrackChanges, CancellationToken ct = default)
    {
        var company = await _repository.Company.GetCompanyAsync(companyId, compTrackChanges, ct);

        if (company is null) throw new CompanyNotFoundException(companyId);

        var employeeEntity = await _repository.Employee.GetEmployeeAsync(companyId, employeeId, empTrackChanges, ct);

        if (employeeEntity is null) throw new EmployeeNotFoundException(employeeId);

        _mapper.Map(employeeForUpdate, employeeEntity);

        await _repository.SaveAsync();
    }
}