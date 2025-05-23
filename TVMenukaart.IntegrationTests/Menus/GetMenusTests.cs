using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TVMenukaart.DTO;
using TVMenukaart.IntegrationTests.Abstractions;
using Xunit;

namespace TVMenukaart.IntegrationTests.Menus
{
    public class GetMenusTests : BaseFunctionalTest
    {
        public GetMenusTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Should_ReturnOK_AndNoMenus()
        {
            // Arrange
            var client = await CreateClientWithAuth();
            
            // Act
            var response = await client.GetAsync("api/menu");
            var result = await response.Content.ReadFromJsonAsync<IEnumerable<MenuDto>>();
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.Empty(result);
        }
        
        [Fact]
        public async Task Should_ReturnOK_AndOneMenu()
        {
            // Arrange
            var client = await CreateClientWithAuth();
            var menu = DataSeeder.GetTestMenu();
            
            // Act
            var response = await client.GetAsync($"api/menu/{menu.Id}");
            var result = await response.Content.ReadFromJsonAsync<MenuDto>();
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.NotNull(result);
        }
    }
}
