using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Web.Api
{
    [ApiController]
    public abstract class ApiControllerBase : ControllerBase
    {
        protected string TraceId => HttpContext?.TraceIdentifier ?? string.Empty;

        protected ActionResult<ApiResponse<T>> OkResponse<T>(T data)
            => Ok(ApiResponse<T>.Ok(data, TraceId));

        protected ActionResult<ApiResponse<T>> CreatedResponse<T>(string location, T data)
            => Created(location, ApiResponse<T>.Ok(data, TraceId));

        // Useful for async workflows where you return 202 + operation metadata in the Data
        protected ActionResult<ApiResponse<T>> AcceptedResponse<T>(T data)
            => Accepted(ApiResponse<T>.Ok(data, TraceId));

        protected ActionResult<ApiResponse<T>> BadRequestResponse<T>(
            string errorCode,
            string errorMessage,
            IDictionary<string, string[]>? details = null)
            => BadRequest(ApiResponse<T>.Fail(errorCode, errorMessage, TraceId, details));

        protected ActionResult<ApiResponse<T>> NotFoundResponse<T>(string message = "Resource not found.")
            => NotFound(ApiResponse<T>.Fail(ErrorCodes.NotFound, message, TraceId));

        protected ActionResult<ApiResponse<T>> ConflictResponse<T>(string errorCode, string message)
            => Conflict(ApiResponse<T>.Fail(errorCode, message, TraceId));
    }
}
