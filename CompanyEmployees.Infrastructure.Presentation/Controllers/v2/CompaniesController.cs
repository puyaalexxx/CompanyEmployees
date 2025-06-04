using Asp.Versioning;
using CompanyEmployees.Core.Services.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.RequestFeatures;
using System.Text.Json;

namespace CompanyEmployees.Infrastructure.Presentation.Controllers.v2;

[ApiVersion("2.0", Deprecated = true)]
//enable URL versioning
//[Route("/api/{v:apiversion}/companies")]
[Route("/api/companies")]
[ApiController]
public class CompaniesController : ControllerBase
{
    private readonly IServiceManager _service;

    public CompaniesController(IServiceManager service)
    {
        _service = service;
    }

    [HttpGet(Name = "GetCompanies")]
    public async Task<IActionResult> GetAllCompanies([FromQuery] CompanyParameters companyParameters, CancellationToken ct)
    {
        var pagedResult = await _service.CompanyService.GetAllCompaniesAsync(companyParameters, false, ct);

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagedResult.metaData));

        return Ok(pagedResult.companies);
    }


}
