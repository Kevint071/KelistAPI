using System.Net;
using System.Text.Json;
using Infrastructure.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Kelist.API.Middlewares
{
    public class GlobalExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
        private readonly Dictionary<Type, Func<HttpContext, Exception, Task>> _exceptionHandlers;

        public GlobalExceptionHandlingMiddleware(ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _logger = logger;
            _exceptionHandlers = new()
            {
                { typeof(DbUpdateException), HandleDbUpdateException },
                { typeof(Exception), HandleGenericException }
            };
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception exception)
            {
                var exceptionType = exception.GetType();
                if (_exceptionHandlers.TryGetValue(exceptionType, out var handler))
                {
                    await handler.Invoke(context, exception);
                }
                else
                {
                    await _exceptionHandlers[typeof(Exception)].Invoke(context, exception);
                }
            }
        }

        private async Task HandleDbUpdateException(HttpContext context, Exception ex)
        {
            var dbUpdateException = (DbUpdateException)ex;

            if (dbUpdateException.InnerException is not SqlException sqlException)
            {
                await HandleGenericException(context, dbUpdateException);
                _logger.LogWarning("There is an error of DbUpdateException that is not handled");
                return;
            }

            string detailMessage = sqlException.InnerException?.Message ?? sqlException.Message;
            string objectName = ErrorParser.ExtractObjectName(detailMessage);
            string duplicateValue = ErrorParser.ExtractDuplicateValue(detailMessage);

            int Status;
            string Type;
            string Title;
            string Detail;
            

            switch (sqlException.Number)
            {
                case 2601:
                    Status = (int)HttpStatusCode.Conflict;
                    Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.8";
                    Title = "Duplicate Key Error";
                    Detail = $"Cannot insert duplicate key row in object '{objectName}'. The duplicate value is '{duplicateValue}'";
                    
                    break;
                default:
                    Status = (int)HttpStatusCode.BadRequest;
                    Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1";
                    Title = "Bad Request";
                    Detail = "An unexpected database error occurred.";
                    break;
            }

            _logger.LogWarning(dbUpdateException, "DbUpdateException occurred: {Detail}", Detail);
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            ProblemDetails problem = new()
            {
                Status = Status,
                Type = Type,
                Title = Title,
                Detail = Detail,
                Instance = context.Request.Path
            };

            string json = JsonSerializer.Serialize(problem);
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(json);
        }

        private async Task HandleGenericException(HttpContext context, Exception ex)
        {
            _logger.LogError(ex, "An error occurred: {Message}", ex.Message);
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            ProblemDetails problem = new()
            {
                Status = (int)HttpStatusCode.InternalServerError,
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
                Title = "Server Error",
                Detail = "An internal server error has occurred while processing your request.",
                Instance = context.Request.Path

            };

            string json = JsonSerializer.Serialize(problem);
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(json);
        }
    }
}
