using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TVMenukaart.DTO;
using TVMenukaart.IntegrationTests.Abstractions;
using Xunit;

namespace TVMenukaart.IntegrationTests.Menus
{
    public class UpdateMenusTests : BaseFunctionalTest
    {
        public UpdateMenusTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Should_ReturnNoContentResult_WhenMenuIsUpdated()
        {
            // Arrange
            var appUser = DataSeeder.GetTestUser();
            var client = await CreateClientWithAuth(appUser.UserName);
            var menu = DataSeeder.GetTestMenu();
            var updatedMenu = new MenuDto
            {
                Id = menu.Id,
                Name = "A whole different name"
            };

            // Act
            var response = await client.PutAsJsonAsync($"api/menu/{menu.Id}", updatedMenu);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Should_ReturnBadRequest_WhenMenuIdIsDifferentThanGivenId()
        {
            // Arrange
            var appUser = DataSeeder.GetTestUser();
            var client = await CreateClientWithAuth(appUser.UserName);
            var menu = DataSeeder.GetTestMenu();
            var updatedMenu = new MenuDto
            {
                Id = menu.Id,
                Name = "A whole different name"
            };

            // Act
            var response = await client.PutAsJsonAsync($"api/menu/999999", updatedMenu);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Should_ReturnNotFound_WhenMenuIsNotFound()
        {
            // Arrange
            var appUser = DataSeeder.GetTestUser();
            var client = await CreateClientWithAuth(appUser.UserName);
            var menu = DataSeeder.GetTestMenu();
            var updatedMenu = new MenuDto
            {
                Id = 999999,
                Name = "A whole different name"
            };

            // Act
            var response = await client.PutAsJsonAsync($"api/menu/999999", updatedMenu);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
