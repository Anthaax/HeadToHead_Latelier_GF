using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HeadToHead_Latelier.API.Exceptions
{
    public static class ExceptionHandler
    {
        public static ObjectResult HandleException(Exception exception)
        {
            var response = exception switch
            {
                FileNotFoundException => new ExceptionResponses
                {
                    Message = "The requested resource could not be found.",
                    Details = exception.Message,
                    StatusCode = (int)HttpStatusCode.NotFound
                },
                ArgumentNullException => new ExceptionResponses
                {
                    Message = "A required parameter is missing.",
                    Details = exception.Message,
                    StatusCode = (int)HttpStatusCode.BadRequest
                },
                ArgumentException => new ExceptionResponses
                {
                    Message = "The provided parameter is invalid.",
                    Details = exception.Message,
                    StatusCode = (int)HttpStatusCode.BadRequest
                },
                InvalidOperationException => new ExceptionResponses
                {
                    Message = "The requested operation cannot be performed.",
                    Details = exception.Message,
                    StatusCode = (int)HttpStatusCode.BadRequest
                },
                UnauthorizedAccessException => new ExceptionResponses
                {
                    Message = "You are not authorized to access this resource.",
                    Details = exception.Message,
                    StatusCode = (int)HttpStatusCode.Forbidden
                },
                _ => new ExceptionResponses
                {
                    Message = "An internal server error has occurred.",
                    Details = exception.Message,
                    StatusCode = (int)HttpStatusCode.InternalServerError
                }
            };

            return new ObjectResult(response)
            {
                StatusCode = response.StatusCode
            };
        }
    }
}
