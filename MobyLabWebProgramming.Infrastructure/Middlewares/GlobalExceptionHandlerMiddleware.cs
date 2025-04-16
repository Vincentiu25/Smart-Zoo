using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MobyLabWebProgramming.Core.Errors;

namespace MobyLabWebProgramming.Infrastructure.Middlewares;

/// <summary>
/// This is the global exception handler/middleware, when a HTTP request arrives it is invoked,
/// if an uncaught exception is caught here it sends a error message back to the client.
/// </summary>
public class GlobalExceptionHandlerMiddleware(ILogger<GlobalExceptionHandlerMiddleware> logger, RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context); // Here the next middleware is invoked, the last middleware invoked calls the corresponding controller method for the specified route.
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Caught exception in global exception handler!");

            var response = context.Response;
            response.ContentType = "application/json";

            HttpStatusCode status;
            string message;

            switch (ex)
            {
                case ServerException serverException:
                    status = serverException.Status;
                    message = serverException.Message;
                    break;

                case ArgumentNullException or ArgumentException:
                    status = HttpStatusCode.BadRequest;
                    message = ex.Message;
                    break;

                case KeyNotFoundException:
                    status = HttpStatusCode.NotFound;
                    message = ex.Message;
                    break;

                case UnauthorizedAccessException:
                    status = HttpStatusCode.Unauthorized;
                    message = ex.Message;
                    break;

                default:
                    status = HttpStatusCode.InternalServerError;
                    message = "An unexpected error occurred!";
                    break;
            }

            response.StatusCode = (int)status;
            await response.WriteAsync(JsonSerializer.Serialize(new ErrorMessage(status, message)));
        }
    }
}
