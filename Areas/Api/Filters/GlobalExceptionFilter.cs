using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;


namespace Lemoo_pos.Areas.Api.Filters
{
    public class GlobalExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {

            // Map exception to HTTP status code
            int statusCode = context.Exception switch
            {
                UnauthorizedAccessException => StatusCodes.Status403Forbidden, // 403 Forbidden
                ArgumentException => StatusCodes.Status400BadRequest,         // 400 Bad Request
                KeyNotFoundException => StatusCodes.Status404NotFound,        // 404 Not Found
                NotSupportedException => StatusCodes.Status405MethodNotAllowed, // 405 Method Not Allowed
                InvalidOperationException => StatusCodes.Status409Conflict,   // 409 Conflict
                TimeoutException => StatusCodes.Status408RequestTimeout,      // 408 Request Timeout
                _ => StatusCodes.Status500InternalServerError                 // Default: 500 Internal Server Error
            };

            // Create response object
            var errorResponse = new
            {
                StatusCode = statusCode,
                context.Exception.Message
            };

            // Set the result
            context.Result = new JsonResult(errorResponse)
            {
                StatusCode = statusCode
            };

            // Optionally log the exception
            System.Console.WriteLine(context.Exception.GetBaseException());
            Console.WriteLine($"Exception caught in filter: {context.Exception.Message}");
        }
    }
}