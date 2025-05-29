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
    public async Task<IActionResult> GetEmployees(Guid companyId, CancellationToken ct)
    {
        var employees = await _service.EmployeeService.GetEmployeesAsync(companyId, false, ct);

        return Ok(employees);
    }

    [HttpGet("{employeeId:guid}", Name = "GetEmployeeForCompany")]
    public async Task<IActionResult> GetEmployeeForCompany(Guid companyId, Guid employeeId, CancellationToken ct)
    {
        var employee = await _service.EmployeeService.GetEmployeeAsync(companyId, employeeId, false, ct);

        return Ok(employee);
    }

    [HttpPost]
    public async Task<IActionResult> CreateEmployeeForCompany(Guid companyId, [FromBody] EmployeeForCreationDto employee,
        [FromServices] IValidator<EmployeeForCreationDto> validator, CancellationToken ct)
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
            await _service.EmployeeService.CreateEmployeeForCompanyAsync(companyId, employee, trackChanges: false, ct);

        return CreatedAtRoute("GetEmployeeForCompany",
            new { companyId = companyId, employeeId = employeeToReturn.Id }, employeeToReturn);
    }

    [HttpDelete("{employeeId:guid}")]
    public async Task<IActionResult> DeleteEmployeeForCompany(Guid companyId, Guid employeeId, CancellationToken ct)
    {
        await _service.EmployeeService.DeleteEmployeeForCompanyAsync(companyId, employeeId, trackChanges: false, ct);

        return NoContent();
    }

    [HttpPut("{employeeId:guid}")]
    public async Task<IActionResult> UpdateEmployeeForCompany(Guid companyId, Guid employeeId, [FromBody] EmployeeForUpdateDto employee,
        [FromServices] IValidator<EmployeeForUpdateDto> validator, CancellationToken ct)
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


        await _service.EmployeeService.UpdateEmployeeForCompanyAsync(companyId, employeeId, employee,
            compTrackChanges: false, empTrackChanges: true, ct);

        return NoContent();
    }

    /// <summary>
    /// Remove employee from the company and delete the company if it has no employees left. (Additional*)
    /// </summary>
    /// <param name="companyId"></param>
    /// <param name="employeeId"></param>
    /// <returns></returns>
    [HttpDelete("remove-employee-and-company/{employeeId:guid}")]
    public async Task<IActionResult> DeleteEmployeeForCompanyAndCompanyAsync(Guid companyId, Guid employeeId, CancellationToken ct)
    {
        return await DeleteEmployeeForCompany(companyId, employeeId, ct);
    }
}