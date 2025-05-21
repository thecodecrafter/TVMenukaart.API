using System.Net;
using FluentAssertions;
using TVMenukaart.IntegrationTests.Abstractions;
using Xunit;

namespace TVMenukaart.IntegrationTests.Menu
{
    public class DeleteMenusTests : BaseFunctionalTest
    {
        public DeleteMenusTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Should_ReturnNoContentResult_WhenMenuIsDeleted()
        {
            // Arrange
            var appUser = DataSeeder.GetTestUser();
            var client = await CreateClientWithAuth(appUser.UserName);
            var menu = DataSeeder.GetTestMenu();

            // Act
            var response = await client.DeleteAsync($"api/menu/{menu.Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Should_ReturnNotFound_WhenMenuIsNotFound()
        {
            // Arrange
            var appUser = DataSeeder.GetTestUser();
            var client = await CreateClientWithAuth(appUser.UserName);

            // Act
            var response = await client.DeleteAsync($"api/menu/9999999");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
