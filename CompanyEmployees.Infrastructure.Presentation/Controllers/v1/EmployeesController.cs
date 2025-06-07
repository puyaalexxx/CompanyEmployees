// Ignore Spelling: validator

using CompanyEmployees.Core.Domain.LinkModels;
using CompanyEmployees.Core.Services.Abstractions;
using CompanyEmployees.Infrastructure.Presentation.ActionFilters;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;
using System.Text.Json;

namespace CompanyEmployees.Infrastructure.Presentation.Controllers.v1;

[ApiController]
[Route("api/companies/{companyId}/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IServiceManager _service;
    //using FluentValidations to validate the EmployeeForCreationDto object - if you don't use FromServuces attribute
    //private readonly IValidator<EmployeeForCreationDto> _postValidator;
    //private readonly IValidator<EmployeeForUpdateDto> _putValidator;

    public EmployeesController(IServiceManager service /*IValidator<EmployeeForCreationDto> postValidator, IValidator<EmployeeForUpdateDto> putValidator*/)
    {
        _service = service;
        // _postValidator = postValidator;
        // _putValidator = putValidator;
    }

    [HttpGet]
    [HttpHead]
    [ServiceFilter(typeof(ValidateMediaAttribute))]
    public async Task<IActionResult> GetEmployeesForCompany(Guid companyId,
        [FromQuery] EmployeeParameters employeeParameters, CancellationToken ct)
    {
        var linkParams = new LinkParameters(employeeParameters, HttpContext);
        var result = await _service.EmployeeService.GetEmployeesAsync(companyId, linkParams, false, ct);

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(result.metaData));

        return result.linkResponse.HasLinks ? Ok(result.linkResponse.LinkedEntities) : Ok(result.linkResponse.ShapedEntities);
    }

    [HttpGet("{employeeId:guid}", Name = "GetEmployeeForCompany")]
    public async Task<IActionResult> GetEmployeeForCompany(Guid companyId, Guid employeeId, CancellationToken ct)
    {
        var employees = await _service.EmployeeService.GetEmployeeAsync(companyId, employeeId, false, ct);

        return Ok(employees);
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
            new { companyId, employeeId = employeeToReturn.Id }, employeeToReturn);
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
    [HttpDelete("remove-employee-and-company/{employeeId:guid}")]
    public async Task<IActionResult> DeleteEmployeeForCompanyAndCompanyAsync(Guid companyId, Guid employeeId, CancellationToken ct)
    {
        return await DeleteEmployeeForCompany(companyId, employeeId, ct);
    }
}