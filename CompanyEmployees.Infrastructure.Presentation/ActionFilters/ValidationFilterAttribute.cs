using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyEmployees.Infrastructure.Presentation.ActionFilters
{
    public class ValidationFilterAttribute : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Removed problematic code as ActionExecutedContext does not have ActionArguments property.
            // No replacement is needed here since the variable 'param' was unused.
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Implementing OnActionExecuting to handle validation logic.
            var action = context.RouteData.Values["action"]?.ToString();
            var controller = context.RouteData.Values["controller"]?.ToString();

            // Correctly accessing ActionArguments from ActionExecutingContext.
            var param = context.ActionArguments
                .SingleOrDefault(x => x.Value != null && x.Value.ToString().Contains("Dto")).Value;

            if (param == null)
            {
                context.Result = new BadRequestObjectResult($"Object is null. Controller: {controller}, Action: {action}");

                return;
            }

            if (!context.ModelState.IsValid)
            {
                context.Result = new UnprocessableEntityObjectResult(context.ModelState);
            }
        }
    }
}
