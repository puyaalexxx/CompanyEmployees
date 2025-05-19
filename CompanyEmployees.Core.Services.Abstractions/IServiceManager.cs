namespace CompanyEmployees.Core.Services.Abstractions;

public interface IServiceManager
{
    ICompanyService CompanyService { get; }
    IEmployeeService EmployeeService { get; }
}