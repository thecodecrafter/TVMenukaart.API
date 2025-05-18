using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using TVMenukaart.DTO;
using TVMenukaart.IntegrationTests.Abstractions;
using Xunit;
using Assert = Xunit.Assert;

namespace TVMenukaart.IntegrationTests.Users
{
    public class AccountControllerTests : BaseFunctionalTest
    {
        public AccountControllerTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Should_ReturnNotFound_WhenUserIsNotFound()
        {
            // Arrange
            var request = new LoginDto { Username = "userNotFound", Password = "Password@1" };

            // Act
            var response = await HttpClient.PostAsJsonAsync("api/Account/login", request);

            // Assert

            var result = await response.Content.ReadFromJsonAsync<UserDto>();
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task Should_ReturnUser_WhenUserIsFound()
        {
            // Arrange
            var loginRequest = new LoginDto { Username = "test", Password = "Password@1" };
            var request = new RegisterDto { Username = "test", Password = "Password@1" };
            await HttpClient.PostAsJsonAsync("api/Account/register", request);

            // Act
            var response = await HttpClient.PostAsJsonAsync("api/Account/login", loginRequest);

            // Assert
            var result = await response.Content.ReadFromJsonAsync<UserDto>();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task RefreshToken_WithValidToken_ReturnsNewUserDto()
        {
            // Arrange
            var clientWithAuth = await CreateClientWithAuth();
            
            // Act
            var response = await clientWithAuth.PostAsync("api/account/refreshToken", null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task RefreshToken_WithValidAuthentication_ShouldSucceed()
        {
            // Arrange
            var clientWithAuth = await CreateClientWithAuth();

            // Act
            var response = await clientWithAuth.PostAsync("api/account/refreshToken", null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<UserDto>();
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task RefreshToken_WithoutAuth_ReturnsUnauthorized()
        {
            // Act
            var response = await HttpClient.PostAsync("api/account/refreshToken", null);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task RefreshToken_WithInvalidToken_ReturnsUnauthorized()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Username = "testuser2",
                Password = "Password123!"
            };

            // Register a user first
            var registerResponse = await HttpClient.PostAsJsonAsync("api/account/register", registerDto);
            var registerResult = await registerResponse.Content.ReadFromJsonAsync<UserDto>();
            Assert.NotNull(registerResult?.Token);

            // Add an invalid refresh token cookie
            HttpClient.DefaultRequestHeaders.Add("Cookie", "refreshToken=invalid_token_value");

            // Add the JWT token to the authorization header
            HttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", registerResult.Token);

            // Act
            var response = await HttpClient.PostAsync("api/account/refreshToken", null);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Should_ReturnBadRequest_WhenPasswordIsMissing()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Username = "testuser2",
                Password = ""
            };

            // Act
            var response = await HttpClient.PostAsJsonAsync("api/account/register", registerDto);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
