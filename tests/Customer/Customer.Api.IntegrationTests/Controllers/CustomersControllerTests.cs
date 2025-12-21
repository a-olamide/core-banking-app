using Customer.Api.Contracts;
using Customer.Application.Customers.Dtos;
using Customer.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SharedKernel.Api;
using SharedKernel.Web.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Customer.Api.IntegrationTests.Controllers
{
//We now have three complementary integration tests:

//ShouldReturnSuccessEnvelope API contract + MediatR + middleware
//ShouldReturnValidationErrorEnvelope Validation pipeline + error mapping
//ShouldPersistToDatabase EF Core + SQL Server + migrations
    public sealed class CustomersControllerTests :
     IClassFixture<CustomWebApplicationFactory>,
     IAsyncLifetime
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public CustomersControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        // Runs ONCE per test class
        public async Task InitializeAsync()
        {
            await DatabaseFixture.ResetAsync(_factory.Services);
        }

        public Task DisposeAsync() => Task.CompletedTask;

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

        [Fact]
        public async Task Post_Customers_ShouldPersistToDatabase()
        {
            // Arrange
            var request = new CreateCustomerRequest(
                FirstName: "Persist",
                MiddleName: null,
                LastName: "Test",
                Email: "persist@test.com",
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
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Assert persistence
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CustomerDbContext>();

            var customer = await db.Customers
                .SingleOrDefaultAsync(c => c.Email.Value == "persist@test.com");

            customer.Should().NotBeNull();
            customer!.Name.FirstName.Should().Be("Persist");
        }

        [Fact]
        public async Task Post_Customers_ShouldReturnConflict_WhenEmailAlreadyExists()
        {
            // Arrange
            var email = "dup@test.com";

            var first = new CreateCustomerRequest(
                FirstName: "Ola",
                MiddleName: null,
                LastName: "Akin",
                Email: email,
                PhoneCountryCode: "+234",
                PhoneNumber: "8012345678",
                AddressLine1: "12 Admiralty Way",
                AddressLine2: null,
                City: "Lagos",
                StateOrProvince: "Lagos",
                PostalCode: "100001",
                CountryCode: "NG"
            );

            var second = first with { FirstName = "Another" }; // same email, different name

            // Act 1: create first customer
            var r1 = await _client.PostAsJsonAsync("/api/customers", first);
            r1.StatusCode.Should().Be(HttpStatusCode.OK);

            // Act 2: try to create another customer with same email
            var r2 = await _client.PostAsJsonAsync("/api/customers", second);

            // Assert
            r2.StatusCode.Should().Be(HttpStatusCode.Conflict);

            var envelope = await r2.Content.ReadFromJsonAsync<ApiResponse<object>>();

            envelope.Should().NotBeNull();
            envelope!.Success.Should().BeFalse();
            envelope.Error.Should().NotBeNull();
            envelope.Error!.Code.Should().Be(ErrorCodes.CustomerEmailAlreadyExists);
            envelope.TraceId.Should().NotBeNullOrEmpty();
        }
    }
}
