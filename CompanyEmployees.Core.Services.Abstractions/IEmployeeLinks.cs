using CompanyEmployees.Core.Domain.LinkModels;
using Microsoft.AspNetCore.Http;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Core.Services.Abstractions
{
    public interface IEmployeeLinks
    {
        LinkResponse TryGenerateLinks(IEnumerable<EmployeeDto> employeesDto, string fields, Guid companyId, HttpContext httpContext);
    }
}
