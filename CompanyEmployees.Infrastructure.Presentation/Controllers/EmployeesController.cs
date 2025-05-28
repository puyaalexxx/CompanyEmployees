// Ignore Spelling: validator

using CompanyEmployees.Core.Services.Abstractions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Infrastructure.Presentation.Controllers;

[ApiController]
[Route("api/companies/{companyId}/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IServiceManager _service;
    //using FluentValidations to validate the EmployeeForCreationDto object - if you dont use FromServuces atrribute
    //private readonly IValidator<EmployeeForCreationDto> _postValidator;
    //private readonly IValidator<EmployeeForUpdateDto> _putValidator;

    public EmployeesController(IServiceManager service /*IValidator<EmployeeForCreationDto> postValidator, IValidator<EmployeeForUpdateDto> putValidator*/)
    {
        _service = service;
        // _postValidator = postValidator;
        // _putValidator = putValidator;
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
    public IActionResult CreateEmployeeForCompany(Guid companyId, [FromBody] EmployeeForCreationDto employee,
        [FromServices] IValidator<EmployeeForCreationDto> validator)
    {
        if (employee is null) return BadRequest("EmployeeForCreationDto object is null");

        //use Attributes for validation
        //if (!ModelState.IsValid) return UnprocessableEntity(ModelState);

        //use FluentValidation for validation
        var valResult = validator.Validate(employee);
        if (!valResult.IsValid)
        {
            return UnprocessableEntity(valResult.ToDictionary());
        }

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

    [HttpPut("{employeeId:guid}")]
    public IActionResult UpdateEmployeeForCompany(Guid companyId, Guid employeeId, [FromBody] EmployeeForUpdateDto employee,
        [FromServices] IValidator<EmployeeForUpdateDto> validator)
    {
        if (employee is null) return BadRequest("EmployeeForUpdateDto object is null");

        //use Attributes for validation
        //if (!ModelState.IsValid) return UnprocessableEntity(ModelState);

        //use FluentValidation for validation
        var valResult = validator.Validate(employee);
        if (!valResult.IsValid)
        {
            return UnprocessableEntity(valResult.ToDictionary());
        }


        _service.EmployeeService.UpdateEmployeeForCompany(companyId, employeeId, employee,
            compTrackChanges: false, empTrackChanges: true);

        return NoContent();
    }

    /// <summary>
    /// Remove employee from the company and delete the company if it has no employees left. (Additional*)
    /// </summary>
    /// <param name="companyId"></param>
    /// <param name="employeeId"></param>
    /// <returns></returns>
    [HttpDelete("remove-employee-and-company/{employeeId:guid}")]
    public IActionResult DeleteEmployeeForCompanyAndCompany(Guid companyId, Guid employeeId)
    {
        return DeleteEmployeeForCompany(companyId, employeeId);
    }
}