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

    public IEnumerable<EmployeeDto> GetEmployees(Guid companyId, bool trackChanges)
    {
        var company = _repository.Company.GetCompany(companyId, trackChanges);

        if (company is null) throw new CompanyNotFoundException(companyId);

        var employeesFromDb = _repository.Employee.GetEmployees(companyId, trackChanges);
        var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesFromDb);

        return employeesDto;
    }

    public EmployeeDto GetEmployee(Guid companyId, Guid employeeId, bool trackChanges)
    {
        var company = _repository.Company.GetCompany(companyId, trackChanges);

        if (company is null) throw new CompanyNotFoundException(companyId);

        var employeeFromDb = _repository.Employee.GetEmployee(companyId, employeeId, trackChanges);

        if (employeeFromDb is null) throw new EmployeeNotFoundException(employeeId);

        var employee = _mapper.Map<EmployeeDto>(employeeFromDb);

        return employee;
    }

    public EmployeeDto CreateEmployeeForCompany(Guid companyId, EmployeeForCreationDto employeeForCreation, bool trackChanges)
    {
        var company = _repository.Company.GetCompany(companyId, trackChanges);
        
        if (company is null) throw new CompanyNotFoundException(companyId);
        
        var employeeEntity = _mapper.Map<Employee>(employeeForCreation);
        
        _repository.Employee.CreatEmployeeForCompany(companyId, employeeEntity);
        _repository.Save();
        
        var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntity);

        return employeeToReturn;
    }
}