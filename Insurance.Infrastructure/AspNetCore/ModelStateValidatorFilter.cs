using System;
using System.Linq;
using Insurance.Domain.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Insurance.Infrastructure.AspNetCore
{
    public class ModelStateValidatorFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            //
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var message = string.Join(" ", context.ModelState.SelectMany(m => m.Value.Errors.Select(e => e.ErrorMessage.EnsureEndsWithDot())));
                context.Result = new BadRequestObjectResult(ServiceResponse.Fail(StatusCodes.Status400BadRequest, "G001", message));
            }
        }
    }
}