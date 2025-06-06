using System.Net;
using FluentAssertions;
using TVMenukaart.IntegrationTests.Abstractions;
using TVMenukaart.Models;
using Xunit;

namespace TVMenukaart.IntegrationTests.Restaurants
{
    public class DeleteRestaurantsTests : BaseFunctionalTest
    {
        public DeleteRestaurantsTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
        }
        
        [Fact]
        public async Task Should_ReturnNotFound_WhenRestaurantIsNotFound()
        {
            // Arrange
            var client = await CreateClientWithAuth();

            // Act
            var response = await client.DeleteAsync($"api/restaurant/999999");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Fact]
        public async Task Should_ReturnNoContent_WhenRestaurantIsDeleted()
        {
            // Arrange
            var appUser = DataSeeder.GetTestUser();
            var client = await CreateClientWithAuth();
            var restaurant = new Restaurant()
            {
                Name = "TestRestaurant",
                AppUser = appUser
            };
            
            DbContext.Restaurants.Add(restaurant);
            await DbContext.SaveChangesAsync();

            // Act
            var response = await client.DeleteAsync($"api/restaurant/{restaurant.Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}
