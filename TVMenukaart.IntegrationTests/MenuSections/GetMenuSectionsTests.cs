using System.Net;
using FluentAssertions;
using TVMenukaart.IntegrationTests.Abstractions;
using Xunit;

namespace TVMenukaart.IntegrationTests.MenuSections
{
    public class GetMenuSectionsTests : BaseFunctionalTest
    {
        public GetMenuSectionsTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Should_ReturnOK_WhenGettingMenuSections()
        {
            // Arrange
            var client = await CreateClientWithAuth();
            var menu = DataSeeder.GetTestMenu();

            // Act
            var response = await client.GetAsync($"api/MenuSection/admin/{menu.Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Should_ReturnNotFound_WhenMenuDoesNotExist()
        {
            // Arrange
            var client = await CreateClientWithAuth();

            // Act
            var response = await client.GetAsync($"api/MenuSection/admin/999999");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
