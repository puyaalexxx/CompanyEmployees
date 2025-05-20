using CompanyEmployees.Core.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace CompanyEmployees.Infrastructure.Presentation.Controllers;

[ApiController]
[Route("api/companies/{companyId}/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IServiceManager _service;

    public EmployeesController(IServiceManager service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult GetEmployees(Guid companyId)
    {
        var employees = _service.EmployeeService.GetEmployees(companyId, false);

        return Ok(employees);
    }

    [HttpGet("{employeeId:guid}")]
    public IActionResult GetEmployeeForCompany(Guid companyId, Guid employeeId)
    {
        var employee = _service.EmployeeService.GetEmployee(companyId, employeeId, false);

        return Ok(employee);
    }
}