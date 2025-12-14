using Microsoft.AspNetCore.Http;
using SharedKernel.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Web.Api
{
    public sealed class ExceptionHandlingMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (ValidationException ex)
            {
                await WriteError(
                    context,
                    HttpStatusCode.BadRequest,
                    ErrorCodes.ValidationError,
                    ex.Message,
                    ex.Details);
            }
            catch (DomainException ex)
            {
                await WriteError(context, HttpStatusCode.BadRequest, ex.ErrorCode, ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                await WriteError(context, HttpStatusCode.NotFound, ErrorCodes.NotFound, ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                await WriteError(context, HttpStatusCode.Unauthorized, ErrorCodes.Unauthorized, ex.Message);
            }

            catch (Exception)
            {
                await WriteError(context, HttpStatusCode.InternalServerError,
                    ErrorCodes.InternalServerError, "An unexpected error occurred.");
            }
        }

        private static async Task WriteError(
            HttpContext context,
            HttpStatusCode statusCode,
            string code,
            string message,
            IDictionary<string, string[]>? details = null)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var traceId = context.TraceIdentifier;
            var response = ApiResponse<object>.Fail(code, message, traceId, details);

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
