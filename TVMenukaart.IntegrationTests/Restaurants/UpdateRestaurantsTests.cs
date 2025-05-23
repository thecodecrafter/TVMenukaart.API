using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TVMenukaart.DTO;
using TVMenukaart.IntegrationTests.Abstractions;
using Xunit;

namespace TVMenukaart.IntegrationTests.Restaurants
{
    public class UpdateRestaurantsTests : BaseFunctionalTest
    {
        public UpdateRestaurantsTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
        }
        
        [Fact]
        public async Task Should_ReturnOK_WhenRestaurantIsUpdated()
        {
            // Arrange
            var client = await CreateClientWithAuth();
            var restaurant = DataSeeder.GetTestRestaurant();
            
            // Act
            var response = await client.PutAsJsonAsync($"api/restaurant", restaurant);
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        
        [Fact]
        public async Task Should_ReturnNotFound_WhenRestaurantIsNotFound()
        {
            // Arrange
            var client = await CreateClientWithAuth();
            var restaurant = new RestaurantDto
            {
                Id = 999999
            };
            
            // Act
            var response = await client.PutAsJsonAsync($"api/restaurant", restaurant);
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
