using CompanyEmployees.Core.Domain.Entities.Responses;
using CompanyEmployees.Core.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Infrastructure.Presentation.Controllers.v1
{
    [ApiController]
    [Route("api/companies/api-response")]
    public class CompaniesControllerWithApiResponse : ApiControllerBaseForApiResponses
    {
        private readonly IServiceManager _service;

        public CompaniesControllerWithApiResponse(IServiceManager service) => _service = service;

        [HttpGet]
        public IActionResult GetCompanies()
        {
            var baseResult = _service.CompanyService.GetAllCompanies(trackChanges: false);
            var companies = ((ApiOkResponse<IEnumerable<CompanyDto>>)baseResult).Result;

            return Ok(companies);
        }

        [HttpGet("{id:guid}")]
        public IActionResult GetCompany(Guid id)
        {
            var baseResult = _service.CompanyService.GetCompany(id, trackChanges: false);

            if (!baseResult.Success) return ProcessError(baseResult);

            var company = ((ApiOkResponse<CompanyDto>)baseResult).Result;

            return Ok(company);
        }
    }
}
