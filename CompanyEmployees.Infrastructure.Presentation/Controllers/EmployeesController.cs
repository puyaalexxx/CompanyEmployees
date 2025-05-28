using CompanyEmployees.Core.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects;

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

    [HttpGet("{employeeId:guid}", Name = "GetEmployeeForCompany")]
    public IActionResult GetEmployeeForCompany(Guid companyId, Guid employeeId)
    {
        var employee = _service.EmployeeService.GetEmployee(companyId, employeeId, false);

        return Ok(employee);
    }

    [HttpPost]
    public IActionResult CreateEmployeeForCompany(Guid companyId, [FromBody] EmployeeForCreationDto employee)
    {
        if (employee is null) return BadRequest("EmployeeForCreationDto object is null");

        var employeeToReturn =
            _service.EmployeeService.CreateEmployeeForCompany(companyId, employee, trackChanges: false);
        
        return CreatedAtRoute("GetEmployeeForCompany", 
            new { companyId = companyId, employeeId = employeeToReturn.Id }, employeeToReturn);
    }

    [HttpDelete("{employeeId:guid}")]
    public IActionResult DeleteEmployeeForCompany(Guid companyId, Guid employeeId)
    {
        _service.EmployeeService.DeleteEmployeeForCompany(companyId, employeeId, trackChanges: false);
        
        return NoContent();
    }

    [HttpDelete("remove-employee-and-company/{employeeId:guid}")]
    public IActionResult DeleteEmployeeForCompanyAndCompany(Guid companyId, Guid employeeId)
    {
        return DeleteEmployeeForCompany(companyId, employeeId);
    }

    [HttpPut("{employeeId:guid}")]
    public IActionResult UpdateEmployeeForCompany(Guid companyId, Guid employeeId, [FromBody] EmployeeForUpdateDto employee)
    {
        if (employee is null) return BadRequest("EmployeeForUpdateDto object is null");

        _service.EmployeeService.UpdateEmployeeForCompany(companyId, employeeId, employee, 
            compTrackChanges: false, empTrackChanges: true);

        return NoContent();
    }
}