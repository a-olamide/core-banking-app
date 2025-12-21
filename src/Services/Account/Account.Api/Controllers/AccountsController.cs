using Account.Api.Contracts;
using Account.Application.Accounts.Commands.OpenAccount;
using Account.Application.Accounts.Commands.PlaceHold;
using Account.Application.Accounts.Commands.PlaceLien;
using Account.Application.Accounts.Commands.ReleaseHold;
using Account.Application.Accounts.Commands.ReleaseLien;
using Account.Application.Accounts.Dtos;
using Account.Application.Accounts.Queries.GetAccountsByCustomer;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Web.Api;

namespace Account.Api.Controllers
{
    [Route("api/accounts")]
    public sealed class AccountsController : ApiControllerBase
    {
        private readonly IMediator _mediator;

        public AccountsController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<ActionResult<ApiResponse<AccountDto>>> OpenAccount(OpenAccountRequest request, CancellationToken ct)
        {
            var cmd = new OpenAccountCommand(
                request.CustomerId,
                request.Currency,
                (int)request.AccountType);

            var result = await _mediator.Send(cmd, ct);
            return OkResponse(result);
        }

        [HttpGet("/api/customers/{customerId:guid}/accounts")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<AccountDto>>>> GetAccountsByCustomer(Guid customerId, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetAccountsByCustomerQuery(customerId), ct);
            return OkResponse(result);
        }

        [HttpPost("{accountId:guid}/holds")]
        public async Task<ActionResult<ApiResponse<HoldDto>>> PlaceHold(Guid accountId, PlaceHoldRequest request, CancellationToken ct)
        {
            var result = await _mediator.Send(new PlaceHoldCommand(
                accountId,
                request.Amount,
                request.Currency,
                request.Reason,
                request.ExpiresAt), ct);

            return OkResponse(result);
        }

        [HttpDelete("{accountId:guid}/holds/{holdId:guid}")]
        public async Task<ActionResult<ApiResponse<object>>> ReleaseHold(Guid accountId, Guid holdId, [FromQuery] string? reason, CancellationToken ct)
        {
            await _mediator.Send(new ReleaseHoldCommand(accountId, holdId, reason), ct);
            return OkResponse<object>(new { Released = true });
        }

        [HttpPost("{accountId:guid}/liens")]
        public async Task<ActionResult<ApiResponse<LienDto>>> PlaceLien(Guid accountId, PlaceLienRequest request, CancellationToken ct)
        {
            var result = await _mediator.Send(new PlaceLienCommand(
                accountId,
                request.Amount,
                request.Currency,
                request.Reason,
                request.Reference), ct);

            return OkResponse(result);
        }

        [HttpDelete("{accountId:guid}/liens/{lienId:guid}")]
        public async Task<ActionResult<ApiResponse<object>>> ReleaseLien(Guid accountId, Guid lienId, [FromQuery] string? reason, CancellationToken ct)
        {
            await _mediator.Send(new ReleaseLienCommand(accountId, lienId, reason), ct);
            return OkResponse<object>(new { Released = true });
        }
    }
}
