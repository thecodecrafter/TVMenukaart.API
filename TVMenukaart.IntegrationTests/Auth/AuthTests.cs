using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TVMenukaart.Controllers;
using TVMenukaart.DTO;
using TVMenukaart.IntegrationTests.Abstractions;
using TVMenukaart.Models;
using Xunit;

namespace TVMenukaart.IntegrationTests.Auth
{
    public class AuthTests : BaseFunctionalTest
    {
        public AuthTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Should_ReturnOk_WhenDeviceCodeIsGenerated()
        {
            // Arrange
            var response = await HttpClient.GetAsync("api/auth/device-code");
            
            // Act
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Should_ReturnIsAuthenticatedFalse_WhenUserByPollingTokenIsNotFound()
        {
            // Arrange
            var deviceCodeResponse = await HttpClient.GetAsync("api/auth/device-code");
            var deviceCode = await deviceCodeResponse.Content.ReadFromJsonAsync<DeviceCode>();
            var request = new PollingRequest() { PollingToken = deviceCode.PollingToken };

            // Act
            var pollResponse = await HttpClient.PostAsJsonAsync("api/auth/poll", request);
            
            // Assert
            var pollResult = await pollResponse.Content.ReadFromJsonAsync<PollingResponse>(); 
            pollResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            pollResult.IsAuthenticated.Should().BeFalse();
        }
        
        [Fact]
        public async Task Should_ReturnIsAuthenticatedTrue_WhenUserByPollingTokenIsFound()
        {
            // Arrange
            var clientWithAuth = await CreateClientWithAuth();
            var deviceCodeResponse = await HttpClient.GetAsync("api/auth/device-code");
            var deviceCode = await deviceCodeResponse.Content.ReadFromJsonAsync<DeviceCode>();
            var request = new PollingRequest() { PollingToken = deviceCode.PollingToken };

            // Act
            await clientWithAuth.PostAsJsonAsync("api/auth/verify", new VerifyCodeRequest() { Code = deviceCode.Code });
            var pollResponse = await HttpClient.PostAsJsonAsync("api/auth/poll", request);
            
            // Assert
            var pollResult = await pollResponse.Content.ReadFromJsonAsync<PollingResponse>(); 
            pollResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            pollResult.IsAuthenticated.Should().BeTrue();
        }
        
        [Fact]
        public async Task Should_ReturnOk_CodeIsVerified()
        {
            // Arrange
            var clientWithAuth = await CreateClientWithAuth();
            
            var deviceCodeResponse = await HttpClient.GetAsync("api/auth/device-code");
            var deviceCode = await deviceCodeResponse.Content.ReadFromJsonAsync<DeviceCode>();

            // Act
            var verifyRequest = new VerifyCodeRequest() { Code = deviceCode.Code };
            var verifyResponse = await clientWithAuth.PostAsJsonAsync("api/auth/verify", verifyRequest);
            var verifyResult = await verifyResponse.Content.ReadAsStringAsync();
            
            // Assert
            var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(verifyResult);
            Assert.True(result.ContainsKey("message"));
            Assert.Equal("Device successfully linked!", result["message"]);
        }
        
        [Fact]
        public async Task Should_ReturnBadRequest_WhenNoDeviceCodeIsFound()
        {
            // Arrange
            var clientWithAuth = await CreateClientWithAuth();
            var verifyRequest = new VerifyCodeRequest() { Code = "BOGUS" };

            // Act
            var verifyResponse = await clientWithAuth.PostAsJsonAsync("api/auth/verify", verifyRequest);
            var verifyResult = await verifyResponse.Content.ReadAsStringAsync();
            
            // Assert
            verifyResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(verifyResult);
            Assert.True(result.ContainsKey("message"));
            Assert.Equal("Invalid or expired code.", result["message"]);
        }
    }
}
