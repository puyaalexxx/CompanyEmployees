using Asp.Versioning;
using CompanyEmployees.Core.Services.Abstractions;
using CompanyEmployees.Infrastructure.Presentation.ActionFilters;
using CompanyEmployees.Infrastructure.Presentation.ModelBinders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.RateLimiting;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;
using System.Text.Json;

namespace CompanyEmployees.Infrastructure.Presentation.Controllers.v1;

[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "v1")]
//[Route("/api/{v:apiversion}/[controller]")] //enable URL versioning

[Route("/api/[controller]")]
[ApiController]
//[ResponseCache(CacheProfileName = "120SecondsDuration")]
[OutputCache(PolicyName = "120SecondsDuration")]
public class CompaniesController : ControllerBase
{
    private readonly IServiceManager _service;

    public CompaniesController(IServiceManager service)
    {
        _service = service;
    }

    /// <summary>
    /// GetsThe list of all companies with pagination and filtering options.
    /// </summary>
    /// <param name="companyParameters">Params</param>
    /// <param name="ct">Params</param>
    /// <returns></returns>
    [HttpGet(Name = "GetCompanies")]
    [EnableRateLimiting("RateLimitPolicy")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> GetAllCompanies([FromQuery] CompanyParameters companyParameters, CancellationToken ct)
    {
        var pagedResult = await _service.CompanyService.GetAllCompaniesAsync(companyParameters, false, ct);

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagedResult.metaData));

        return Ok(pagedResult.companies);
    }

    [HttpGet("{id:guid}", Name = "CompanyById")]
    [OutputCache(Duration = 60)]
    //[ResponseCache(Duration = 60)]
    public async Task<IActionResult> GetCompany(Guid id, CancellationToken ct)
    {
        var company = await _service.CompanyService.GetCompanyAsync(id, trackChanges: false, ct);

        //cache revalidation
        var etag = $"\"{Guid.NewGuid():n}\"";
        HttpContext.Response.Headers.ETag = etag;

        return Ok(company);
    }

    [HttpGet("collection/({ids})", Name = "CompanyCollection")]
    public async Task<IActionResult> GetCompanyCollection(
        [ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids, CancellationToken ct)
    {
        var companies = await _service.CompanyService.GetByIdsAsync(ids, trackChanges: false, ct);

        return Ok(companies);
    }


    /// <summary>
    /// Creates a newly created company
    /// </summary>
    /// <param name="company"></param>
    /// <returns>A newly created company</returns>
    /// <response code="201">Returns the newly created item</response>
    /// <response code="400">If the item is null</response>
    /// <response code="422">If the model is invalid</response>
    [HttpPost(Name = "CreateCompany")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(422)]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> CreateCompany([FromBody] CompanyForCreationDto company, CancellationToken ct)
    {
        var createdCompany = await _service.CompanyService.CreateCompanyAsync(company, ct);

        return CreatedAtRoute("CompanyById", new { id = createdCompany.Id }, createdCompany);
    }

    [HttpPost("collection")]
    public async Task<IActionResult> CreateCompanyCollection(
        [FromBody] IEnumerable<CompanyForCreationDto> companyCollection, CancellationToken ct)
    {
        var result = await _service.CompanyService.CreateCompanyCollectionAsync(companyCollection, ct);

        return CreatedAtRoute("CompanyCollection", new { result.ids }, result.companies);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCompany(Guid id, CancellationToken ct)
    {
        await _service.CompanyService.DeleteCompanyAsync(id, trackChanges: false, ct);

        return NoContent();
    }

    [HttpPut("{companyId:guid}")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateCompany(Guid companyId,
        [FromBody] CompanyForCreationDto company, CancellationToken ct)
    {
        await _service.CompanyService.UpdateCompanyAsync(companyId, company, trackChanges: true, ct);

        return NoContent();
    }

    [HttpOptions]
    public IActionResult GetCompaniesOptions()
    {
        Response.Headers.Add("Allow", "GET, POST, DELETE, PUT, OPTIONS");

        return Ok();
    }

    /// <summary>
    /// Disable output caching for this action only
    /// </summary>
    /// <returns></returns>
    [HttpGet("output-nocache")]
    [OutputCache(NoStore = true)]
    public IActionResult NonCachedOutput()
    {
        return Ok("This is a non-cached output response.");
    }

    /// <summary>
    /// Enable the cache for the query string parameter
    /// </summary>
    /// <param name="firstKey"></param>
    /// <param name="secondKey"></param>
    /// <returns></returns>
    [HttpGet("output-varybykey")]
    [OutputCache(VaryByQueryKeys = new[] { nameof(firstKey) }, Duration = 10)]
    public IActionResult VaryByKey(string firstKey, string secondKey)
    {
        return Ok($"{firstKey} {secondKey} - retrieved at {DateTime.Now}");
    }
}