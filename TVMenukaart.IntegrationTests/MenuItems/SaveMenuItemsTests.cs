using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TVMenukaart.DTO;
using TVMenukaart.IntegrationTests.Abstractions;
using Xunit;

namespace TVMenukaart.IntegrationTests.MenuItems
{
    public class SaveMenuItemsTests : BaseFunctionalTest
    {
        public SaveMenuItemsTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Should_ReturnOK_WhenMenuItemIsCreated()
        {
            // Arrange
            var appUser = DataSeeder.GetTestUser();
            var client = await CreateClientWithAuth(appUser.UserName);
            var menuSection = DataSeeder.GetTestMenuSection();

            var menuItem = new MenuItemDto
            {
                Name = "Test",
                Description = "Description",
                Price = 4,
                MenuSectionId = menuSection.Id
            };

            // Act
            var response = await client.PostAsJsonAsync("api/menuitems", menuItem);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
