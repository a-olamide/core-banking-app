using Customer.Api.Contracts;
using Customer.Application.Customers.Command.CreateCustomer;
using Customer.Application.Customers.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Web.Api;

namespace Customer.Api.Controllers
{
    [Route("api/customers")]
    public sealed class CustomersController : ApiControllerBase
    {
        private readonly ISender _sender;

        public CustomersController(ISender sender) => _sender = sender;

        [HttpPost]
        public async Task<ActionResult<ApiResponse<CustomerDto>>> Create(
            [FromBody] CreateCustomerRequest request, CancellationToken ct)
        {
            var cmd = new CreateCustomerCommand(
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
                request.CountryCode
            );

            var result = await _sender.Send(cmd, ct);

            return OkResponse(result);
            // Later: CreatedResponse($"/api/customers/{result.Id}", result);
        }
    }
}
