using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TVMenukaart.DTO;
using TVMenukaart.IntegrationTests.Abstractions;
using Xunit;

namespace TVMenukaart.IntegrationTests.Menu
{
    public class SaveMenusTests : BaseFunctionalTest
    {
        public SaveMenusTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
        }
        
        [Fact]
        public async Task Should_ReturnOK_WhenMenuIsCreated()
        {
            // Arrange
            var appUser = DataSeeder.GetTestUser();
            var client = await CreateClientWithAuth(appUser.UserName);
            var restaurant = DataSeeder.GetTestRestaurantByUser(appUser.Id);
            
            // Act
            var response = await client.PostAsJsonAsync($"api/menu?name=testMenu&restaurantId={restaurant.Id}", new {});
            var result = await response.Content.ReadFromJsonAsync<MenuDto>();
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        
        [Fact]
        public async Task Should_ReturnNotFound_WhenRestaurantIsNotFound()
        {
            // Arrange
            var appUser = DataSeeder.GetTestUser();
            var client = await CreateClientWithAuth(appUser.UserName);
            
            // Act
            var response = await client.PostAsJsonAsync($"api/menu?name=testMenu&restaurantId=9876", new {});
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Fact]
        public async Task Should_ReturnUnauthorized_WhenUserIsNotOwnerOfRestaurant()
        {
            // Arrange
            var appUser = DataSeeder.GetTestUser();
            var appUser2 = DataSeeder.GetTestUser(1);
            var client = await CreateClientWithAuth(appUser2.UserName);
            var restaurant = DataSeeder.GetTestRestaurantByUser(appUser.Id);
            
            // Act
            var response = await client.PostAsJsonAsync($"api/menu?name=testMenu&restaurantId={restaurant.Id}", new {});
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
