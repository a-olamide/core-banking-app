using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Web.Api
{
    public sealed record ApiResponse<T>(
    bool Success,
    T? Data,
    ApiError? Error,
    string? TraceId
)
    {
        public static ApiResponse<T> Ok(T data, string? traceId = null)
            => new(true, data, null, traceId);

        public static ApiResponse<T> Fail(string code, string message, string? traceId = null,
            IDictionary<string, string[]>? details = null)
            => new(false, default, new ApiError(code, message, details), traceId);
    }
}
