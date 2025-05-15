using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using TVMenukaart.DTO;
using TVMenukaart.UnitTests.Abstractions;
using Xunit;
using Assert = Xunit.Assert;

namespace TVMenukaart.UnitTests.Users
{
    public class AccountControllerTests : BaseFunctionalTest
    {
        private readonly FunctionalTestWebAppFactory _factory;

        public AccountControllerTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Should_ReturnNotFound_WhenUserIsNotFound()
        {
            // Arrange
            var request = new LoginDto { Username = "test2", Password = "Password@1" };

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

        // [Fact]
        // public async Task Should_ReturnUser_WhenTokenIsRefreshed()
        // {
        //     // Arrange
        //     var loginRequest = new LoginDto { Username = "test", Password = "Password@1" };
        //     var request = new RegisterDto { Username = "test", Password = "Password@1" };
        //     await HttpClient.PostAsJsonAsync("api/Account/register", request);
        //     var loginResponse = await HttpClient.PostAsJsonAsync("api/Account/login", loginRequest);
        //     var loginResult = await loginResponse.Content.ReadFromJsonAsync<UserDto>();
        //     // loginResult.refre
        //
        //     // Act
        //     HttpContent content = new StringContent("");
        //     var response = await HttpClient.PostAsync("api/account/refreshtoken", content);
        //
        //     // Assert
        //     var result = await response.Content.ReadFromJsonAsync<UserDto>();
        //
        //     response.StatusCode.Should().Be(HttpStatusCode.OK);
        //     result.Should().NotBeNull();
        // }

        [Fact]
        public async Task RefreshToken_WithValidToken_ReturnsNewUserDto()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Username = "testuser",
                Password = "Password123!"
            };

            // Register a user first
            var registerResponse = await HttpClient.PostAsJsonAsync("api/account/register", registerDto);
            registerResponse.EnsureSuccessStatusCode();

            // Login to get the JWT token
            var loginDto = new LoginDto
            {
                Username = registerDto.Username,
                Password = registerDto.Password
            };
            var loginResponse = await HttpClient.PostAsJsonAsync("api/account/login", loginDto);
            loginResponse.EnsureSuccessStatusCode();
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<UserDto>();

            // Get the refresh token from cookies
            var refreshTokenCookie = loginResponse.Headers
                .GetValues("Set-Cookie")
                .FirstOrDefault(x => x.StartsWith("refreshToken="));

            // Extract just the token value
            //var refreshToken = refreshTokenCookie?.Split(';')[0];  // Gets just the "refreshToken=value" part
            var refreshTokenValue = refreshTokenCookie?
                .Split(';')[0] // Get just the "refreshToken=value" part
                .Split('=')[1]; // Get just the value
            refreshTokenValue = WebUtility.UrlDecode(refreshTokenValue); // Decode the value

            var decodedCookieHeader = $"refreshToken={refreshTokenValue}";

            

            // Act
            var clientWithAuth = CreateClient(loginResult.Token);
            if (refreshTokenCookie != null)
            {
                clientWithAuth.DefaultRequestHeaders.Add("Cookie", decodedCookieHeader);
            }
            var response = await clientWithAuth.PostAsync("api/account/refreshToken", null);

            // Add debug logging for failure case
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error Response: {error}");
                Console.WriteLine($"Status Code: {response.StatusCode}");
            }

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task RefreshToken_WithValidAuthentication_ShouldSucceed()
        {
            // Arrange
            // 1. Register a user
            var registerDto = new RegisterDto
            {
                Username = "testuser",
                Password = "Password123!"
            };
            await HttpClient.PostAsJsonAsync("api/account/register", registerDto);

            // 2. Login to get the token
            var loginDto = new LoginDto
            {
                Username = "testuser",
                Password = "Password123!"
            };
            var loginResponse = await HttpClient.PostAsJsonAsync("api/account/login", loginDto);
            loginResponse.EnsureSuccessStatusCode();
            var userDto = await loginResponse.Content.ReadFromJsonAsync<UserDto>();

            // Create a new request with both JWT and refresh token
            var cookies = new CookieContainer();

            var setCookie = loginResponse.Headers.GetValues("Set-Cookie").FirstOrDefault();
            var refreshToken = setCookie.Split(';').FirstOrDefault(x => x.Trim().StartsWith("refreshToken="))?.Split('=')[1];
            cookies.Add(new Uri("https://localhost"), new Cookie("refreshToken", refreshToken));

            var clientWithAuth = CreateClient(userDto.Token, cookies);

            // var clientWithAuth = _factory.CreateClient();
            var responseWho = await clientWithAuth.GetAsync("api/debug/whoami");
            Console.WriteLine(await responseWho.Content.ReadAsStringAsync());

            // Act
            Console.WriteLine("Bearer Token: " + userDto.Token);
            var response = await clientWithAuth.PostAsync("api/account/refreshToken", null);
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Status: {response.StatusCode}, Content: {content}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<UserDto>();
            result.Should().NotBeNull();
            result.Username.Should().Be("testuser");
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
    }
}
