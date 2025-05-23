using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TVMenukaart.DTO;
using TVMenukaart.IntegrationTests.Abstractions;
using Xunit;

namespace TVMenukaart.IntegrationTests.MenuItems
{
    public class UpdateMenuItemsTests : BaseFunctionalTest
    {
        public UpdateMenuItemsTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Should_ReturnOK_WhenMenuItemIsUpdated()
        {
            // Arrange
            var appUser = DataSeeder.GetTestUser();
            var client = await CreateClientWithAuth(appUser.UserName);
            var menuItem = DataSeeder.GetTestMenuItem();

            // Act
            var response = await client.PutAsJsonAsync($"api/menuitems/{menuItem.Id}", menuItem);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Should_ReturnBadRequest_WhenIdIsNotEqualToIdPayload()
        {
            // Arrange
            var appUser = DataSeeder.GetTestUser();
            var client = await CreateClientWithAuth(appUser.UserName);
            var menuItem = DataSeeder.GetTestMenuItem();

            // Act
            var response = await client.PutAsJsonAsync($"api/menuitems/99999999", menuItem);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Should_ReturnNotFound_WhenMenuItemIsNotFound()
        {
            // Arrange
            var appUser = DataSeeder.GetTestUser();
            var client = await CreateClientWithAuth(appUser.UserName);
            var menuItem = new MenuItemDto
            {
                Id = 99999999
            };

            // Act
            var response = await client.PutAsJsonAsync($"api/menuitems/99999999", menuItem);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
