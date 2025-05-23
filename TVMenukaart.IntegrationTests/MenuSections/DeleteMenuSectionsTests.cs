using System.Net;
using FluentAssertions;
using TVMenukaart.IntegrationTests.Abstractions;
using Xunit;

namespace TVMenukaart.IntegrationTests.MenuSections
{
    public class DeleteMenuSectionsTests : BaseFunctionalTest
    {
        public DeleteMenuSectionsTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
        }
        
        [Fact]
        public async Task Should_ReturnNotFound_WhenMenuSectionIsNotFound()
        {
            // Arrangee
            var appUser = DataSeeder.GetTestUser();
            var client = await CreateClientWithAuth(appUser.UserName);

            // Act
            var response = await client.DeleteAsync($"api/menusection/999999");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Should_ReturnNoContent_WhenMenuSectionIsDeleted()
        {
            // Arrange
            var appUser = DataSeeder.GetTestUser();
            var client = await CreateClientWithAuth(appUser.UserName);
            var menuSection = DataSeeder.GetTestMenuSection();
        
            // Act
            var response = await client.DeleteAsync($"api/menusection/{menuSection.Id}");
        
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}
