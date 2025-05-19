using CompanyEmployees.Core.Domain.Repositories;
using CompanyEmployees.Core.Services.Abstractions;
using LoggingService;

namespace CompanyEmployees.Core.Services;

internal sealed class EmployeeService : IEmployeeService
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger;

    public EmployeeService(IRepositoryManager repository, ILoggerManager logger)
    {
        _repository = repository;
        _logger = logger;
    }
}