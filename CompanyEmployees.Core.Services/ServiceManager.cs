using CompanyEmployees.Core.Domain.Repositories;
using CompanyEmployees.Core.Services.Abstractions;
using LoggingService;

namespace CompanyEmployees.Core.Services;

public class ServiceManager : IServiceManager
{
    private readonly Lazy<ICompanyService> _companyService;
    private readonly Lazy<IEmployeeService> _employeeService;


    public ServiceManager(IRepositoryManager repositoryManager, ILoggerManager loggerManager)
    {
        _companyService = new Lazy<ICompanyService>(() => new CompanyService(repositoryManager, loggerManager));
        _employeeService = new Lazy<IEmployeeService>(() => new EmployeeService(repositoryManager, loggerManager));
    }

    public ICompanyService CompanyService => _companyService.Value;
    public IEmployeeService EmployeeService => _employeeService.Value;
}