using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TVMenukaart.DTO;
using TVMenukaart.IntegrationTests.Abstractions;
using Xunit;

namespace TVMenukaart.IntegrationTests.MenuSections
{
    public class SaveMenuSectionTests : BaseFunctionalTest
    {
        public SaveMenuSectionTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Should_ReturnNotFound_WhenMenuDoesNotExist()
        {
            // Arrange
            var appUser = DataSeeder.GetTestUser();
            var client = await CreateClientWithAuth(appUser.UserName);

            // Act
            var response = await client.PostAsJsonAsync($"api/menusection?menuId={99999}&sectionName=test", new { });

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Should_ReturnOk_WhenMenuSectionIsCreated()
        {
            // Arrange
            var appUser = DataSeeder.GetTestUser();
            var client = await CreateClientWithAuth(appUser.UserName);
            var menu = DataSeeder.GetTestMenu();

            // Act
            var response = await client.PostAsJsonAsync($"api/menusection?menuId={menu.Id}&sectionName=test", new { });
            var result = await response.Content.ReadFromJsonAsync<MenuSectionDto>();

            menu = await DbContext.Menus
                .Include(m => m.MenuSections)
                .FirstOrDefaultAsync(i => i.Id == menu.Id);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Name.Should().Be("test");
            Assert.Contains(result.Name, menu.MenuSections.Select(x => x.Name));
        }
    }
}
