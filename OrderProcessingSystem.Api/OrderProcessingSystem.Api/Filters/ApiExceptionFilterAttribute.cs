using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OrderProcessingSystem.Application.Common.Exceptions;

namespace OrderProcessingSystem.Api.Filters
{
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is ValidationException validationException)
            {
                var details = new ValidationProblemDetails(validationException.Errors)
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Title = "Validation Error"
                };

                context.Result = new BadRequestObjectResult(details);
                context.ExceptionHandled = true;
            }
        }
    }
}
