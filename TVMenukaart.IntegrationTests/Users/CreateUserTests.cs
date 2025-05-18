using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using TVMenukaart.DTO;
using TVMenukaart.IntegrationTests.Abstractions;
using Xunit;

namespace TVMenukaart.IntegrationTests.Users
{
    public class CreateUserTests : BaseFunctionalTest
    {
        public CreateUserTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Should_ReturnOk_WhenRequestIsValid()
        {
            // Arrange
            var request = new RegisterDto { Username = "test4", Password = "Password@1" };

            // Act
            var response = await HttpClient.PostAsJsonAsync("api/Account/register", request);

            // Assert
            var result = await response.Content.ReadFromJsonAsync<UserDto>();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task Should_ReturnConflict_WhenUserExists()
        {
            // Arrange
            var request = new RegisterDto { Username = "test5", Password = "Password@1" };
            await HttpClient.PostAsJsonAsync("api/Account/register", request);

            // Act
            var response = await HttpClient.PostAsJsonAsync("api/Account/register", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task Should_ReturnBadRequest_WhenPasswordIsInvalid()
        {
            // Arrange
            var request = new RegisterDto { Username = "test2", Password = "Password@1" };
            var registerResponse = await HttpClient.PostAsJsonAsync("api/Account/register", request);
            registerResponse.EnsureSuccessStatusCode();
            var loginRequest = new LoginDto { Username = "test2", Password = "p" };
            
            // Act
            var response = await HttpClient.PostAsJsonAsync("api/Account/login", loginRequest);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            
            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal("User/password is invalid", result.Title);
            Assert.Equal(404, result.Status);
        }

        [Fact]
        public async Task Should_ReturnBadRequest_WhenUsernameIsMissing()
        {
            // Arrange
            var request = new RegisterDto { Username = "", Password = "Password@1" };

            // Act
            var response = await HttpClient.PostAsJsonAsync("api/Account/register", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Should_ReturnBadRequest_WhenPasswordIsTooShort()
        {
            // Arrange
            var request = new RegisterDto { Username = "test3", Password = "1" };

            // Act
            var response = await HttpClient.PostAsJsonAsync("api/Account/register", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
