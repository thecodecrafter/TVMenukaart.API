using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TVMenukaart.DTO;
using TVMenukaart.IntegrationTests.Abstractions;
using TVMenukaart.Models;
using Xunit;

namespace TVMenukaart.IntegrationTests.Restaurants
{
    public class GetRestaurantsTests : BaseFunctionalTest
    {
        public GetRestaurantsTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
        }
        
        [Fact]
        public async Task Should_ReturnOK_WhenGettingRestaurantOfUser()
        {
            // Arrange
            var appUser = DataSeeder.GetTestUser();
            var client = await CreateClientWithAuth(appUser.UserName);
            var restaurants = appUser.Restaurants;
            
            // Act
            var response = await client.GetAsync("api/restaurant");
            var result = await response.Content.ReadFromJsonAsync<IEnumerable<RestaurantDto>>();
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.Equal(restaurants.Count, result.Count());
        }
        
        [Fact]
        public async Task Should_ReturnOK_WhenGettingRestaurantById()
        {
            // Arrange
            var appUser = DataSeeder.GetTestUser();
            var client = await CreateClientWithAuth(appUser.UserName);
            var restaurant = appUser.Restaurants.FirstOrDefault();

            if (restaurant == null)
            {
                restaurant = new Restaurant()
                {
                    Name = "TestRestaurant"
                };
                appUser.Restaurants.Add(restaurant);
                await DbContext.SaveChangesAsync();
            }
            
            // Act
            var response = await client.GetAsync($"api/restaurant/{restaurant.Id}");
            var result = await response.Content.ReadFromJsonAsync<RestaurantDto>();
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Name.Should().Be(restaurant.Name);
        }
        
        [Fact]
        public async Task Should_ReturnNotFound_WhenRestaurantIsNotFound()
        {
            // Arrange
            var client = await CreateClientWithAuth();
            
            // Act
            var response = await client.GetAsync($"api/restaurant/99999999");
            var result = await response.Content.ReadFromJsonAsync<RestaurantDto>();
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
