using CompanyEmployees.Core.Domain.Entities.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyEmployees.Infrastructure.Presentation.Controllers
{
    public class ApiControllerBaseForApiResponses : ControllerBase
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult ProcessError(ApiBaseResponse baseResponse)
        {
            return baseResponse switch
            {
                ApiNotFoundResponse => NotFound(new ProblemDetails
                {
                    Detail = ((ApiNotFoundResponse)baseResponse).Message,
                    Status = StatusCodes.Status404NotFound
                }),
                ApiBadRequestResponse => BadRequest(new ProblemDetails
                {
                    Detail = ((ApiBadRequestResponse)baseResponse).Message,
                    Status = StatusCodes.Status400BadRequest
                }),
                _ => throw new NotImplementedException()
            };
        }
    }
}
