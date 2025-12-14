using Microsoft.AspNetCore.Mvc;
using SharedKernel.Web.Api;
using SharedKernel.Domain.Exceptions;

namespace Customer.Api.Controllers
{
    public sealed record OkTestResponse(string Message);

    [Route("api/test")]
    public sealed class TestController : ApiControllerBase
    {
        [HttpGet("ok")]
        public ActionResult<ApiResponse<OkTestResponse>> OkTest()
            => OkResponse(new OkTestResponse("Pipeline OK"));

        [HttpGet("domain-error")]
        public IActionResult DomainError()
            => throw new DomainException("TEST_DOMAIN_ERROR", "This is a domain exception test.");

        [HttpGet("validation-error")]
        public IActionResult ValidationError()
        {
            var details = new Dictionary<string, string[]>
            {
                ["email"] = new[] { "Email is invalid." },
                ["firstName"] = new[] { "First name is required." }
            };

            throw new ValidationException(
                ErrorCodes.ValidationError,
                "Validation failed.",
                details);
        }

        [HttpGet("crash")]
        public IActionResult Crash()
            => throw new Exception("Boom!");
    }
}
