using Account.Api.Contracts;
using Account.Application.Accounts.Dtos;
using Account.Domain.Accounts;
using FluentAssertions;
using SharedKernel.Web.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Account.Api.IntegrationTests.Controllers
{
    public sealed class AccountsControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public AccountsControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Post_Accounts_ShouldPersistAndReturnEnvelope()
        {
            await DatabaseFixture.ResetAsync(_factory.Services);

            var request = new OpenAccountRequest(
                CustomerId: Guid.NewGuid(),
                Currency: "NGN",
                AccountType: AccountType.Savings);

            var response = await _client.PostAsJsonAsync("/api/accounts", request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var envelope = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
            envelope.Should().NotBeNull();
            envelope!.Success.Should().BeTrue();
            envelope.TraceId.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Post_Holds_ShouldReduceAvailableBalance()
        {
            await DatabaseFixture.ResetAsync(_factory.Services);

            // open account
            var open = new OpenAccountRequest(Guid.NewGuid(), "NGN", AccountType.Savings);
            var openResp = await _client.PostAsJsonAsync("/api/accounts", open);
            openResp.StatusCode.Should().Be(HttpStatusCode.OK);


            var openEnvelope = await openResp.Content.ReadFromJsonAsync<ApiResponse<AccountDto>>();

            openEnvelope.Should().NotBeNull();
            openEnvelope!.Success.Should().BeTrue();

            var accountId = openEnvelope.Data!.Id;

            // place hold
            var holdReq = new PlaceHoldRequest(Amount: 100m, Currency: "NGN", Reason: "Card auth", ExpiresAt: null);
            var holdResp = await _client.PostAsJsonAsync($"/api/accounts/{accountId}/holds", holdReq);

            holdResp.StatusCode.Should().Be(HttpStatusCode.OK);

            var holdEnvelope = await holdResp.Content.ReadFromJsonAsync<ApiResponse<object>>();
            holdEnvelope!.Success.Should().BeTrue();
        }
    }
}
