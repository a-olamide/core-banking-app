using Customer.Api.Contracts;
using Customer.Application.Customers.Dtos;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using SharedKernel.Web.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Api.IntegrationTests.Controllers
{
    public sealed class CustomersControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public CustomersControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Post_Customers_ShouldReturnSuccessEnvelope()
        {
            // Arrange    
            var request = new CreateCustomerRequest(
                FirstName: "Ola",
                MiddleName: "B",
                LastName: "Akin",
                Email: "Ola.Akin@gmail.com",
                PhoneCountryCode: "+234",
                PhoneNumber: "8012345678",
                AddressLine1: "12 Admiralty Way",
                AddressLine2: null,
                City: "Lagos",
                StateOrProvince: "Lagos",
                PostalCode: "100001",
                CountryCode: "NG"
            );

            // Act
            var response = await _client.PostAsJsonAsync("/api/customers", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var envelope = await response.Content.ReadFromJsonAsync<ApiResponse<CustomerDto>>();
            envelope.Should().NotBeNull();
            envelope!.Success.Should().BeTrue();
            envelope.Error.Should().BeNull();
            envelope.TraceId.Should().NotBeNullOrEmpty();
            envelope.Data!.Email.Should().Be("ola.akin@gmail.com");
        }

        [Fact]
        public async Task Post_Customers_ShouldReturnValidationErrorEnvelope_WhenInvalid()
        {
            // Arrange
            var request = new CreateCustomerRequest(
                FirstName: "",
                MiddleName: null,
                LastName: "",
                Email: "bad",
                PhoneCountryCode: "",
                PhoneNumber: "",
                AddressLine1: "",
                AddressLine2: null,
                City: "",
                StateOrProvince: "",
                PostalCode: "",
                CountryCode: "N"
            );

            // Act
            var response = await _client.PostAsJsonAsync("/api/customers", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var envelope = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
            envelope!.Success.Should().BeFalse();
            envelope.Error!.Code.Should().Be(ErrorCodes.ValidationError);
            envelope.Error.Details.Should().NotBeNull();
        }
    }
}
