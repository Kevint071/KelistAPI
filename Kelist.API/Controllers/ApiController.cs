using ErrorOr;
using Kelist.API.Common.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Kelist.API.Controllers
{
    [ApiController]
    public class ApiController : ControllerBase
    {
        protected ActionResult Problem(List<Error> errors)
        {
            if (errors == null || errors.Count is 0) return Problem();

            if (errors.All(error => error.Type == ErrorType.Validation)) return ValidationProblem(errors);

            HttpContext.Items[HttpContextItemKeys.Errors] = errors;
            return Problem(errors[0]);
        }

        private ObjectResult Problem(Error error)
        {
            var statusCode = error.Type switch
            {
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                _ => StatusCodes.Status500InternalServerError
            };

            string? instance = HttpContext?.Request != null
                ? HttpContext.Request.Path + HttpContext.Request.QueryString
                : null;

            return Problem(statusCode: statusCode, title: error.Description, instance: instance);
        }

        private ActionResult ValidationProblem(List<Error> errors)
        {
            var modelStateDictionary = new ModelStateDictionary();
            foreach (var error in errors)
            {
                modelStateDictionary.AddModelError(error.Code, error.Description);
            }
            return ValidationProblem(
                modelStateDictionary: modelStateDictionary,
                instance: HttpContext.Request.Path + HttpContext.Request.QueryString
            );
        }
    }
}
