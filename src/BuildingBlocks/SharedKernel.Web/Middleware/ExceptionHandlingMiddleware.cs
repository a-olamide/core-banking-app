using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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
            catch (DomainException ex) when (ex.ErrorCode == ErrorCodes.CustomerEmailAlreadyExists)
            {
                await WriteError(context, HttpStatusCode.Conflict, ex.ErrorCode, ex.Message);
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
            catch (DbUpdateException ex) when (IsEmailUniqueConstraintViolation(ex))
            {
                await WriteError(
                    context,
                    HttpStatusCode.Conflict,
                    ErrorCodes.CustomerEmailAlreadyExists,
                    "Email already exists.");
            }
            catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
            {
                await WriteError(
                    context,
                    HttpStatusCode.Conflict,
                    ErrorCodes.CustomerEmailAlreadyExists,
                    "Email already exists.");
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
            if (context.Response.HasStarted)
                throw new InvalidOperationException("The response has already started.");
            context.Response.Clear();

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var traceId = context.TraceIdentifier;
            var response = ApiResponse<object>.Fail(code, message, traceId, details);

            await context.Response.WriteAsJsonAsync(response);
        }
        private static bool IsUniqueConstraintViolation(DbUpdateException ex)
        {
            // SQL Server unique index/constraint violations:
            // 2601: Cannot insert duplicate key row in object with unique index
            // 2627: Violation of UNIQUE KEY constraint
            if (ex.InnerException is SqlException sqlEx)
                return sqlEx.Number is 2601 or 2627;

            return false;
        }
        private static bool IsEmailUniqueConstraintViolation(DbUpdateException ex)
        {
            if (ex.InnerException is not SqlException sqlEx)
                return false;

            if (sqlEx.Number is not (2601 or 2627))
                return false;

            // Only treat EMAIL uniqueness as CUSTOMER_EMAIL_EXISTS
            return sqlEx.Message.Contains("UX_Customers_Email", StringComparison.OrdinalIgnoreCase);
        }
    }
}
