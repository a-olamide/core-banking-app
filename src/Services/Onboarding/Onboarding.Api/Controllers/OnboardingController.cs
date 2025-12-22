using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Onboarding.Api.Contracts;
using Onboarding.Infrastructure.Saga;
using SharedKernel.Messaging.Commands;
using SharedKernel.Web.Api;

namespace Onboarding.Api.Controllers
{
    [Route("api/onboarding")]
    public sealed class OnboardingController : ApiControllerBase
    {
        private readonly IPublishEndpoint _publish;
        private readonly OnboardingSagaDbContext _db;

        public OnboardingController(IPublishEndpoint publish, OnboardingSagaDbContext db)
        {
            _publish = publish;
            _db = db;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<object>>> Start(StartOnboardingRequest request, CancellationToken ct)
        {
            var operationId = Guid.NewGuid();

            await _publish.Publish(new StartOnboardingCommand(
                operationId,
                request.FirstName,
                request.MiddleName,
                request.LastName,
                request.Email,
                request.PhoneCountryCode,
                request.PhoneNumber,
                request.AddressLine1,
                request.AddressLine2,
                request.City,
                request.StateOrProvince,
                request.PostalCode,
                request.CountryCode,
                request.Currency,
                (int)request.AccountType
            ), ct);

            // 202 + operation metadata
            return AcceptedResponse<object>(new
            {
                operationId,
                status = "QUEUED"
            });
        }

        [HttpGet("{operationId:guid}")]
        public async Task<ActionResult<ApiResponse<OnboardingStatusDto>>> Get(Guid operationId, CancellationToken ct)
        {
            var saga = await _db.Set<OnboardingState>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.CorrelationId == operationId, ct);

            if (saga is null)
                return NotFoundResponse<OnboardingStatusDto>("Onboarding operation not found.");

            var dto = new OnboardingStatusDto(
                saga.CorrelationId,
                saga.CurrentState,
                saga.CustomerId,
                saga.AccountId,
                saga.AccountNumber,
                saga.ErrorCode,
                saga.ErrorMessage);

            return OkResponse(dto);
        }
    }
}
