using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TVMenukaart.DTO;
using TVMenukaart.IntegrationTests.Abstractions;
using Xunit;

namespace TVMenukaart.IntegrationTests.MenuItems
{
    public class GetMenuItemsTests : BaseFunctionalTest
    {
        public GetMenuItemsTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Should_ReturnOK_WhenGettingMenuItems()
        {
            // Arrange
            var client = await CreateClientWithAuth();
            var menuSection = DataSeeder.GetTestMenuSection();

            // Act
            var response = await client.GetAsync($"api/menuItems/menuSection/{menuSection.Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Should_ReturnNotFound_WhenMenuSectionDoesExist()
        {
            // Arrange
            var client = await CreateClientWithAuth();

            // Act
            var response = await client.GetAsync($"api/menuItems/menuSection/{999999999}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Should_ReturnOk_WhenGettingMenuItem()
        {
            // Arrange
            var client = await CreateClientWithAuth();
            var menuItem = DataSeeder.GetTestMenuItem();

            // Act
            var response = await client.GetAsync($"api/menuItems/{menuItem.Id}");
            var result = await response.Content.ReadFromJsonAsync<MenuItemDto>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Should_ReturnNotFound_WhenMenuItemDoesNotExist()
        {
            // Arrange
            var client = await CreateClientWithAuth();

            // Act
            var response = await client.GetAsync($"api/menuItems/{99999999}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
