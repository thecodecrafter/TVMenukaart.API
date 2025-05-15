using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TVMenukaart.DTO;
using TVMenukaart.UnitTests.Abstractions;
using Xunit;

namespace TVMenukaart.UnitTests.Users
{
    public class CreateUserTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
    {
        [Fact]
        public async Task Should_ReturnOk_WhenRequestIsValid()
        {
            // Arrange
            var request = new RegisterDto { Username = "test", Password = "Password@1" };

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
            var request = new RegisterDto { Username = "test", Password = "Password@1" };
            await HttpClient.PostAsJsonAsync("api/Account/register", request);

            // Act
            var response = await HttpClient.PostAsJsonAsync("api/Account/register", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
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
        public async Task Should_ReturnBadRequest_WhenPasswordIsMissing()
        {
            // Arrange
            var request = new RegisterDto { Username = "test", Password = "" };

            // Act
            var response = await HttpClient.PostAsJsonAsync("api/Account/register", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
