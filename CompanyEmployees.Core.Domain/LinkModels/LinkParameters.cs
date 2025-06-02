using Microsoft.AspNetCore.Http;
using Shared.RequestFeatures;

namespace CompanyEmployees.Core.Domain.LinkModels
{
    public record LinkParameters(EmployeeParameters EmployeeParameters, HttpContext Context);
}
