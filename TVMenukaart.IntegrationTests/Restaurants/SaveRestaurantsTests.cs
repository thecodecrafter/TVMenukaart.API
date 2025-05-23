using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TVMenukaart.DTO;
using TVMenukaart.IntegrationTests.Abstractions;
using Xunit;

namespace TVMenukaart.IntegrationTests.Restaurants
{
    public class SaveRestaurantsTests : BaseFunctionalTest
    {
        public SaveRestaurantsTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
        }
        
        [Fact]
        public async Task Should_ReturnOK_WhenRestaurantIsCreated()
        {
            // Arrange
            var client = await CreateClientWithAuth();
            var restaurant = new RestaurantDto
            {
                Name = "TestRestaurant",
            };
            
            // Act
            var response = await client.PostAsJsonAsync($"api/restaurant", restaurant);
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
