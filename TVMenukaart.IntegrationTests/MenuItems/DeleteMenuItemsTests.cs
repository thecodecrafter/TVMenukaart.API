using System.Net;
using FluentAssertions;
using TVMenukaart.IntegrationTests.Abstractions;
using Xunit;

namespace TVMenukaart.IntegrationTests.MenuItems
{
    public class DeleteMenuItemsTests : BaseFunctionalTest
    {
        public DeleteMenuItemsTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Should_ReturnNotFound_WhenMenuItemIsNotFound()
        {
            // Arrange
            var appUser = DataSeeder.GetTestUser();
            var client = await CreateClientWithAuth(appUser.UserName);

            // Act
            var response = await client.DeleteAsync($"api/menuitems/99999999");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Should_ReturnNoContent_WhenMenuItemIsDeleted()
        {
            // Arrange
            var appUser = DataSeeder.GetTestUser();
            var client = await CreateClientWithAuth(appUser.UserName);
            var menuItem = DataSeeder.GetTestMenuItem();

            // Act
            var response = await client.DeleteAsync($"api/menuitems/{menuItem.Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}
