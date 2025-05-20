using CompanyEmployees.Core.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace CompanyEmployees.Infrastructure.Presentation.Controllers;

[Route("/api/[controller]")]
[ApiController]
public class CompaniesController : ControllerBase
{
    private readonly IServiceManager _service;

    public CompaniesController(IServiceManager service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult GetAllCompanies(bool trackChanges)
    {
        try
        {
            var companies = _service.CompanyService.GetAllCompanies(trackChanges: false);
            
            return Ok(companies);
        }
        catch (Exception e)
        {
            return StatusCode(500, "Internal Server Error");
        }
    }
}